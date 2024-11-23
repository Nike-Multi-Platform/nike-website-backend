using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using nike_website_backend.Helpers;
using nike_website_backend.Interfaces;

namespace nike_website_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserAccountController : ControllerBase
    {
       private readonly IUserAccountRepository _userAccountRepository;
        public UserAccountController(IUserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }
        [HttpPost("verify-id-token")]
        public async Task<IActionResult> VerifyIdToken([FromBody] string idToken)
        {
            return Ok(await _userAccountRepository.VerifyIdTokenAsync(idToken));
        }

        [HttpPost("get-id-token-from-custom-token")]
        public async Task<IActionResult> GetIdTokenFromCustomToken([FromBody] string customToken)
        {
            return Ok(await _userAccountRepository.GetIdTokenFromCustomToken(customToken));
        }

        [HttpPost("generate-and-fetch-id-token")]
        public async Task<IActionResult> GenerateAndFetchIdToken([FromBody] string uid)
        {
            return Ok(await _userAccountRepository.GenerateAndFetchIdToken(uid));
        }

    }
}
