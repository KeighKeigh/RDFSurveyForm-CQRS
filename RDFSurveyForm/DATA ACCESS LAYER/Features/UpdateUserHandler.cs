using MediatR;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Data;
using RDFSurveyForm.Model;
using static RDFSurveyForm.Handlers.Errors.Features.UserManagement.AddUserHandler;

namespace RDFSurveyForm.Handlers
{
    public class UpdateUserHandler
    {
        public class UpdateUserCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string UserName { get; set; }
            public int? RoleId { get; set; }
            public int? GroupsId { get; set; }
            public string EditedBy { get; set; }
            public int? DepartmentId { get; set; }
        }

        public class Handler : IRequestHandler<UpdateUserCommand, Result>
        {
            private readonly StoreContext _context;
            private readonly IMediator _mediator;
            public Handler(StoreContext context, IMediator mediator)
            {

                _context = context;
                _mediator = mediator;

            }

             async Task<Result> IRequestHandler<UpdateUserCommand, Result>.Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                //await _mediator.Send(new FullNameEmptyCommand(request.FullName), cancellationToken);
                //await _mediator.Send(new UserNameEmptyCommand(request.UserName), cancellationToken);
                //await _mediator.Send(new FullNameExceptionCommand(request.FullName), cancellationToken);
                //await _mediator.Send(new UserNameExceptionCommand(request.UserName), cancellationToken);

                var updateuser = await _context.Users.FirstOrDefaultAsync(info => info.Id == request.Id);
                if (updateuser != null)
                {
                    updateuser.UserName = request.UserName;
                    updateuser.FullName = request.FullName;
                    updateuser.RoleId = request.RoleId;
                    updateuser.GroupsId = request.GroupsId;
                    updateuser.EditedBy = request.EditedBy;
                    updateuser.DepartmentId = request.DepartmentId;


                    await _context.SaveChangesAsync();                    
                }
                return Result.Success();
                
            }
        }
    }
}
