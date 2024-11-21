using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupSurveyManagement.InActiveGroupSurvey
{
    public class GroupSurveyActiveHandler
    {
        public class GroupSurveyActiveCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<GroupSurveyActiveCommand, Result>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            async public Task<Result> Handle(GroupSurveyActiveCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await GroupSurveyActivity(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            private async Task<Result> Validator(GroupSurveyActiveCommand command, CancellationToken cancellationToken)
            {
                bool categoryId = await _context.GroupSurvey
                    .AnyAsync(c => c.Id == command.Id);
                if (!categoryId)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                return null;
            }

            private async Task GroupSurveyActivity(GroupSurveyActiveCommand command, CancellationToken cancellationToken)
            {
                var setIsactive = await _context.GroupSurvey.FirstOrDefaultAsync(x => x.SurveyGeneratorId == command.Id);
                var setIsactivee = await _context.SurveyGenerator.FirstOrDefaultAsync(x => x.Id == command.Id);
                var setIsactiveee = await _context.SurveyScores.Where(x => x.SurveyGeneratorId == command.Id).ToListAsync();
                if (setIsactive != null)
                {
                    setIsactive.IsActive = !setIsactive.IsActive;
                    setIsactivee.IsActive = !setIsactivee.IsActive;
                    foreach (var item in setIsactiveee)
                    {
                        item.IsActive = !item.IsActive;
                    }
                }
            }
        }
    }
}
