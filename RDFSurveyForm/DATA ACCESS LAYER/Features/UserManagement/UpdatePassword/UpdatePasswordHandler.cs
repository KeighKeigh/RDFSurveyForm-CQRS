using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using RDFSurveyForm.Model;

namespace RDFSurveyForm.Handlers
{
    public partial class UpdatePasswordHandler
    {



        public class Handler : IRequestHandler<UpdatePasswordCommand, Result>
        {
            private readonly StoreContext _context;
            private readonly IMediator _mediator;
            public Handler(StoreContext context, IMediator mediator)
            {

                _context = context;
                _mediator = mediator;

            }
            public async Task<Result> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
            {

                var updatepassword = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.Id);

                var valildator = await Validator(updatepassword, command, cancellationToken);
                if (valildator is not null)
                    return valildator;

                await UpdatePassword(updatepassword, command, cancellationToken);

                await _context.SaveChangesAsync();
                return Result.Success();
            }


            private async Task<Result> Validator(User updatepassword, UpdatePasswordCommand command, CancellationToken cancellationToken)
            {
                bool userIds = await _context.Users
                    .AnyAsync(u => u.Id == command.Id, cancellationToken);

                if (!userIds)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                if (updatepassword == null)
                {
                    return Result.Failure(UserErrors.IdDoesNotExist());
                }
                if (!BCrypt.Net.BCrypt.Verify(command.Password, updatepassword.Password))
                {
                    return Result.Failure(UserErrors.WrongPassword());
                }
                if (BCrypt.Net.BCrypt.Verify(command.NewPassword, updatepassword.Password))
                {
                    return Result.Failure(UserErrors.PassSameAsOld());
                }

                if (command.NewPassword != command.ConfirmPassword)
                {
                    return Result.Failure(UserErrors.PassConfirmationError());

                }

                return null;
            }

            private async Task UpdatePassword(User updatepassword, UpdatePasswordCommand command, CancellationToken cancellationToken)
            {
                updatepassword.Password = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);
                updatepassword.UpdatePass = true;
            }
                

            
        }

        
    }
}
