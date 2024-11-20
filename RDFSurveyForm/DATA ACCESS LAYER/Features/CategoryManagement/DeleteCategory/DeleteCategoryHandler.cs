using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.CategoryManagement.DeleteCategory
{
    public class DeleteCategoryHandler
    {
        public class DeleteCategoryCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<DeleteCategoryCommand, Result>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            async public Task<Result> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await CategoryActivity(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            private async Task<Result> Validator(DeleteCategoryCommand command, CancellationToken cancellationToken)
            {
                bool categoryId = await _context.Category
                    .AnyAsync(c => c.Id == command.Id);
                if (!categoryId)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                return null;
            }

            private async Task CategoryActivity(DeleteCategoryCommand command, CancellationToken cancellationToken)
            {
                var deleteCategory = await _context.Category.FirstOrDefaultAsync(x => x.Id == command.Id);
                if (deleteCategory != null)
                {
                    _context.Remove(deleteCategory);
                }
            }
        }
    }
}
