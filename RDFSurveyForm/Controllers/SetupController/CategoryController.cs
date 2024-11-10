﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common.EXTENSIONS;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.Dto.SetupDto.CategoryDto;
using RDFSurveyForm.Dto.SetupDto.GroupDto;
using RDFSurveyForm.Services;

namespace RDFSurveyForm.Controllers.SetupController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _context;

        public CategoryController(IUnitOfWork unitOfWork,StoreContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory(AddCategoryDto category)
        {
            var categoryChecker = await _unitOfWork.Category.PercentageChecker(category);
            if (categoryChecker == false)
            {
                return BadRequest("% exceeded 100%");
            }
            var categoryExist = await _unitOfWork.Category.CategoryAlreadyExist(category.CategoryName);
            if (categoryExist == false)
            {
                return BadRequest("Category Already Exist!");
            }
            
            await _unitOfWork.Category.AddCategory(category);
            return Ok("Category Added!");
        }

        [HttpPut("UpdateCategory/{Id:int}")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDto category, [FromRoute] int Id)
        {
            category.Id = Id;
            var categoryExist = await _unitOfWork.Category.CategoryAlreadyExist(category.CategoryName);
            var updateCategory = await _context.Category.FirstOrDefaultAsync(x => x.Id == category.Id);
            if (categoryExist == false && category.CategoryName != updateCategory.CategoryName)
            {
                return Ok("Category Name Already Exist!");


            }
            var categoryChecker = await _unitOfWork.Category.PercentageCheckers(category);
            if (categoryChecker == false)
            {
                return BadRequest("% exceeded 100%");
            }

            var categoryId = await _unitOfWork.Category.UdpateCategory(category);
            if (categoryId == false)
            {
                return BadRequest("Category Id Not Found!");
            }
            return Ok("Update Successfuly!");
        }

        [HttpGet("CategoryListPagination")]
        public async Task<ActionResult<IEnumerable<GetCategoryDto>>> CategoryListPagnation([FromQuery] UserParams userParams, bool? status, string search)
        {
            var categorysummary = await _unitOfWork.Category.CategoryPagnation(userParams, status, search);
            Response.AddPaginationHeader(categorysummary.CurrentPage, categorysummary.PageSize, categorysummary.TotalCount, categorysummary.TotalPages, categorysummary.HasNextPage, categorysummary.HasPreviousPage);

            var categorysummaryResult = new
            {
                categorysummary,
                categorysummary.CurrentPage,
                categorysummary.PageSize,
                categorysummary.TotalCount,
                categorysummary.TotalPages,
                categorysummary.HasNextPage,
                categorysummary.HasPreviousPage
            };
            return Ok(categorysummaryResult);
        }

        [HttpDelete("DeleteCategory/{Id:int}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int Id)
        {
            var deleteCategory = await _unitOfWork.Category.DeleteCategory(Id);
            if (deleteCategory == false)
            {
                return BadRequest("Category does not exist!");
            }
            return Ok("Deleted");
        }

        
    }
}
