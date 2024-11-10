using MediatR;
using RDFSurveyForm.Common;

namespace RDFSurveyForm.Handlers.Errors.Features.UserManagement
{
    public partial class AddUserHandler
    {
        public class AddUserCommand : IRequest<Result>
        {
            public string FullName { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public int? GroupsId { get; set; }
            public string CreatedBy { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public int? RoleId { get; set; }
            public int? DepartmentId { get; set; }


        }




    }
}
