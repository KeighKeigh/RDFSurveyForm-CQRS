using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupManagement.InActiveGroup
{
    public class GroupActiveHandler
    {
        public class GroupActiveCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<GroupActiveCommand, Result>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            async public Task<Result> Handle(GroupActiveCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await GroupActivity(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            private async Task<Result> Validator(GroupActiveCommand command, CancellationToken cancellationToken)
            {
                bool groupId = await _context.Groups
                    .AnyAsync(c => c.Id == command.Id);
                if (!groupId)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                return null;
            }

            private async Task GroupActivity(GroupActiveCommand command, CancellationToken cancellationToken)
            {
                var setIsactive = await _context.Groups.FirstOrDefaultAsync(x => x.Id == command.Id);
                if (setIsactive != null)
                {
                    setIsactive.IsActive = !setIsactive.IsActive;
                }
            }
        }
    }
}
