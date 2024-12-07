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

        public async Task<Response<List<CategoryDto>>> getCategoriesByCatAndObject(int categoryId, int productObjectId)
        {
            Response<List<CategoryDto>> response = new Response<List<CategoryDto>>();
            var query = _context.Categories.AsQueryable();

            if (categoryId > 0)
            {
                query = query.Where(c => c.CategoriesId == categoryId);
            }

            if (productObjectId > 0)
            {
                query = query.Where(c => c.ProductObjectId == productObjectId);
            }

            var categories = await query
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoriesId,
                    CategoryName = c.CategoriesName,
                    SubCategories = c.SubCategories
                        .Select(sc => new SubCategoryDto
                        {
                            SubCategoryId = sc.SubCategoriesId,
                            SubCategoryName = sc.SubCategoriesName,
                            CategoryId = sc.CategoriesId
                        }).ToList()
                })
                .Where(c => c.SubCategories.Any()) // Filter out categories without subcategories
                .ToListAsync();

            if (!categories.Any())
            {
                response.StatusCode = 404;
                response.Message = "Không tìm thấy danh mục";
                return response;
            }

            response.StatusCode = 200;
            response.Message = "Lấy danh mục thành công";
            response.Data = categories;
            return response;
        }
    }
}