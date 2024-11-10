using MediatR;
using RDFSurveyForm.Common;

namespace RDFSurveyForm.Handlers
{
    public partial class UpdatePasswordHandler
    {
        public class UpdatePasswordCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string Password { get; set; }
            public string NewPassword { get; set; }
            public string ConfirmPassword { get; set; }

            
        }

        
    }
}
