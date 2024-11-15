using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Threading;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.ResetPassword
{
    public class ResetPasswordHandler
    {
        public class ResetPasswordCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }



        public class Handler : IRequestHandler<ResetPasswordCommand, Result>
        {

            private readonly StoreContext _context;
            public Handler(StoreContext context)
            {

                _context = context;

            }
            public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await ResetPassword(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }

            public async Task<Result> Validator(ResetPasswordCommand command, CancellationToken cancellationToken)
            {
                bool userId = await _context.Users
                   .AnyAsync(u => u.Id == command.Id, cancellationToken);

                if (!userId)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                return null;
            }

            private async Task ResetPassword(ResetPasswordCommand command, CancellationToken cancellationToken)
            {
                var resetPassword = await _context.Users.FirstOrDefaultAsync(x => x.Id == command.Id);
                if (resetPassword != null)
                {
                    resetPassword.Password = BCrypt.Net.BCrypt.HashPassword(resetPassword.UserName);
                    resetPassword.UpdatePass = false;
                }

            }
        }
    }
}
