using MediatR;
using RDFSurveyForm.Common;
using RDFSurveyForm.Common.HELPERS;

namespace RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.GetUsers
{
    public partial class GetUser
    {
        public class GetUserQuery : UserParams, IRequest<PagedList<GetUserResult>>
        {
            public string Search { get; set; }
            public bool? Is_Archive { get; set; }

        }
    }
}
