using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Common.EXTENSIONS;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Dto.SetupDto.GroupDto;
using RDFSurveyForm.Model;
using RDFSurveyForm.Services;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.CategoryManagement.DeleteCategory.DeleteCategoryHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupManagement.AddGroup.AddGroupHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupManagement.DeleteGroup.DeleteGroupHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupManagement.GetGroup.GetGroup;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupManagement.InActiveGroup.GroupActiveHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupManagement.UpdateGroup.UpdateGroupHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.UserActive.UserActiveHandler;

namespace RDFSurveyForm.Controllers.SetupController
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;
        private readonly IMediator _mediator;

        public GroupController(IUnitOfWork unitOfWork, StoreContext context, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mediator = mediator;
        }

        [HttpPost("AddGroup")]
        public async Task<IActionResult> AddGroup(AddGroupCommand command)
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

        [HttpPut("UpdateGroup/{Id:int}")]
        public async Task<IActionResult> UpdateGroup([FromBody]UpdateGroupCommand command,[FromRoute] int Id)
        {
            command.Id = Id;
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

        [HttpGet("GroupListPagination")]
        public async Task<IActionResult> GroupListPagnation([FromQuery] GetGroupQuery query)
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

        [HttpPatch("Setisactive/{Id:int}")]
        public async Task<IActionResult> SetIsactive([FromRoute] int Id)
        {
            try
            {
                var command = new GroupActiveCommand
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

        [HttpDelete("DeleteGroup/{Id:int}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int Id)
        {
            try
            {
                var command = new DeleteGroupCommand
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
    }

}
