using MediatR;
using Microsoft.AspNetCore.Mvc;
using RDFSurveyForm.Common.EXTENSIONS;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Dto.ModelDto.RoleDto;
using RDFSurveyForm.Model;
using RDFSurveyForm.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.RoleManagement.AddRoles.AddRoleHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.RoleManagement.InActiveRoles.RoleActiveHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.RoleManagement.UpdateRoles.UpdateRoleHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.UserActive.UserActiveHandler;

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

        [HttpPut("UpdateRole")]

        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleCommand role)
        {
            try
            {
                var result = await _mediator.Send(role);

                if (result.IsFailure)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }



       

        [HttpPatch("SetIsActive/{id:int}")]
        public async Task<IActionResult> SetIsActive([FromRoute] int id)
        {
            try
            {
                var command = new RoleActiveCommand
                {
                    Id = id
                };

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
