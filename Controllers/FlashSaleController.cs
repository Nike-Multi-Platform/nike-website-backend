﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nike_website_backend.Interfaces;

namespace nike_website_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashSaleController : ControllerBase
    {
        public IFlashSaleRepository _flashSaleRepository;
        public FlashSaleController(IFlashSaleRepository iFlashSaleRepository)
        {
            _flashSaleRepository = iFlashSaleRepository;
        }

        [HttpGet("/get-current-flash-sales")]
        public async Task<IActionResult> getActiveFlashSale([FromQuery] int limit)
        {
            return Ok(await _flashSaleRepository.getActiveFlashSale(limit));
        }
    }
}