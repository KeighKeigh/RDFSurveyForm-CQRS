using MediatR;
using Microsoft.AspNetCore.Mvc;
using RDFSurveyForm.Common;
using RDFSurveyForm.Common.EXTENSIONS;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Dto.ModelDto.RoleDto;
using RDFSurveyForm.Model;
using RDFSurveyForm.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.RoleManagement.AddRoles.AddRoleHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.RoleManagement.GetRoles.GetRole;
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



       

        [HttpPatch("SetIsActive/{Id:int}")]
        public async Task<IActionResult> SetIsActive([FromRoute] int Id)
        {
            try
            {
                var command = new RoleActiveCommand
                {
                    Id = Id
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
        public async Task<IActionResult> CustomerListPagnation([FromQuery] GetRoleQuery query )
        {
            try
            {
                var users = await _mediator.Send(query);

                Response.AddPaginationHeader(

                   users.CurrentPage,
                   users.PageSize,
                   users.TotalCount,
                   users.TotalPages,
                   users.HasNextPage,
                   users.HasPreviousPage

                    );

                var results = new
                {
                    users,
                    users.PageSize,
                    users.TotalCount,
                    users.TotalPages,
                    users.HasNextPage,
                    users.HasPreviousPage
                };

                var successResult = Result.Success(results);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
