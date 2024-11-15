using MediatR;
using RDFSurveyForm.Common;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.RoleManagement.AddRoles
{
    public partial class AddRoleHandler
    {
        public class AddRoleCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string RoleName { get; set; }

            public DateTime CreatedAt { get; set; }

            public ICollection<string> Permission { get; set; }
        }
    }
}
