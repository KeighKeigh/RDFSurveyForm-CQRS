using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Dto.SetupDto.GroupSurveyDto;
using RDFSurveyForm.Model;
using RDFSurveyForm.Model.Setup;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupManagement.GetGroup.GetGroup;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupSurveyManagement.ViewGroupSurvey
{
    public class ViewGroupSurveyHandler
    {
        public record class ViewGroupSurveyResult
        {
            public int? SurveyGeneratorId { get; set; }

            public List<Categoria> Categories { get; set; }
            public class Categoria
            {
                public int? Id { get; set; }
                public string CategoryName { get; set; }
                public decimal Score { get; set; }
                public decimal Limit { get; set; }
                public decimal CategoryPercentage { get; set; }
                public decimal SurveyPercentage { get; set; }
            }
        }

        public class ViewGroupSurveyQuery : IRequest<ViewGroupSurveyResult>
        {
            public int? SurveyGeneratorId { get; set; }
        }

        //public class Handler : IRequestHandler<ViewGroupSurveyQuery, ViewGroupSurveyResult>
        //{
        //    private readonly StoreContext _context;

        //    public Handler(StoreContext context)
        //    {
        //        _context = context;
        //    }


            //async public Task<ViewGroupSurveyResult> Handle(ViewGroupSurveyQuery request, CancellationToken cancellationToken)
            //{
            //    var validator = await Validator(request, cancellationToken);
            //    if (validator is not null)
            //        return validator;

            //    var results = _context.SurveyScores.Where(x => x.SurveyGeneratorId == request.SurveyGeneratorId)
            //    .GroupBy(x => x.SurveyGeneratorId).Select(x => new ViewGroupSurveyResult
            //    {
            //        SurveyGeneratorId = x.Key,
            //        Categories = x.Select(x => new ViewGroupSurveyResult.Categoria
            //        {
            //            Id = x.Id,
            //            CategoryName = x.CategoryName,
            //            CategoryPercentage = x.CategoryPercentage * 100,
            //            Score = x.Score,
            //            Limit = x.Limit,
            //            SurveyPercentage = (x.Score / x.Limit) * x.CategoryPercentage,

            //        }).ToList()

            //    }).FirstOrDefaultAsync(cancellationToken);

            //    return await results;
            //}

            //private async Task
        //}
    }
}
