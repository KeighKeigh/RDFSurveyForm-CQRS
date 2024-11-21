using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupSurveyManagement.UpdateGroupSurvey
{
    public class UpdateGroupSurveyHandler
    {
        public class UpdateGroupSurveyCommand : IRequest<Result>
        {
            public int? GroupsId { get; set; }
            public string UpdatedBy { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public class Handler : IRequestHandler<UpdateGroupSurveyCommand, Result>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {

                _context = context;

            }

            async public Task<Result> Handle(UpdateGroupSurveyCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await UpdateGroupSurvey(command, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            private async Task<Result> Validator(UpdateGroupSurveyCommand command, CancellationToken cancellationToken)
            {
                bool NoExistId = await _context.GroupSurvey.AnyAsync(x => x.GroupsId == command.GroupsId);
                if (!NoExistId)
                    return Result.Failure(UserErrors.NoGroupId());

                return null;
            }

            private async Task UpdateGroupSurvey(UpdateGroupSurveyCommand command, CancellationToken cancellationToken)
            {
                var updateScore = await _context.GroupSurvey
                    .Where(x => x.GroupsId == command.GroupsId
                     && x.IsActive == true).ToListAsync();
                foreach (var items in updateScore)
                {



                    items.UpdatedBy = command.UpdatedBy;
                    items.UpdatedAt = command.UpdatedAt;
                    items.IsTransacted = true;

                }
            }
        }
    }
}
