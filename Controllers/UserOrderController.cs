using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nike_website_backend.Dtos;
using nike_website_backend.Interfaces;

namespace nike_website_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserOrderController : ControllerBase
    {
        private readonly IUserOrderRepository _userOrderRepository;
        public UserOrderController(IUserOrderRepository userOrderRepository)
        {
            _userOrderRepository = userOrderRepository;
        }

        [HttpGet("get-orders")]
        public async Task<IActionResult> GetUserOrder([FromQuery]string userId, [FromQuery] int userOrderStatusId, [FromQuery] string? text, [FromQuery] int limit, [FromQuery] int page)
        {
            return Ok(await _userOrderRepository.GetUserOrder(userId, userOrderStatusId, text, limit, page));
        }

        [HttpGet("test")]
        public async Task<IActionResult> GetOrderDetailGHNAsync(string orderCode)
        {
            return Ok(await _userOrderRepository.GetOrderDetailGHNAsync(orderCode));
        }
        [HttpGet("get-details")]
        public async Task<IActionResult> GetOrderDetail([FromQuery] int userOrderId)
        {
           return Ok(await _userOrderRepository.GetOrderDetail(userOrderId));
        }
        [HttpGet("order-status")]
        public async Task<IActionResult> GetStatusList()
        {
            return Ok(await _userOrderRepository.GetStatusList());
        }
        [HttpPost("write-review")]
        public async Task<IActionResult> WriteReviews([FromBody]ReviewResponse review)
        {
            return Ok(await _userOrderRepository.WriteReviews(review));
        }
        [HttpPost("send-request")]
        public async Task<IActionResult> SendRequest(RequestDTO request)
        {
            return Ok(await _userOrderRepository.SendRequest(request));
        }
    }

}

