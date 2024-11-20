using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Model;
using RDFSurveyForm.Model.Setup;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.GetUsers.GetUser;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.BranchManagement.GetBranch
{
    public class GetBranch
    {
        public record class GetBranchResult
        {
            public int Id { get; set; }
            public string BranchName { get; set; }
            public string BranchCode { get; set; }
            public bool IsActive { get; set; } = true;
            public DateTime CreatedAt { get; set; }
            public string CreatedBy { get; set; }
            public DateTime UpdatedAt { get; set; }
            public string UpdatedBy { get; set; }
        }

        public class GetBranchQuery : UserParams, IRequest<PagedList<GetBranchResult>>
        {
            public string Search { get; set; }
            public bool? Is_Archive { get; set; }
        }

        public class Handler : IRequestHandler<GetBranchQuery, PagedList<GetBranchResult>>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetBranchResult>> Handle(GetBranchQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Branch> branchQuery = _context.Branches
                    .AsNoTrackingWithIdentityResolution()
                    .Include(r => r.Groups)
                    .AsSplitQuery();


                if (!string.IsNullOrEmpty(request.Search))
                    branchQuery = branchQuery.Where(r => r.BranchName.ToLower().Contains(request.Search));

                if (request.Is_Archive is not null)
                    branchQuery = branchQuery.Where(r => r.IsActive == request.Is_Archive);


                var results = branchQuery
                    .Select(r => new GetBranchResult
                    {
                        Id = r.Id,
                        BranchName = r.BranchName,
                        BranchCode = r.BranchCode,
                        IsActive = r.IsActive,
                        CreatedAt= r.CreatedAt,
                        CreatedBy= r.CreatedBy,
                        UpdatedAt= r.UpdatedAt,
                        UpdatedBy= r.UpdatedBy,

                    });


                return await PagedList<GetBranchResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
