using MediatR;
using RDFSurveyForm.Common;

namespace RDFSurveyForm.Handlers
{
    public partial class UpdateUserHandler
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
    }
}
