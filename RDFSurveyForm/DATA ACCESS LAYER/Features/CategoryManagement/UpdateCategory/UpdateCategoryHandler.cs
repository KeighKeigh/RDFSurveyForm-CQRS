using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.CategoryManagement.UpdateCategory
{
    public class UpdateCategoryHandler
    {
        public class UpdateCategoryCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string CategoryName { get; set; }
            public decimal CategoryPercentage { get; set; }
            public decimal Limit { get; set; } = 100;
            public DateTime UpdatedAt { get; set; }
            public string UpdatedBy { get; set; }
        }

        public class Handler : IRequestHandler<UpdateCategoryCommand, Result>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {

                _context = context;

            }

            async public Task<Result> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await UpdateCategory(command, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            private async Task<Result> Validator(UpdateCategoryCommand command, CancellationToken cancellationToken)
            {
                bool categoryId = await _context.Category
                    .AnyAsync(c => c.Id == command.Id, cancellationToken);
                if (!categoryId)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                var users = await _context.Category.Where(x => x.IsActive && x.Id != command.Id).ToListAsync();
                var percentage = command.CategoryPercentage * .01M;
                var result = users.Sum(x => x.CategoryPercentage);
                var total = result + percentage;
                if (total > 1)
                {
                    return Result.Failure(UserErrors.PercentageExceed());
                }

                bool categoryExist = await _context.Category
                    .AnyAsync(c => c.CategoryName == command.CategoryName);
                if (categoryExist)
                    return Result.Failure(UserErrors.CategoryExist());

                return null;
            }

            private async Task UpdateCategory(UpdateCategoryCommand command, CancellationToken cancellationToken)
            {
                var updatecategory = await _context.Category.FirstOrDefaultAsync(info => info.Id == command.Id);
                if (updatecategory != null)
                {
                    updatecategory.CategoryName = command.CategoryName;
                    updatecategory.CategoryPercentage = command.CategoryPercentage / 100;
                    updatecategory.UpdatedAt = command.UpdatedAt;
                    updatecategory.UpdatedBy = command.UpdatedBy;
                }
            }
        }

    }
}
