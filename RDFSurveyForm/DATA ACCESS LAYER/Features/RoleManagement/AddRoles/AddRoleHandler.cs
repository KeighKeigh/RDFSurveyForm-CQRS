using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using RDFSurveyForm.Model;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.RoleManagement.AddRoles
{
    public partial class AddRoleHandler
    {

        public class Handler : IRequestHandler<AddRoleCommand, Result>
        {
            private readonly StoreContext _context;
            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddRoleCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await AddRole(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            public async Task<Result> Validator(AddRoleCommand command, CancellationToken cancellationToken)
            {
                bool userIds = await _context.Users
                    .AnyAsync(u => u.Id == command.Id, cancellationToken);

                if (!userIds)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                bool roleExist = await _context.Roles.AnyAsync(r => r.RoleName == command.RoleName, cancellationToken);

                if (roleExist)
                    return Result.Failure(UserErrors.RoleNameExist());

                return null;
            }

            public async Task AddRole(AddRoleCommand command, CancellationToken cancellationToken)
            {
                var addrole = new Role
                {
                    RoleName = command.RoleName,
                    CreatedAt = DateTime.Now,
                    Permission = command.Permission,
                };
                await _context.Roles.AddAsync(addrole);
            }
        }
    }
}
