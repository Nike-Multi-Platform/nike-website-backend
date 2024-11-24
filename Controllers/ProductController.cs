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

        [HttpGet("product-parents/{subCategoryId}")]
        public async Task<IActionResult> GetProductParents(int subCategoryId, [FromQuery] QueryObject queryObject)
        {
            return Ok(await _productRepository.GetProductParents(subCategoryId, queryObject));
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

    }
}