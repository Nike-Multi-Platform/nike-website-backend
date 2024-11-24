using FirebaseAdmin.Auth;
using nike_website_backend.Dtos;
using nike_website_backend.Helpers;

namespace nike_website_backend.Interfaces
{
    public interface IUserAccountRepository
    {
        Task<Response<FirebaseToken>> VerifyIdTokenAsync(string idToken);
        Task<Response<string>> GetIdTokenFromCustomToken(string customToken);
        Task<Response<string>> GenerateAndFetchIdToken(string uid);
    }
}
