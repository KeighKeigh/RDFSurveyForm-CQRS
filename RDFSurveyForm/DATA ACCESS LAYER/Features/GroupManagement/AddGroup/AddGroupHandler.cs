using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using RDFSurveyForm.Model.Setup;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupManagement.AddGroup
{
    public class AddGroupHandler
    {

        public class AddGroupCommand : IRequest<Result>
        {
            public string GroupName { get; set; }
            public DateTime CreatedAt { get; set; }
            public string CreatedBy { get; set; }
            public int? BranchId { get; set; }
        }

        public class Handler : IRequestHandler<AddGroupCommand, Result>
        {
            private readonly StoreContext _context;
            public Handler(StoreContext context)
            {

                _context = context;

            }

            async public Task<Result> Handle(AddGroupCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await CreateGroup(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            private async Task<Result> Validator(AddGroupCommand command, CancellationToken cancellationToken)
            {
                bool groupExist = await _context.Groups.AnyAsync(g => g.GroupName == command.GroupName);
                if (groupExist)
                    return Result.Failure(UserErrors.GroupExist());

                return null;
            }
            private async Task CreateGroup(AddGroupCommand command, CancellationToken cancellationToken)
            {
                var addGroup = new Groups
                {
                    GroupName = command.GroupName,
                    BranchId = command.BranchId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = command.CreatedBy,
                };
                await _context.Groups.AddAsync(addGroup);
            }
        }
    }
}
