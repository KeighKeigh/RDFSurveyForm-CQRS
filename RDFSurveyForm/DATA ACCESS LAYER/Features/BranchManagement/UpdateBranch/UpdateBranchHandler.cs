using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.BranchManagement.UpdateBranch
{
    public class UpdateBranchHandler
    {
        public class UpdateBranchCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string BranchName { get; set; }
            public string BranchCode { get; set; }
            public DateTime UpdatedAt { get; set; }
            public string UpdatedBy { get; set; }
        }

        public class Handler : IRequestHandler<UpdateBranchCommand, Result>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            async public Task<Result> Handle(UpdateBranchCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await UpdateBranch(command, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            public async Task<Result> Validator(UpdateBranchCommand command, CancellationToken cancellationToken)
            {
                bool invalidId = await _context.Branches
                    .AnyAsync(b => b.Id == command.Id, cancellationToken);
                if (!invalidId)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                bool branchExist = await _context.Branches
                    .AnyAsync(b => b.BranchName == command.BranchName, cancellationToken);
                if (branchExist)
                    return Result.Failure(UserErrors.BranchExist());


                bool codeExist = await _context.Branches
                    .AnyAsync(b => b.BranchCode == command.BranchCode, cancellationToken);

                if (codeExist)
                    return Result.Failure(UserErrors.BranchCodeExist());


                return null;
            }

            public async Task UpdateBranch(UpdateBranchCommand command, CancellationToken cancellationToken)
            {
                var updateBranch = await _context.Branches.FirstOrDefaultAsync(info => info.Id == command.Id);
                if (updateBranch != null)
                {
                    updateBranch.BranchName = command.BranchName;
                    updateBranch.BranchCode = command.BranchCode;
                    updateBranch.UpdatedAt = command.UpdatedAt;
                    updateBranch.UpdatedBy = command.UpdatedBy;
                }
            }
        }
    }
}
