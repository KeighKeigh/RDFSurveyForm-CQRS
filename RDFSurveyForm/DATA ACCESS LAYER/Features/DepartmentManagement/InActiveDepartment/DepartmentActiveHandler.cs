using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.DepartmentManagement.InActiveDepartment
{
    public class DepartmentActiveHandler
    {
        public class DepartmentActiveCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<DepartmentActiveCommand, Result>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(DepartmentActiveCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await DepartmentActivity(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            public async Task<Result> Validator(DepartmentActiveCommand command, CancellationToken cancellationToken)
            {
                bool departId = await _context.Department
                    .AnyAsync(u => u.Id == command.Id);

                if (departId)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                return null;
            }

            private async Task DepartmentActivity(DepartmentActiveCommand command, CancellationToken cancellationToken)
            {
                var setIsactive = await _context.Department.FirstOrDefaultAsync(x => x.Id == command.Id);
                if (setIsactive != null)
                {
                    setIsactive.IsActive = !setIsactive.IsActive;
                }

            }
        }
    }
}
