using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using RDFSurveyForm.Model.Setup;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.BranchManagement.AddBranch
{
    public class AddBranchHandler
    {
        public class AddBranchCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string BranchName { get; set; }
            public string BranchCode { get; set; }
            public DateTime CreatedAt { get; set; }
            public string CreatedBy { get; set; }
        }

        public class Handler : IRequestHandler<AddBranchCommand, Result>
        {
            private readonly StoreContext _context;
            public Handler(StoreContext context)
            {

                _context = context;

            }

            async public Task<Result> Handle(AddBranchCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await CreateBranch(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            private async Task<Result> Validator(AddBranchCommand command, CancellationToken cancellationToken)
            {
                bool branchExist = await _context.Branches
                    .AnyAsync(b => b.BranchName == command.BranchName, cancellationToken);
                if(branchExist)                
                    return Result.Failure(UserErrors.BranchExist());


                bool codeExist = await _context.Branches
                    .AnyAsync(b => b.BranchCode == command.BranchCode, cancellationToken);

                if (codeExist)
                    return Result.Failure(UserErrors.BranchCodeExist());


                return null;
            }

            private async Task CreateBranch(AddBranchCommand command, CancellationToken cancellationToken)
            {
                var addBranch = new Branch
                {
                    Id = command.Id,
                    BranchName = command.BranchName,
                    BranchCode = command.BranchCode,
                    CreatedAt = DateTime.Now,
                    CreatedBy = command.CreatedBy,
                };
                await _context.Branches.AddAsync(addBranch);

            }
        }
    }
}
