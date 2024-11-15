using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Handlers.Errors.UserError;
using RDFSurveyForm.Model;
using System.Runtime.InteropServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static RDFSurveyForm.Handlers.Errors.Features.UserManagement.AddUserHandler;

namespace RDFSurveyForm.Handlers
{
    public partial class UpdateUserHandler
    {

        public class Handler : IRequestHandler<UpdateUserCommand, Result>
        {
            private readonly StoreContext _context;
            private readonly IMediator _mediator;
            public Handler(StoreContext context, IMediator mediator)
            {

                _context = context;
                _mediator = mediator;

            }

            public async Task<Result> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
            {
                var validator = await Validator(command,cancellationToken);
                if (validator is not null) 
                    return validator;

                await UpdateUser(command, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            public async Task<Result?> Validator(UpdateUserCommand command, CancellationToken cancellationToken)
            {
                bool userIds = await _context.Users
                    .AnyAsync(u => u.Id == command.Id, cancellationToken);

                if (!userIds)
                    return Result.Failure(UserErrors.IdDoesNotExist());

                if (string.IsNullOrEmpty(command.FullName))
                    return Result.Failure(UserErrors.EmptyFullName());

                if (string.IsNullOrEmpty(command.UserName))
                    return Result.Failure(UserErrors.EmptyUserName());


                bool fullnameExist = await _context.Users
                    .AnyAsync(u => u.FullName == command.FullName, cancellationToken);

                if (fullnameExist)
                    return Result.Failure(UserErrors.NameExist());

                bool usernameExist = await _context.Users
                    .AnyAsync(u => u.UserName == command.UserName, cancellationToken);

                if (usernameExist)
                    return Result.Failure(UserErrors.UserNameExist());

                

                return null;
            }

            public async Task UpdateUser(UpdateUserCommand command, CancellationToken cancellationToken)
            {
                var updateuser = await _context.Users.FirstOrDefaultAsync(info => info.Id == command.Id);
                if (updateuser != null)
                {
                    updateuser.UserName = command.UserName;
                    updateuser.FullName = command.FullName;
                    updateuser.RoleId = command.RoleId;
                    updateuser.GroupsId = command.GroupsId;
                    updateuser.EditedBy = command.EditedBy;
                    updateuser.DepartmentId = command.DepartmentId;                   
                }
                
            }

             
        }
    }
}
