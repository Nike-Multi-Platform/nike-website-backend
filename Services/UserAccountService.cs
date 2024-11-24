using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using nike_website_backend.Dtos;
using nike_website_backend.Helpers;
using nike_website_backend.Interfaces;
using nike_website_backend.Models;
using System.Text;


namespace nike_website_backend.Services
{
    public class UserAccountService : IUserAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public UserAccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<FirebaseToken>> VerifyIdTokenAsync(string idToken)
        {
            var response = new Response<FirebaseToken>();

            if (string.IsNullOrEmpty(idToken))
            {
                response.Message = "ID token is required.";
                response.StatusCode = 400; // Bad Request
                return response;
            }

            try
            {
                // Xác minh token bằng Firebase Admin SDK
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);

                response.Data = decodedToken;
                response.Message = "Token verified successfully.";
                response.StatusCode = 200; // OK
                return response;
            }
            catch (FirebaseAuthException ex)
            {
                // Xử lý các lỗi phổ biến của Firebase
                response.StatusCode = ex.AuthErrorCode switch
                {
                    AuthErrorCode.ExpiredIdToken => 401, // Unauthorized
                    AuthErrorCode.InvalidIdToken => 401, // Unauthorized
                    AuthErrorCode.RevokedIdToken => 403, // Forbidden
                    _ => 500 // Internal Server Error
                };

                response.Message = ex.AuthErrorCode switch
                {
                    AuthErrorCode.ExpiredIdToken => "ID token has expired.",
                    AuthErrorCode.InvalidIdToken => "Invalid ID token.",
                    AuthErrorCode.RevokedIdToken => "ID token has been revoked.",
                    _ => "An error occurred while verifying the token."
                };

                return response;
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi không mong đợi khác
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500; // Internal Server Error
                return response;
            }
        }

        public async Task<Response<string>> GetIdTokenFromCustomToken(string customToken)
        {
            var response = new Response<string>();

            if (string.IsNullOrEmpty(customToken))
            {
                response.Message = "Custom token is required.";
                response.StatusCode = 400;
                return response;
            }
            try
            {
                // get api key
                var firebaseConfig = ConfigHelper.LoadFirebaseConfig();
                string firebaseApiKey = firebaseConfig.API_KEY;

                using (var httpClient = new HttpClient())
                {
                    var firebaseAuthUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithCustomToken?key={firebaseApiKey}";

                    var payload = new
                    {
                        token = customToken,
                        returnSecureToken = true
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                    var firebaseResponse = await httpClient.PostAsync(firebaseAuthUrl, content);
                    var responseString = await firebaseResponse.Content.ReadAsStringAsync();

                    if (!firebaseResponse.IsSuccessStatusCode)
                    {
                        response.Message = "Error logging in with custom token.";
                        response.StatusCode = (int)firebaseResponse.StatusCode;
                        return response;
                    }

                    var result = JsonConvert.DeserializeObject<dynamic>(responseString);
                    string idToken = result?.idToken;

                    if (!string.IsNullOrEmpty(idToken))
                    {
                        response.Data = idToken;
                        response.Message = "ID Token generated successfully.";
                        response.StatusCode = 200;
                    }
                    else
                    {
                        response.Message = "Failed to retrieve ID Token.";
                        response.StatusCode = 500;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500;
            }

            return response;
        }

        public async Task<Response<string>> GenerateAndFetchIdToken(string uid)
        {
            var response = new Response<string>();

            if (string.IsNullOrEmpty(uid))
            {
                response.Message = "UID is required.";
                response.StatusCode = 400;
                return response;
            }

            try
            {
                // check user exists in firebase auth
                UserRecord userRecord;
                try
                {
                    userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
                }
                catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
                {
                    response.Message = "User not found.";
                    response.StatusCode = 404;
                    return response;
                }

                // create custom token
                string customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid);

                // get idToken from custom token
                var idTokenResponse = await GetIdTokenFromCustomToken(customToken);

                if (idTokenResponse.StatusCode == 200)
                {
                    response.Data = idTokenResponse.Data;
                    response.Message = "ID Token generated successfully.";
                    response.StatusCode = 200;
                }
                else
                {
                    response.Message = idTokenResponse.Message;
                    response.StatusCode = idTokenResponse.StatusCode;
                }
            }
            catch (FirebaseAuthException ex)
            {
                response.Message = $"Firebase Authentication error: {ex.Message}";
                response.StatusCode = 500;
            }
            catch (Exception ex)
            {
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500;
            }

            return response;
        }

    }
}
