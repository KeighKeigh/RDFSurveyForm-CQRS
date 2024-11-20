using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common;
using RDFSurveyForm.Common.EXTENSIONS;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.DATA_ACCESS_LAYER.Features.CategoryManagement.InActiveCategory;
using RDFSurveyForm.Dto.SetupDto.CategoryDto;
using RDFSurveyForm.Dto.SetupDto.GroupDto;
using RDFSurveyForm.Model;
using RDFSurveyForm.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.CategoryManagement.AddCategory.AddCategoryHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.CategoryManagement.DeleteCategory.DeleteCategoryHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.CategoryManagement.GetCategory.GetCategory;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.CategoryManagement.InActiveCategory.CategoryActiveHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.CategoryManagement.UpdateCategory.UpdateCategoryHandler;
using static RDFSurveyForm.DATA_ACCESS_LAYER.Features.UserManagement.UserActive.UserActiveHandler;

namespace RDFSurveyForm.Controllers.SetupController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;
        private readonly IMediator _mediator;

        public CategoryController(IUnitOfWork unitOfWork,StoreContext context, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mediator = mediator;                
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory(AddCategoryCommand command)
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

        [HttpPut("UpdateCategory/{Id:int}")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryCommand command, [FromRoute] int Id)
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

        [HttpGet("CategoryListPagination")]
        public async Task<IActionResult> CategoryListPagnation([FromQuery] GetCategoryQuery query)
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

        [HttpPatch("SetIsActive/{Id:int}")]
        public async Task<IActionResult> SetIsActive([FromRoute] int Id)
        {
            try
            {
                var command = new CategoryActiveCommand
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

        [HttpDelete("DeleteCategory/{Id:int}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int Id)
        {
            try
            {
                var command = new DeleteCategoryCommand
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
