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
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.GroupSurveyManagement.GetGroupSurveys.GetGroupSurvey;
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

        public async Task<IActionResult> AddGroupSurvey(AddGroupSurveyDto survey)
        {
            var scoreExceed = await _unitofWork.GroupSurvey.ScoreLimit(survey);
            if (scoreExceed == false)
            {
                return BadRequest("Follow the Limit!");
            }
            var groupexist = await _unitofWork.GroupSurvey.GroupIdDoesnotExist(survey.GroupsId);
            if (groupexist == false)
            {
                    return BadRequest("Group Id does not exist!");
            }

            var addgroupSurvey = await _unitofWork.GroupSurvey.AddSurvey(survey);
            if (addgroupSurvey == false)
            {
                    return BadRequest("Error!");
            }

            await _unitofWork.CompleteAsync();

            return Ok("Survey Added");
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
            var setisactive = await _unitofWork.GroupSurvey.SetIsActive(Id);
            if (setisactive == false)
            {
                return BadRequest("Id does not exist!");
            }
            return Ok("Updated");
        }


    }
}
