using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.DepartmentManagement.UpdateDepartment
{
    public class UpdateDepartmentHandler
    {
        public class UpdateDepartmentCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string DepartmentName { get; set; }
            public int? DepartmentNo { get; set; }
            public string EditedBy { get; set; }
            public DateTime EditedAt { get; set; }
        }

        public class Handler : IRequestHandler<UpdateDepartmentCommand, Result>
        {
            private readonly StoreContext _context;
            public Handler(StoreContext context)
            {

                _context = context;

            }

            async public Task<Result> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await UpdateDepartment(command, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            public async Task <Result> Validator(UpdateDepartmentCommand command, CancellationToken cancellationToken)
            {
                bool deptname = await _context.Department
                    .AnyAsync(u => u.DepartmentName == command.DepartmentName, cancellationToken);

                if (deptname)
                    return Result.Failure(UserErrors.DepartmentExist());

                bool deptId = await _context.Department
                    .AnyAsync(d => d.Id == command.Id, cancellationToken);
                if (!deptId)
                    return Result.Failure(UserErrors.IdDoesNotExist());


                return null;
            }

            public async Task UpdateDepartment(UpdateDepartmentCommand command, CancellationToken cancellationToken)
            {
                var updatedept = await _context.Department.FirstOrDefaultAsync(info =>  info.Id == command.Id);
                if (updatedept != null)
                {
                    updatedept.DepartmentName = command.DepartmentName;
                    updatedept.DepartmentNo = command.DepartmentNo;
                    updatedept.EditedBy = command.EditedBy;
                    updatedept.EditedAt = command.EditedAt;
                }
            }
        }
    }
}
