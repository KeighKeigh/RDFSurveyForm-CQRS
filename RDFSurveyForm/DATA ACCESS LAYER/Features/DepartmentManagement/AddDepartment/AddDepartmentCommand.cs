using MediatR;
using RDFSurveyForm.Common;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.DepartmentManagement.AddDepartment
{
    public partial class AddDepartmentHandler
    {
        public class AddDepartmentCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string DepartmentName { get; set; }
            public DateTime CreatedAt { get; set; }
            public int? DepartmentNo { get; set; }
            public bool IsActive { get; set; }
            public string StatusSync { get; set; }
        }
    }
}
