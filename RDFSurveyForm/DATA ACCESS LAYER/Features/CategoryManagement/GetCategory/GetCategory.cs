using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Model;
using RDFSurveyForm.Model.Setup;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.GetUsers.GetUser;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.CategoryManagement.GetCategory
{
    public class GetCategory
    {
        public record class GetCategoryResult
        {
            public int Id { get; set; }
            public string CategoryName { get; set; }
            public decimal CategoryPercentage { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public string CreatedBy { get; set; }
            public DateTime UpdatedAt { get; set; }
            public string UpdatedBy { get; set; }
            public decimal Limit { get; set; } = 100;
        }
        public class GetCategoryQuery : UserParams, IRequest<PagedList<GetCategoryResult>>
        {
            public string Search { get; set; }
            public bool? Is_Archive { get; set; }
        }

        public class Handler : IRequestHandler<GetCategoryQuery, PagedList<GetCategoryResult>>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            async public Task<PagedList<GetCategoryResult>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Category> categoryQuery = _context.Category
                    .AsNoTrackingWithIdentityResolution()
                    .AsSplitQuery();


                if (!string.IsNullOrEmpty(request.Search))
                    categoryQuery = categoryQuery.Where(r => r.CategoryName.ToLower().Contains(request.Search));

                if (request.Is_Archive is not null)
                    categoryQuery = categoryQuery.Where(r => r.IsActive == request.Is_Archive);


                var results = categoryQuery
                    .Select(r => new GetCategoryResult
                    {
                        Id = r.Id,
                        CategoryName = r.CategoryName,
                        CategoryPercentage = r.CategoryPercentage * 100,
                        IsActive = r.IsActive,
                        CreatedAt = r.CreatedAt,
                        CreatedBy = r.CreatedBy,
                        UpdatedAt = r.UpdatedAt,
                        UpdatedBy = r.UpdatedBy,
                        Limit = r.Limit,
                     

                    });


                return await PagedList<GetCategoryResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
