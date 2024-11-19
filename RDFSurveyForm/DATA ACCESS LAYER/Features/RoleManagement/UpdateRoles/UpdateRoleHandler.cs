using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using RDFSurveyForm.Model;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static RDFSurveyForm.Handlers.UpdatePasswordHandler;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.RoleManagement.UpdateRoles
{
    public class UpdateRoleHandler
    {
        public class UpdateRoleCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string RoleName { get; set; }
            public ICollection<string> Permission { get; set; }
            public string EditedBy { get; set; }
        }

        public class Handler : IRequestHandler<UpdateRoleCommand, Result>
        {
            private readonly StoreContext _context;
            private readonly IMediator _mediator;
            public Handler(StoreContext context, IMediator mediator)
            {

                _context = context;
                _mediator = mediator;

            }
            async public Task<Result> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
            {
                var updateRole = await _context.Roles.FirstOrDefaultAsync(u => u.Id == command.Id);

                var valildator = await Validator(updateRole, command, cancellationToken);
                if (valildator is not null)
                    return valildator;

                await UpdateRole(updateRole, command, cancellationToken);
                await _context.SaveChangesAsync();
                return Result.Success();
            }

            private async Task<Result> Validator(Role updateRole, UpdateRoleCommand command, CancellationToken cancellationToken)
            {
                bool roleIds = await _context.Roles
                    .AnyAsync(u => u.Id == command.Id, cancellationToken);

                if (!roleIds)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                if (updateRole == null)
                {
                    return Result.Failure(UserErrors.IdDoesNotExist());
                }
                var verification = command.Permission;
                var updatePermission = await _context.Roles.FirstOrDefaultAsync(x => x.Id == command.Id);
                var verify = updatePermission.Permission;

                if (verify.Count >= verification.Count)
                {
                    return Result.Failure(UserErrors.PermissionTagged());
                }
                if (verify.Count <= verification.Count)
                {
                    return Result.Failure(UserErrors.PermissionUntagged());
                }
                return null;
            }

            private async Task UpdateRole(Role updateRole, UpdateRoleCommand command, CancellationToken cancellationToken)
            {
                var updaterole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == command.Id);

                if (updaterole != null)
                {
                    updaterole.RoleName = command.RoleName;
                    updaterole.Permission = command.Permission;
                    updaterole.EditedBy = command.EditedBy;

                }
            }
        }
    }
}
