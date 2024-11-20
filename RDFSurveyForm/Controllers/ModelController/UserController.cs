using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RDFSurveyForm.Common;
using RDFSurveyForm.Common.EXTENSIONS;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Dto.ModelDto.UserDto;
using RDFSurveyForm.Model;
using RDFSurveyForm.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.GetUsers.GetUser;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.ResetPassword.ResetPasswordHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.UserActive.UserActiveHandler;
using static RDFSurveyForm.Handlers.Errors.Features.UserManagement.AddUserHandler;
using static RDFSurveyForm.Handlers.UpdatePasswordHandler;
using static RDFSurveyForm.Handlers.UpdateUserHandler;

namespace RDFSurveyForm.Controllers.ModelController
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;
        private readonly IMediator _mediator;

        public UserController(IUnitOfWork unitOfWork, StoreContext context, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mediator = mediator;

        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddNewUser(AddUserCommand command)
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

        [HttpPut("UpdateUser")]

        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand user)
        {
            try
            {
                var result = await _mediator.Send(user);

                if (result.IsFailure)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }


        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                if (result.IsFailure)
                {
                    return BadRequest(result);
                }

                return Ok(result);

            }
            catch(Exception ex)
            {
                return Conflict(ex.Message);
            }


        }

        [HttpGet("page")]
        public async Task<IActionResult> GetUser([FromQuery] GetUserQuery query)
        {
            try
            {
                var users  = await _mediator.Send(query);

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
            catch(Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


       

        [HttpPatch("SetIsActive/{Id:int}")]
        public async Task<IActionResult> SetIsActive([FromRoute] int Id)
        {
            try
            {
                var command = new UserActiveCommand
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

        [HttpPut("Resetpassword/{id:int}")]
        public async Task<IActionResult> ResetPassword([FromRoute] int id)
        {

            try
            {
                var command = new ResetPasswordCommand
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

    }
}
