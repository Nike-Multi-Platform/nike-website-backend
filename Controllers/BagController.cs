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
    }
        
    }
