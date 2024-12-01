using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nike_website_backend.Interfaces;

namespace nike_website_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteRepository _favoriteRepository;
        public FavoriteController(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }
      
        [HttpPost("add-to-favorites/{userId}")]
        public async Task<IActionResult> addToFavorite( String userId, [FromQuery] int product_id)
        {
            return Ok(await _favoriteRepository.addToFavorite(userId, product_id));
        }
        [HttpPost("remove-from-favorites/{id}")]
        public async Task<IActionResult> removeFromFavorite(int id)
        {
            return Ok(await _favoriteRepository.removeFromFavorite(id));
        }
        [HttpGet("get-favorites/{userId}")]
        public async Task<IActionResult> getFavorites(string userId,[FromQuery] int page,[FromQuery] int limit)
        {
            return Ok(await _favoriteRepository.getFavorites(userId,page,limit));
        }
    }
}
