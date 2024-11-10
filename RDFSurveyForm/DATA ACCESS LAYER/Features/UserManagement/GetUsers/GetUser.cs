using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Model;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.GetUsers
{
    public partial class GetUser
    {

        public class Handler : IRequestHandler<GetUserQuery, PagedList<GetUserResult>>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetUserResult>> Handle(GetUserQuery request, CancellationToken cancellationToken)
            {

                IQueryable<User> userQuery = _context.Users
                    .AsNoTrackingWithIdentityResolution()
                    .Include(r => r.Role)
                    .Include(r => r.Groups)
                    .Include(r => r.Department)
                    .AsSplitQuery();


                if (!string.IsNullOrEmpty(request.Search))
                    userQuery = userQuery.Where(r => r.FullName.ToLower().Contains(request.Search));

                if (request.Is_Archive is not null)
                    userQuery = userQuery.Where(r => r.IsActive == request.Is_Archive);


                var results = userQuery
                    .Select(r => new GetUserResult
                    {
                        Id = r.Id,
                        Fullname = r.FullName,
                        Username = r.UserName,
                        RoleId = r.RoleId,
                        Role_Name = r.Role.RoleName,
                        GroupsId = r.Groups.Id,
                        Group_Name = r.Groups.GroupName,
                        DepartmentId = r.DepartmentId,
                        Department_Name = r.Department.DepartmentName,
                        Created_By = r.CreatedBy,
                        Created_At = r.CreatedAt,
                        Updated_By = r.EditedBy,
                        Updated_At = r.EditedAt,
                        Is_Archive = r.IsActive
                        
                    });


                return await PagedList<GetUserResult>.CreateAsync(results, request.PageNumber, request.PageSize);

            
            }


        }
    }
}
