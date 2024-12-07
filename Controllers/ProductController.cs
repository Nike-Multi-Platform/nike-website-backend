using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using nike_website_backend.Helpers;
using nike_website_backend.Interfaces;

namespace nike_website_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("product-objects")]
        public async Task<IActionResult> GetProductObjects()
        {
            return Ok(await _productRepository.GetProductObjects());
        }

        [HttpGet("product-parents")]
        public async Task<IActionResult> GetProductParents([FromQuery] QueryObject queryObject)
        {
            return Ok(await _productRepository.GetProductParents(queryObject));
        }

        [HttpGet("product-parent-detail/{productParentId}")]
        public async Task<IActionResult> GetProductParentDetail(int productParentId)
        {
            return Ok(await _productRepository.GetProductParentDetail(productParentId));
        }
        [HttpGet("product-detail/{productId}")]
        public async Task<IActionResult> GetProductDetail(int productId)
        {
            return Ok(await _productRepository.GetProductDetail(productId));
        }
        [HttpGet("product-icons")]
        public async Task<IActionResult> GetIcons([FromQuery] int page, [FromQuery] int limit)
        {
            return Ok(await _productRepository.GetIcons(page,limit));
        }
        [HttpGet("new-release")]
        public async Task<IActionResult> GetNewRelease([FromQuery] int page, [FromQuery] int limit)
        {
            return Ok(await _productRepository.GetNewRelease(page,limit));
        }
        [HttpGet("products-by-object-id")]
        public async Task<IActionResult> GetProductByObjectID([FromQuery] int page, [FromQuery] int limit,[FromQuery] int objectId)
        {
            return Ok(await _productRepository.GetProductByObjectID(page, limit,objectId));
        }
        [HttpGet("product-reviews/{productId}")]
        public async Task<IActionResult> getReviewsOfColor(int productId, [FromQuery] int page, [FromQuery] int limit, [FromQuery] string sortBy, [FromQuery] int rating)
        {
            return Ok(await _productRepository.getReviewsOfColor(productId,page,limit,sortBy,rating));
        }
        [HttpGet("recommendation")]
        public async Task<IActionResult> getRecommendProductParent([FromQuery] string? userId, [FromQuery] int limit)
        {
            return Ok(await _productRepository.getRecommendProductParent(userId,limit));
        }

        [HttpGet("search-filter")]
        public async Task<IActionResult> SearchFilter([FromQuery] SearchFilterQueryObject queryObject)
        {
            return Ok(await _productRepository.SearchFilter(queryObject));
        }

    }
}