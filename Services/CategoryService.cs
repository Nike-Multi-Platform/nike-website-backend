using Microsoft.EntityFrameworkCore;
using nike_website_backend.Dtos;
using nike_website_backend.Interfaces;
using nike_website_backend.Models;

namespace nike_website_backend.Services
{
    public class CategoryService : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<CategoryDto>> getSubCategoriesByCategoryId(int categoryId)
        {
            Response<CategoryDto> response = new Response<CategoryDto>();

            var category = await _context.Categories.Where(c => c.CategoriesId == categoryId)
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoriesId,
                CategoryName = c.CategoriesName,
                SubCategories = c.SubCategories.Select(sc => new SubCategoryDto
                {
                    SubCategoryId = sc.SubCategoriesId,
                    SubCategoryName = sc.SubCategoriesName,
                    CategoryId = sc.CategoriesId
                }).ToList()
            }).FirstOrDefaultAsync();
            if (category == null)
            {
                response.StatusCode = 404;
                response.Message = "Không tìm thấy danh mục";
                return response;
            }

            response.StatusCode = 200;
            response.Message = "Lấy danh mục thành công";
            response.Data = category;
            return response;
        }

    }
}