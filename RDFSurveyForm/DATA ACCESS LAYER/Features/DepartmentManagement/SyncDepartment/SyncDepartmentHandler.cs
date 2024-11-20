using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.DepartmentManagement.SyncDepartment
{
    public class SyncDepartmentHandler
    {
        public class SyncDepartmentCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string DepartmentName { get; set; }
            public DateTime CreatedAt { get; set; }
            public int? DepartmentNo { get; set; }
            public bool IsActive { get; set; }
            public string StatusSync { get; set; }
        }

        //public class Handler : IRequestHandler<SyncDepartmentCommand, Result>
        //{
        //    async public Task<Result> Handle(SyncDepartmentCommand command, CancellationToken cancellationToken)
        //    {
        //        var validator = await Validator(command, cancellationToken);
        //        if (validator is not null)
        //            return validator;

        //        await CreateUser(command, cancellationToken);

        //        await _context.SaveChangesAsync(cancellationToken);

        //        return Result.Success();
        //    }


        //}
    }
}
