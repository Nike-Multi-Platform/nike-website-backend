using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using nike_website_backend.Dtos;
using nike_website_backend.Helpers;
using nike_website_backend.Interfaces;
using nike_website_backend.Models;

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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto userinfo)
        {
            return Ok(await _userAccountRepository.RegisterAsync(userinfo));
        }

        [HttpPost("login-with-email-password")]
        public async Task<IActionResult> LoginWithEmailPassword([FromBody] LoginDto loginInfo)
        {
            return Ok(await _userAccountRepository.LoginWithEmailPassword(loginInfo));
        }

        [HttpPost("login-with-google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] string idToken)
        {
            return Ok(await _userAccountRepository.LoginWithGoogle(idToken));
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string UserId)
        {
            return Ok(await _userAccountRepository.Logout(UserId));
        }

        [HttpPost("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto user_info)
        {
            return Ok(await _userAccountRepository.UpdateProfile(user_info));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassWord([FromBody] string email)
        {
            return Ok(await _userAccountRepository.ForgotPassword(email));
        }
        [HttpPost("save-search")]
        public async Task<IActionResult> saveHistorySearch([FromQuery] string userId, [FromQuery] string keyword)
        {
            return Ok(await _userAccountRepository.saveHistorySearch(userId, keyword));
        }
        [HttpGet("get-histories-search")]
        public async Task<IActionResult> getHistorySearch([FromQuery]string UserId)
        {
            return Ok(await _userAccountRepository.getHistorySearch(UserId));
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordInfo)
        {
            return Ok(await _userAccountRepository.ChangePassword(changePasswordInfo));
        }
    }
}
