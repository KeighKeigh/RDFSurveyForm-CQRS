using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupManagement.UpdateGroup
{
    public class UpdateGroupHandler
    {
        public class UpdateGroupCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string GroupName { get; set; }
            public DateTime UpdatedAt { get; set; }
            public string UpdatedBy { get; set; }
            public int? BranchId { get; set; }
        }

        public class Handler : IRequestHandler<UpdateGroupCommand, Result>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {

                _context = context;

            }
            async public Task<Result> Handle(UpdateGroupCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await UpdateGroup(command, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            private async Task<Result> Validator(UpdateGroupCommand command, CancellationToken cancellationToken)
            {
                bool groupExist = await _context.Groups
                    .AnyAsync(g => g.GroupName == command.GroupName);
                if (groupExist)
                    return Result.Failure(UserErrors.GroupExist());

                bool groupId = await _context.Groups
                    .AnyAsync(g => g.Id == command.Id);
                if (!groupId)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                return null;
            }

            private async Task UpdateGroup(UpdateGroupCommand command, CancellationToken cancellationToken)
            {
                var updateGroup = await _context.Groups.FirstOrDefaultAsync(x => x.Id == command.Id);
                if (updateGroup != null)
                {
                    updateGroup.GroupName = command.GroupName;
                    updateGroup.UpdatedAt = DateTime.Now;
                    updateGroup.UpdatedBy = command.UpdatedBy;
                    updateGroup.BranchId = command.BranchId;
                }
            }
        }
    }
}
