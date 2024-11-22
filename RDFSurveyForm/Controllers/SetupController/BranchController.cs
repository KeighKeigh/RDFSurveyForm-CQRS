using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Common.EXTENSIONS;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Dto.ModelDto.UserDto;
using RDFSurveyForm.Dto.SetupDto.BranchDto;
using RDFSurveyForm.Model;
using RDFSurveyForm.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.BranchManagement.AddBranch.AddBranchHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.BranchManagement.GetBranch.GetBranch;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.BranchManagement.InActiveBranch.BranchActiveHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.BranchManagement.UpdateBranch.UpdateBranchHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.UserActive.UserActiveHandler;

namespace RDFSurveyForm.Controllers.SetupController
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;
        private readonly IMediator _mediator;
        public BranchController(IUnitOfWork unitOfWork, StoreContext context, IMediator mediator)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        [HttpPost("AddBranch")]
        public async Task<IActionResult> AddBranch(AddBranchCommand command)
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

        [HttpPut("UpdateBranch/{Id:int}")]
        public async Task<IActionResult> UpdateBranch([FromBody]UpdateBranchCommand command, [FromRoute] int Id)
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

        [HttpGet("BranchListPagination")]
        public async Task<IActionResult> CustomerListPagnation([FromQuery] GetBranchQuery query)
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

        [HttpPatch("SetIsactive/{Id:int}")]
        public async Task<IActionResult> SetIsactive([FromRoute]int Id)
        {
            try
            {
                var command = new BranchActiveCommand
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
