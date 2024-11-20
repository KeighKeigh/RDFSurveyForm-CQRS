using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Model;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.GetUsers.GetUser;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.DepartmentManagement.GetDepartment
{
    public class GetDepartment
    {
        public class GetDepartmentQuery : UserParams, IRequest<PagedList<GetDepartmentResult>>
        {
            public string Search { get; set; }
            public bool? Is_Archive { get; set; }

        }

        public record class GetDepartmentResult
        {
            public int Id { get; set; }
            public string DepartmentName { get; set; }
            public int? DepartmentNo { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public string EditedBy { get; set; }
        }

        public class Handler : IRequestHandler<GetDepartmentQuery, PagedList<GetDepartmentResult>>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            async public Task<PagedList<GetDepartmentResult>> Handle(GetDepartmentQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Department> departmentQuery = _context.Department
                    .AsNoTrackingWithIdentityResolution()
                    .AsSplitQuery();


                if (!string.IsNullOrEmpty(request.Search))
                    departmentQuery = departmentQuery.Where(r => r.DepartmentName.ToLower().Contains(request.Search));

                if (request.Is_Archive is not null)
                    departmentQuery = departmentQuery.Where(r => r.IsActive == request.Is_Archive);


                var results = departmentQuery
                    .Select(r => new GetDepartmentResult
                    {
                        Id = r.Id,
                        DepartmentName = r.DepartmentName,
                        DepartmentNo = r.DepartmentNo,
                        IsActive = r.IsActive,
                        CreatedAt = r.CreatedAt,
                        EditedBy = r.EditedBy,

                    });


                return await PagedList<GetDepartmentResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
