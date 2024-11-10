﻿using Microsoft.EntityFrameworkCore;
using RDFSurveyForm.Common.HELPERS;
using RDFSurveyForm.Data;
using RDFSurveyForm.DataAccessLayer.IR_Setup.Interface;
using RDFSurveyForm.Dto.ModelDto.UserDto;
using RDFSurveyForm.Dto.SetupDto.CategoryDto;
using RDFSurveyForm.Dto.SetupDto.GroupDto;
using RDFSurveyForm.Model.Setup;
using RDFSurveyForm.Services;
using System.Linq;

namespace RDFSurveyForm.DataAccessLayer.IR_Setup.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly StoreContext _context;
 

        public CategoryRepository(StoreContext context)
        {
            _context = context;

        }

        public async Task<bool> CategoryAlreadyExist(string categoryName)
        {
            var categoryAlreadyExist = await _context.Category.AnyAsync(x => x.CategoryName == categoryName);
            if (categoryAlreadyExist)
            {
                return false;
            }
            return true;

        }

        public async Task<bool> PercentageChecker(AddCategoryDto category)
        {
            var users = await _context.Category.Where(x => x.IsActive).ToListAsync();
            var percentage = category.CategoryPercentage * .01M;
            var result = users.Sum(x => x.CategoryPercentage);
            var total = result + percentage;
            if (total > 1)
            {
                return false;
            }
            return true;

        }
        public async Task<bool> PercentageCheckers(UpdateCategoryDto category)
        {
            var users = await _context.Category.Where(x => x.IsActive && x.Id != category.Id) .ToListAsync();
            var percentage = category.CategoryPercentage * .01M;
            var result = users.Sum(x => x.CategoryPercentage);
            var total = result + percentage;
            if (total > 1)
            {
                return false;
            }
            return true;

        }
        public async Task<bool> AddCategory(AddCategoryDto category)
        {
            var addCategory = new Category
            {
                CategoryName = category.CategoryName,
                CategoryPercentage = category.CategoryPercentage * .01M,
                CreatedAt = DateTime.Now,
                CreatedBy = category.CreatedBy,
                Limit = category.Limit,

            };
            await _context.Category.AddAsync(addCategory);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UdpateCategory(UpdateCategoryDto category)
        {
            var updateCategory = await _context.Category.FirstOrDefaultAsync(x => x.Id == category.Id);
            if (updateCategory != null)
            {
                updateCategory.CategoryName = category.CategoryName;
                updateCategory.CategoryPercentage = category.CategoryPercentage * .01M;
                updateCategory.UpdatedAt = DateTime.Now;
                updateCategory.UpdatedBy = category.UpdatedBy;
                updateCategory.Limit = category.Limit;

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<PagedList<GetCategoryDto>> CategoryPagnation(UserParams userParams, bool ? status, string  search)
        {
            var category = _context.Category.Select(x => new GetCategoryDto
            {
                Id = x.Id,
                CategoryName = x.CategoryName,
                CategoryPercentage = x.CategoryPercentage * 100,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                IsActive = x.IsActive,
                Limit = x.Limit,

            });

            if(status != null)
            {
                category = category.Where(x => x.IsActive == status);
            }

            if(!string.IsNullOrEmpty(search))
            {
                category = category.Where(x => Convert.ToString(x.Id).ToLower().Contains(search.Trim().ToLower())
                || Convert.ToString(x.CategoryName).ToLower().Contains(search.Trim().ToLower())
                || Convert.ToString(x.CategoryPercentage).ToLower().Contains(search.Trim().ToLower())
                || Convert.ToString(x.CreatedBy).ToLower().Contains(search.Trim().ToLower())
                || Convert.ToString(x.UpdatedBy).ToLower().Contains(search.Trim().ToLower()));
            }
            return await PagedList<GetCategoryDto>.CreateAsync(category, userParams.PageNumber, userParams.PageSize);
        }



        public async Task<bool> DeleteCategory(int Id)
        {
            var deleteCategory = await _context.Category.FirstOrDefaultAsync(info => info.Id == Id);
            if (deleteCategory != null)
            {
                _context.Remove(deleteCategory);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
