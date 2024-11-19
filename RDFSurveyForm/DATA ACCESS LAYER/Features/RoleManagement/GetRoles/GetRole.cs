using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Dto.ModelDto.RoleDto;
using RDFSurveyForm.Model;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.GetUsers.GetUser;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.RoleManagement.GetRoles
{
    public class GetRole
    {
        public record class GetRoleResult
        {
            public int Id { get; set; }
            public string RoleName { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsActive { get; set; }
            public ICollection<string> Permission { get; set; }
            public string EditedBy { get; set; }
        }
        public class GetRoleQuery : UserParams, IRequest<PagedList<GetRoleResult>>
        {
            public string Search { get; set; }
            public bool? Is_Archive { get; set; }
        }


        public class Handler : IRequestHandler<GetRoleQuery, PagedList<GetRoleResult>>
        {
            private readonly StoreContext _context;
            public Handler(StoreContext context)
            {
                _context = context;            
            }

            public async Task<PagedList<GetRoleResult>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Role> userQuery = _context.Roles
                    .AsNoTrackingWithIdentityResolution()
                    .AsSplitQuery();

                if (!string.IsNullOrEmpty(request.Search))
                    userQuery = userQuery.Where(r => r.RoleName.ToLower().Contains(request.Search));

                if (request.Is_Archive is not null)
                    userQuery = userQuery.Where(r => r.IsActive == request.Is_Archive);

                var result = userQuery
                    .Select(x => new GetRoleResult
                    {
                    Id = x.Id,
                    RoleName = x.RoleName,
                    CreatedAt = DateTime.Now,
                    IsActive = x.IsActive,
                    Permission = x.Permission,
                    EditedBy = x.EditedBy,

                    });

                return await PagedList<GetRoleResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }


        }
    }
}
