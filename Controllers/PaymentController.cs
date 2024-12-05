using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nike_website_backend.Dtos;
using nike_website_backend.Interfaces;
using System.Text.Json;

namespace nike_website_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        public IPaymentRepository _paymentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PaymentController(IPaymentRepository iPaymentRepository, IHttpContextAccessor httpContextAccessor)
        {
            _paymentRepository = iPaymentRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("create-vnpay-url")]
        public async Task<IActionResult> CreatePaymentUrlVNPay([FromBody]PaymentInformationModel model)
        {
          
            return Ok(await _paymentRepository.CreatePaymentUrl(model, HttpContext));
        }
        [HttpPost("checkout-by-cod")]
        public async Task<IActionResult> checkoutByCOD(PaymentInformationModel model)
        {
            return Ok(await _paymentRepository.checkoutByCOD(model));

        }

        [HttpGet("vnpay-return-result")]
        public async Task<IActionResult> PaymentExecute()
        {
            var collections = _httpContextAccessor.HttpContext.Request.Query;
            //foreach (var item in collections)
            //{
            //    Console.WriteLine($"Key: {item.Key}, Value: {item.Value}");
            //}
            return Ok(await _paymentRepository.PaymentExecute(collections));
        }

    }
}
