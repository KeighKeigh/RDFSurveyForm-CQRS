using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Dto.ModelDto.UserDto;
using RDFSurveyForm.Handlers.Errors.UserError;
using RDFSurveyForm.Model;
using RDFSurveyForm.Services;
using System;
using System.Reflection.Metadata.Ecma335;

namespace RDFSurveyForm.Handlers.Errors.Features.UserManagement
{
    public partial class AddUserHandler
    {

        public class Handler : IRequestHandler<AddUserCommand, Result>
        {
            private readonly StoreContext _context;
            public Handler(StoreContext context)
            {

                _context = context;

            }

            public async Task<Result> Handle(AddUserCommand command, CancellationToken cancellationToken)
            {

                var validator = await Validator(command, cancellationToken);
                if (validator is not null)
                    return validator;

                await CreateUser(command, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();

            }

            private async Task<Result> Validator(AddUserCommand command, CancellationToken cancellationToken)
            {
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
            private async Task CreateUser(AddUserCommand command, CancellationToken cancellationToken)
            {
                var adduser = new User
                {
                    FullName = command.FullName,
                    UserName = command.UserName,
                    Password = BCrypt.Net.BCrypt.HashPassword(command.UserName),
                    GroupsId = command.GroupsId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = command.CreatedBy,
                    RoleId = command.RoleId,
                    DepartmentId = command.DepartmentId,

                };

                await _context.Users.AddAsync(adduser);

            }
                
  
        }




    }
}
