using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupManagement.DeleteGroup
{
    public class DeleteGroupHandler
    {

        public class DeleteGroupCommand : IRequest<Result>
        {
           public int Id { get; set; }
        }

        public class Handler : IRequestHandler<DeleteGroupCommand, Result>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            async public Task<Result> Handle(DeleteGroupCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await GroupActivity(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            public async Task<Result> Validator(DeleteGroupCommand command, CancellationToken cancellationToken)
            {
                bool groupId = await _context.Groups
                    .AnyAsync(c => c.Id == command.Id);
                if (!groupId)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                return null;
            }

            public async Task GroupActivity(DeleteGroupCommand command, CancellationToken cancellationToken)
            {
                var deleteGroup = await _context.Groups.FirstOrDefaultAsync(x => x.Id == command.Id);
                if (deleteGroup != null)
                {
                    _context.Remove(deleteGroup);
                }
            }

        }
    }
}
