using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.CategoryManagement.InActiveCategory
{
    public class CategoryActiveHandler
    {
        public class CategoryActiveCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<CategoryActiveCommand, Result>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            async public Task<Result> Handle(CategoryActiveCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await CategoryActivity(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            private async Task<Result> Validator(CategoryActiveCommand command, CancellationToken cancellationToken)
            {
                bool categoryId = await _context.Category
                    .AnyAsync(c=> c.Id == command.Id);
                if (!categoryId)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                return null;
            }

            private async Task CategoryActivity(CategoryActiveCommand command, CancellationToken cancellationToken)
            {
                var setIsactive = await _context.Category.FirstOrDefaultAsync(x => x.Id == command.Id);
                if (setIsactive != null)
                {
                    setIsactive.IsActive = !setIsactive.IsActive;
                }
            }
        }
    }
}
