using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Dto.SetupDto.GroupSurveyDto;
using RDFSurveyForm.Model;
using RDFSurveyForm.Model.Setup;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupSurveyManagement.GetGroupSurveys
{
    public class GetGroupSurvey
    {
        public record class GetGroupSurveyResult
        {
            public int? SurveyGeneratorId { get; set; }

            public string BranchName { get; set; }
            public string GroupName { get; set; }
            public bool IsTransacted { get; set; }
            public decimal FinalScore { get; set; }
            public bool IsActive { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedAt { get; set; }
            public string UpdatedBy { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
        public record class GetScoreResult
        {
            public int? SurveyGeneratorId { get; set; }
            public int? Id { get; set; }
            public string CategoryName { get; set; }
            public decimal Score { get; set; }
            public decimal Limit { get; set; }
            public decimal CategoryPercentage { get; set; }
            public decimal SurveyPercentage { get; set; }
            public decimal ActualScore { get; set; }
        }
        public class GetGroupSurveyQuery : UserParams, IRequest<PagedList<GetGroupSurveyResult>>
        {
            public string Search { get; set; }
            public bool? Is_Archive { get; set; }
        }

        public class Handler : IRequestHandler<GetGroupSurveyQuery, PagedList<GetGroupSurveyResult>>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetGroupSurveyResult>> Handle(GetGroupSurveyQuery request, CancellationToken cancellationToken)
            {
                IQueryable<GroupSurvey> surveyscoreQuery = _context.GroupSurvey
                    .AsNoTrackingWithIdentityResolution()
                    .Include(r => r.Groups)
                    .AsSplitQuery();                           

                var totalScore = _context.SurveyScores
            .GroupBy(x => new
            {
                x.SurveyGeneratorId,
                x.Id,
                x.Score,
                x.Limit,
                //Ids = Ids, 
            }).Select(x => new GetScoreResult
            {
                SurveyGeneratorId = x.Key.SurveyGeneratorId,
                Id = x.Key.Id,
                Score = x.Key.Score / x.Key.Limit,
                Limit = x.Key.Limit,
                ActualScore = x.Key.Score
            });


                var categoryPercentage = _context.SurveyScores
                  .GroupJoin(totalScore, total => total.Id, percentage => percentage.Id, (total, percentage) => new { total, percentage })
                  .SelectMany(x => x.percentage.DefaultIfEmpty(), (x, percentage) => new { x.total, percentage })
                  .GroupBy(x => new
                  {
                      x.total.SurveyGeneratorId,
                      x.total.Id,
                      x.total.CategoryPercentage,
                  }).Select(x => new GetScoreResult
                  {
                      SurveyGeneratorId = x.Key.SurveyGeneratorId,
                      Id = x.Key.Id,
                      Score = x.Sum(x => x.percentage.Score) * x.Key.CategoryPercentage,
                      CategoryPercentage = x.Key.CategoryPercentage,
                  });


                var num = 0;
                int? userss = 0;
                var counts = false;
                var number = await _context.GroupSurvey.Where(x => x.GroupsId != 0 && x.IsActive == true).ToListAsync();
                foreach (var group in number)
                {


                    if (group.GroupsId != null && counts == false)
                    {
                        userss = group.GroupsId;
                        counts = true;
                    }

                    if (userss == group.GroupsId)
                    {
                        num++;
                    }

                }

                var results = surveyscoreQuery
                        .GroupJoin(categoryPercentage, score => score.SurveyGeneratorId, percentage => percentage.SurveyGeneratorId, (score, percentage) => new { score, percentage })
                        .SelectMany(x => x.percentage.DefaultIfEmpty(), (x, percentage) => new { x.score, percentage })
                        .GroupBy(x => x.score.GroupsId)
                        .Select(x => new GetGroupSurveyResult
                        {
                            SurveyGeneratorId = x.Key,
                            BranchName = x.First().score.Groups.Branch.BranchName,
                            IsActive = x.First().score.IsActive,
                            CreatedBy = x.First().score.CreatedBy,
                            CreatedAt = x.First().score.CreatedAt,
                            GroupName = x.First().score.Groups.GroupName,
                            IsTransacted = x.First().score.IsTransacted,
                            FinalScore = (x.Sum(x => x.percentage.Score) * 100) / num
                        });


                if (!string.IsNullOrEmpty(request.Search))
                    surveyscoreQuery = surveyscoreQuery.Where(r => r.SurveyGeneratorId.ToString().Contains(request.Search)
                    || Convert.ToString(r.Groups.Branch.BranchName).ToLower().Contains(request.Search)
                    || Convert.ToString(r.Groups.GroupName).ToLower().Contains(request.Search));

                if (request.Is_Archive is not null)
                    surveyscoreQuery = surveyscoreQuery.Where(r => r.IsActive == request.Is_Archive);


                results = results.OrderByDescending(x => x.FinalScore);

                return await PagedList<GetGroupSurveyResult>.CreateAsync(results, request.PageNumber, request.PageSize);

            }
        }
    }
}
