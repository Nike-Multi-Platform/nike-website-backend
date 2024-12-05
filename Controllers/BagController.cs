using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nike_website_backend.Interfaces;

namespace nike_website_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BagController : ControllerBase
    {
        private readonly IBagRepository _bagRepository;
        public BagController(IBagRepository bagRepository)
        {
            _bagRepository = bagRepository;
        }

        [HttpGet("get-bag/{userId}")]
        public async Task<IActionResult> getBag(String userId)
        {
            return Ok(await _bagRepository.getBag(userId));
        }
        [HttpPost("add-to-bag/{user_id}")]
        public async Task<IActionResult> addToBag(String user_id,[FromQuery] int product_size_id, [FromQuery] int amount)
        {
            return Ok(await _bagRepository.addToBag(user_id,product_size_id,amount));
        }
        [HttpPost("remove-item/{bagId}")]
        public async Task<IActionResult> removeBagItem(int bagId)
        {
            return Ok(await _bagRepository.removeBagItem(bagId));
        }
        [HttpPost("update-quantity/{bag_id}")]
        public async Task<IActionResult> updateItemQuantity(int bag_id,[FromQuery] int quantity)
        {
            return Ok(await _bagRepository.updateItemQuantity(bag_id, quantity));
        }
        [HttpPost("update-select/{bag_id}")]
        public async Task<IActionResult> updateSelected(int bag_id,[FromQuery] Boolean isSelected)
        {
            return Ok(await _bagRepository.updateSelected(bag_id, isSelected));
        }
        [HttpPost("update-size/{bag_id}")]
        public async Task<IActionResult> updateSize(int bag_id,[FromQuery]String userId, [FromQuery] int product_size_id)
        {
            return Ok(await _bagRepository.updateSize(bag_id,userId, product_size_id));
        }
        [HttpGet("get-sizes/{product_id}")]
        public async Task<IActionResult> getProductSizes(int product_id)
        {
            return Ok(await _bagRepository.getProductSizes(product_id));
        }
        [HttpGet("get-total-items/{userId}")]
        public async Task<IActionResult> getTotalAmount(String userId)
        {
            return Ok(await _bagRepository.getTotalAmount(userId));
        }
        [HttpGet("apply-voucher")]
        public async Task<IActionResult> applyVoucher([FromQuery] string userId,[FromQuery] string promoCode)
        {
            return Ok(await _bagRepository.applyVoucher(userId, promoCode));
        }
    }
        
    }
