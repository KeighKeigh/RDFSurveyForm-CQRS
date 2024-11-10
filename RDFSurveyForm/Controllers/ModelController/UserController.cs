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
using RDFSurveyForm.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.GetUsers.GetUser;
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

            //user.Id = Id;
            //var fname = await _unitOfWork.Customer.UserAlreadyExist(user.FullName);
            //var uname = await _unitOfWork.Customer.UserNameAlreadyExist(user.UserName);

            //var existingUser = await _context.Customer.FirstOrDefaultAsync(x => x.Id == user.Id);

            //if (string.IsNullOrEmpty(user.FullName))
            //{
            //    return BadRequest("Enter Full name");
            //}
            //if (string.IsNullOrEmpty(user.UserName))
            //{
            //    return BadRequest("Enter Username");
            //}
            //if (fname == false && user.FullName != existingUser.FullName)
            //{
            //    return BadRequest("Name already Exist!");

            //}
            //if (uname == false && user.UserName != existingUser.UserName)
            //{
            //    return BadRequest("Username already Exist!");
            //}

            //var users = await _unitOfWork.Customer.UpdateUser(user);
            //if (users == false)
            //{
            //    return BadRequest("User Not Found!");
            //}
            //return Ok("User Updated Successfuly!");
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


        

        [HttpGet("CustomerListPagination")]
        public async Task<ActionResult<IEnumerable<GetUserDto>>> CustomerListPagnation([FromQuery] UserParams userParams, bool? status, string search)
        {
            var usersummary = await _unitOfWork.Customer.CustomerListPagnation(userParams, status, search);

            Response.AddPaginationHeader(usersummary.CurrentPage, usersummary.PageSize, usersummary.TotalCount, usersummary.TotalPages, usersummary.HasNextPage, usersummary.HasPreviousPage);

            var usersummaryResult = new
            {
                usersummary,
                usersummary.CurrentPage,
                usersummary.PageSize,
                usersummary.TotalCount,
                usersummary.TotalPages,
                usersummary.HasNextPage,
                usersummary.HasPreviousPage
            };

            return Ok(usersummaryResult);
        }

        [HttpPatch("SetIsActive/{Id}")]
        public async Task<IActionResult> SetIsActive([FromRoute] int Id)
        {
            var setisactive = await _unitOfWork.Customer.SetIsActive(Id);
            if (setisactive == false)
            {
                return BadRequest("Id does not exist!");
            }
            return Ok("Updated");
        }

        [HttpPut("Resetpassword/{Id:int}")]
        public async Task<IActionResult> ResetPassword([FromRoute]int Id)
        {
            var resetPassord = await _unitOfWork.Customer.ResetPassword(Id);
            if(resetPassord == false)
            {
                return BadRequest("Id does not exist!");
            }
            return Ok("Password Reset");
        }

    }
}
