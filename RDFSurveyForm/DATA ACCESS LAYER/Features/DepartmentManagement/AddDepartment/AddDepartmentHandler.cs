using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using RDFSurveyForm.Model;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.DepartmentManagement.AddDepartment
{
    public partial class AddDepartmentHandler
    {

        public class Handler : IRequestHandler<AddDepartmentCommand, Result>
        {
            private readonly StoreContext _context;
            public Handler(StoreContext context)
            {
                _context = context;
            }
            async public Task<Result> Handle(AddDepartmentCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await CreateDepartment(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            public async Task<Result> Validator(AddDepartmentCommand command, CancellationToken cancellationToken)
            {
                bool usernameExist = await _context.Department
                    .AnyAsync(u => u.DepartmentName == command.DepartmentName, cancellationToken);

                if (usernameExist)
                    return Result.Failure(UserErrors.DepartmentExist());

                return null;
            }

            public async Task CreateDepartment(AddDepartmentCommand command, CancellationToken cancellationToken)
            {
                var AddDept = new Department
                {
                    DepartmentName = command.DepartmentName,
                    CreatedAt = DateTime.Now,
                    DepartmentNo = command.DepartmentNo,
                    StatusSync = "New Added"
                };

                await _context.Department.AddAsync(AddDept);
            }
        }
    }
}
