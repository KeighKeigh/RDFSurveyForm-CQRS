using MediatR;
using Microsoft.AspNetCore.Mvc;
using RDFSurveyForm.Common.EXTENSIONS;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Dto.ModelDto.RoleDto;
using RDFSurveyForm.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.RoleManagement.AddRoles.AddRoleHandler;

namespace RDFSurveyForm.Controllers.ModelController
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;
        private readonly IMediator _mediator;

        public RoleController(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            
        }
        [HttpPost]
        [Route("AddNewRole")]
        public async Task<IActionResult> AddNewRole(AddRoleCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("UpdateRole/{Id:int}")]

        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto role, [FromRoute] int Id)
        {
            role.Id = Id;
            var verify = await _unitOfWork.CRole.UpdatedPermission(role);
            var roles = await _unitOfWork.CRole.UpdateRole(role);
            if (roles == false)
            {
                return BadRequest("User does not exist");
            }


            if (roles == true && verify == false)
            {
                return Ok("Tagged a Permission");
            }
            if (roles == true && verify == true)
            {
                return Ok("Untagged a Permission");
            }
            return Ok("ok");
        }



       

        [HttpPatch("SetIsActive/{Id:int}")]
        public async Task<IActionResult> SetIsActive([FromRoute] int Id)
        {
            var isactiveValidation = await _unitOfWork.CRole.IsActiveValidation(Id);
            if (isactiveValidation == true)
            {
                return BadRequest("Cannot Deactivate Role");
            }
            var setisactive = await _unitOfWork.CRole.SetIsActive(Id);
            if (setisactive == null)
            {
                return BadRequest("Id does not exist");

            }


            return Ok("Updated");

        }

        [HttpGet]
        [Route("RoleListPagination")]
        public async Task<ActionResult<IEnumerable<GetRoleDto>>> CustomerListPagnation([FromQuery] UserParams userParams, bool? status, string search)
        {
            var rolesummary = await _unitOfWork.CRole.CustomerListPagnation(userParams, status, search);

            Response.AddPaginationHeader(rolesummary.CurrentPage, rolesummary.PageSize, rolesummary.TotalCount, rolesummary.TotalPages, rolesummary.HasNextPage, rolesummary.HasPreviousPage);

            var rolesummaryResult = new
            {
                rolesummary,
                rolesummary.CurrentPage,
                rolesummary.PageSize,
                rolesummary.TotalCount,
                rolesummary.TotalPages,
                rolesummary.HasNextPage,
                rolesummary.HasPreviousPage
            };

            return Ok(rolesummaryResult);
        }
    }
}
