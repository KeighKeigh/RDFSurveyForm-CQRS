using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using System.ComponentModel.Design;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.UserActive
{
    public class UserActiveHandler
    {
        public class UserActiveCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<UserActiveCommand, Result>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UserActiveCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await UserActivity(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            public async Task<Result> Validator(UserActiveCommand command, CancellationToken cancellationToken)
            {
                bool userId = await _context.Users
                    .AnyAsync(u => u.Id == command.Id);

                if (userId)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                return null;
            }

            private async Task UserActivity(UserActiveCommand command, CancellationToken cancellationToken)
            {
                var setIsactive = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.Id);
                if (setIsactive != null)
                {
                    setIsactive.IsActive = !setIsactive.IsActive;             
                }

            }
        }
    }
}
