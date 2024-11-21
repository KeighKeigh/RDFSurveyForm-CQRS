using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Model;
using RDFSurveyForm.Model.Setup;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.GetUsers.GetUser;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupManagement.GetGroup
{
    public class GetGroup
    {
        public record class GetGroupResult
        {
            public int Id { get; set; }
            public string GroupName { get; set; }
            public bool IsActive { get; set; } = true;
            public DateTime CreatedAt { get; set; }
            public string CreatedBy { get; set; }
            public int? BranchId { get; set; }
            public string BranchName { get; set; }
            public DateTime UpdatedAt { get; set; }
            public string UpdatedBy { get; set; }
        }

        public class GetGroupQuery : UserParams, IRequest<PagedList<GetGroupResult>>
        {
            public string Search { get; set; }
            public bool? Is_Archive { get; set; }
        }

        public class Handler : IRequestHandler<GetGroupQuery, PagedList<GetGroupResult>>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            async public Task<PagedList<GetGroupResult>> Handle(GetGroupQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Groups> groupQuery = _context.Groups
                    .AsNoTrackingWithIdentityResolution()
                    .AsSplitQuery();


                if (!string.IsNullOrEmpty(request.Search))
                    groupQuery = groupQuery.Where(r => r.GroupName.ToLower().Contains(request.Search));

                if (request.Is_Archive is not null)
                    groupQuery = groupQuery.Where(r => r.IsActive == request.Is_Archive);


                var results = groupQuery
                    .Select(r => new GetGroupResult
                    {
                        Id = r.Id,
                        GroupName = r.GroupName,
                        IsActive = r.IsActive,
                        CreatedAt = r.CreatedAt,
                        CreatedBy = r.CreatedBy,
                        BranchId = r.BranchId,
                        BranchName = r.Branch.BranchName,
                        UpdatedAt = r.UpdatedAt,
                        UpdatedBy = r.UpdatedBy,

                    });


                return await PagedList<GetGroupResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
