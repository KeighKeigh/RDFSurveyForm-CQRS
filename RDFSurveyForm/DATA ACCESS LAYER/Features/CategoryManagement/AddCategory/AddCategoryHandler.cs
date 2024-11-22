using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using RDFSurveyForm.Model.Setup;
using static RDFSurveyForm.Dto.SetupDto.GroupSurveyDto.ViewSurveyDto;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.CategoryManagement.AddCategory
{
    public class AddCategoryHandler
    {

        public class AddCategoryCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string CategoryName { get; set; }
            public decimal CategoryPercentage { get; set; }
            public decimal Limit { get; set; }
            public DateTime CreatedAt { get; set; }
            public string CreatedBy { get; set; }
        }

        public class Handler : IRequestHandler<AddCategoryCommand, Result>
        {
            private readonly StoreContext _context;
            public Handler(StoreContext context)
            {

                _context = context;

            }

            async public Task<Result> Handle(AddCategoryCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await CreateCatergory(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            private async Task<Result> Validator(AddCategoryCommand command, CancellationToken cancellationToken)
            {
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

            private async Task CreateCatergory(AddCategoryCommand command, CancellationToken cancellationToken)
            {
                var addCategory = new Category
                {
                    CategoryName = command.CategoryName,
                    CategoryPercentage = command.CategoryPercentage * .01M,
                    CreatedAt = DateTime.Now,
                    CreatedBy = command.CreatedBy,
                    Limit = 100,

                };
                await _context.Category.AddAsync(addCategory);

            }
        }
    }
}
