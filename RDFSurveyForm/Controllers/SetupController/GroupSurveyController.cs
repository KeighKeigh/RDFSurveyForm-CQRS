using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Common.EXTENSIONS;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Dto.ModelDto.UserDto;
using RDFSurveyForm.Dto.SetupDto.GroupSurveyDto;
using RDFSurveyForm.Model;
using RDFSurveyForm.Model.Setup;
using RDFSurveyForm.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupManagement.InActiveGroup.GroupActiveHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupSurveyManagement.AddGroupSurvey.AddGroupSurveyHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupSurveyManagement.GetGroupSurveys.GetGroupSurvey;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupSurveyManagement.InActiveGroupSurvey.GroupSurveyActiveHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.GetUsers.GetUser;

namespace RDFSurveyForm.Controllers.SetupController
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupSurveyController : ControllerBase
    {

        public readonly IUnitOfWork _unitofWork;
        public readonly StoreContext _context;
        public readonly IMediator _mediator;

        public GroupSurveyController(IUnitOfWork unitOfWork, StoreContext context, IMediator mediator)
        {
            _context = context;
            _unitofWork = unitOfWork;
            _mediator = mediator;
        }

        [HttpPost("AddGroupSurvey")]

        public async Task<IActionResult> AddGroupSurvey(AddGroupSurveyCommand command)
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

        [HttpGet("GetGroupSurvey")]
        public async Task<IActionResult> GetGroupSurvey([FromQuery] GetGroupSurveyQuery query)
        {
            try
            {
                var survey = await _mediator.Send(query);

                Response.AddPaginationHeader(

                   survey.CurrentPage,
                   survey.PageSize,
                   survey.TotalCount,
                   survey.TotalPages,
                   survey.HasNextPage,
                   survey.HasPreviousPage

                    );

                var results = new
                {
                    survey,
                    survey.PageSize,
                    survey.TotalCount,
                    survey.TotalPages,
                    survey.HasNextPage,
                    survey.HasPreviousPage
                };

                var successResult = Result.Success(results);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpPatch("ViewSurveyGenerator/{Id:int}")]
        public async Task<IActionResult> ViewSurveyGenerator([FromRoute] int Id)
        {
            var users = await _unitofWork.GroupSurvey.ViewSurvey(Id);

            return Ok(users);
        }

        [HttpPut("UpdateScore")]
        public async Task<IActionResult> UpdateScore([FromBody] UpdateSurveyScoreDto score)
        {
 
            var surveyscore = await _context.GroupSurvey.FirstOrDefaultAsync(x => x.GroupsId == score.GroupsId);
            if (surveyscore == null)
            {
                return BadRequest("ID does not exist!");
            }
            var scores = await _unitofWork.GroupSurvey.UpdateScore(score);
            if (scores == false)
            {
                return BadRequest("Error!");
            }

            return Ok("Score Approved!");

        }

        [HttpPatch("SetIsActive/{Id:int}")]
        public async Task<IActionResult> SetIsActive([FromRoute] int Id)
        {
            try
            {
                var command = new GroupSurveyActiveCommand
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
