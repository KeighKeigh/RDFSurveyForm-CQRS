using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.RoleManagement.InActiveRoles
{
    public class RoleActiveHandler
    {
        public class RoleActiveCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<RoleActiveCommand, Result>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(RoleActiveCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await UserActivity(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            public async Task<Result> Validator(RoleActiveCommand command, CancellationToken cancellationToken)
            {
                bool roleId = await _context.Roles
                    .AnyAsync(u => u.Id == command.Id);

                if (!roleId)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                return null;
            }

            private async Task UserActivity(RoleActiveCommand command, CancellationToken cancellationToken)
            {
                var setIsactive = await _context.Roles.FirstOrDefaultAsync(x => x.Id == command.Id);
                if (setIsactive != null)
                {
                    setIsactive.IsActive = !setIsactive.IsActive;
                }

            }
        }
    }
}
