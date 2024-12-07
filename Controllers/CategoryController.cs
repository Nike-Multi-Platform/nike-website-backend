using Microsoft.AspNetCore.Mvc;
using nike_website_backend.Interfaces;

namespace nike_website_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("{categoryId}")]
        // Bấm vào subcategory trên menu sẽ lấy category và từ category lấy ra subcategory
        public async Task<IActionResult> getSubCategoriesByCategoryId([FromRoute] int categoryId)
        {
            return Ok(await _categoryRepository.getSubCategoriesByCategoryId(categoryId));
        }

        // from query
        [HttpGet("subcategories-category-object")]
        public async Task<IActionResult> getCategoriesByCatAndObject([FromQuery] int categoryId, [FromQuery] int productObjectId)
        {
            return Ok(await _categoryRepository.getCategoriesByCatAndObject(categoryId, productObjectId));
        }
    }
}