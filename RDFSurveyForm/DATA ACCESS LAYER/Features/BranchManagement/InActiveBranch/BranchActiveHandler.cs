using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.BranchManagement.InActiveBranch
{
    public class BranchActiveHandler
    {
        public class BranchActiveCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<BranchActiveCommand, Result>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            async public Task<Result> Handle(BranchActiveCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await BranchActivity(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            private async Task<Result> Validator(BranchActiveCommand command, CancellationToken cancellationToken)
            {
                bool branchId = await _context.Branches
                    .AnyAsync(u => u.Id == command.Id);

                if (branchId)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                return null;
            }

            private async Task BranchActivity(BranchActiveCommand command, CancellationToken cancellationToken)
            {
                var setIsactive = await _context.Branches
                    .FirstOrDefaultAsync(x => x.Id == command.Id);

                if (setIsactive != null)
                {
                    setIsactive.IsActive = !setIsactive.IsActive;
                }
            }
        }
    }
}
