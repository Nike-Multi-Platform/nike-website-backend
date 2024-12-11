using FirebaseAdmin.Auth;
using nike_website_backend.Dtos;
using nike_website_backend.Helpers;
using nike_website_backend.Models;

namespace nike_website_backend.Interfaces
{
    public interface IUserAccountRepository
    {
        Task<Response<Object>> VerifyIdTokenAsync(string idToken);
        Task<Response<string>> GetIdTokenFromCustomToken(string customToken);
        Task<Response<string>> GenerateAndFetchIdToken(string uid);
        Task<Response<Object>> RegisterAsync(RegisterDto userinfo);
        Task<Response<string>> LoginWithEmailPassword(LoginDto loginInfo);
        Task<Response<string>> LoginWithGoogle(string idToken);
        Task<Response<string>> Logout(string UserId);
        Task<Response<string>> UpdateProfile(UpdateProfileDto user_info);
        Task<Response<string>> ForgotPassword(string email);
        Task<Response<List<HistorySearch>>> getHistorySearch(string UserId);
        Task<Response<Boolean>> saveHistorySearch(string userId, string keyword);
        Task<Response<string>> ChangePassword(ChangePasswordDto changePasswordInfo);
    }
}
