using Azure.Core;
using Firebase.Auth;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using nike_website_backend.Dtos;
using nike_website_backend.Helpers;
using nike_website_backend.Interfaces;
using nike_website_backend.Models;
using System.Linq.Expressions;
using System.Text;

using FirebaseAuthException = FirebaseAdmin.Auth.FirebaseAuthException;


namespace nike_website_backend.Services
{
    public class UserAccountService : IUserAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public UserAccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Object>> VerifyIdTokenAsync(string idToken)
        {
            var response = new Response<Object>();

            if (string.IsNullOrEmpty(idToken))
            {
                response.Message = "ID token is required.";
                response.StatusCode = 400;
                return response;
            }

            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                // Lấy thời điểm thu hồi token
                DateTime revokedTime = await FirebaseAuth.DefaultInstance.GetUserAsync(decodedToken.Uid)
                    .ContinueWith(task => task.Result.TokensValidAfterTimestamp);

                // So sánh thời gian tạo token với thời điểm token bị thu hồi
                DateTime authTime = DateTimeOffset.FromUnixTimeSeconds(decodedToken.IssuedAtTimeSeconds).UtcDateTime;
                if (authTime < revokedTime)
                {
                    response.Message = "ID token has been revoked.";
                    response.StatusCode = 403; // Forbidden
                    return response;
                }
                // get user info from database
                var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.UserId == decodedToken.Uid);
                if (user == null)
                {
                    response.Message = "User not found";
                    response.StatusCode = 404;
                    return response;
                }
                Object data = new
                {
                    decodedToken,
                    user
                };
                response.Data = data;
                response.Message = "Token verified successfully.";
                response.StatusCode = 200;
                return response;
            }
            catch (FirebaseAuthException ex)
            {
                response.StatusCode = ex.AuthErrorCode switch
                {
                    AuthErrorCode.ExpiredIdToken => 401,
                    AuthErrorCode.InvalidIdToken => 401,
                    AuthErrorCode.RevokedIdToken => 403,
                    _ => 500
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
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500;
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

        public async Task<Response<object>> RegisterAsync(RegisterDto userinfo)
        {
            // console.log(userinfo);
            Console.WriteLine(userinfo);
            var response = new Response<object>();

            // Validate required fields
            if (string.IsNullOrWhiteSpace(userinfo.UserEmail) ||
                string.IsNullOrWhiteSpace(userinfo.Password) ||
                string.IsNullOrWhiteSpace(userinfo.UserFirstName) ||
                string.IsNullOrWhiteSpace(userinfo.UserLastName) ||
                string.IsNullOrWhiteSpace(userinfo.UserGender) ||
                string.IsNullOrWhiteSpace(userinfo.UserPhoneNumber))
            {
                response.Message = "All fields are required";
                response.StatusCode = 400;
                return response;
            }
            try
            {
                // Check if user already exists
                try
                {
                    await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(userinfo.UserEmail);
                    response.Message = "This email is already in use";
                    response.StatusCode = 400;
                    return response;
                }
                catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
                {
                    // Expected exception when user does not exist; continue processing
                }
                catch (FirebaseAuthException ex)
                {
                    response.Message = "Error checking if user exists";
                    response.StatusCode = 500;
                    return response;
                }

                // Create user in Firebase
                var user = new UserRecordArgs
                {
                    Email = userinfo.UserEmail,
                    EmailVerified = false, // Ensure the email is unverified
                    Password = userinfo.Password,
                    Disabled = false
                };
                // Create user in Firebase 
                var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(user);

                // Send email verification
                var verificationLink = await GenerateVerificationLink(userinfo.UserEmail);
                if (verificationLink.Contains("https://nike-d3392.firebaseapp.com/__/auth/action?apiKey="))
                {
                    // Send email verification
                    var mailer = new MailerHelper();
                    await mailer.SendVerifyEmailAsync(userinfo.UserEmail, verificationLink);
                }
                else
                {
                    response.Message = verificationLink;
                    response.StatusCode = 500;
                    return response;
                }


                // Create user in your database
                var newUser = new UserAccount
                {
                    UserId = userRecord.Uid,
                    UserEmail = userRecord.Email,
                    UserFirstName = userinfo.UserFirstName,
                    UserLastName = userinfo.UserLastName,
                    UserGender = userinfo.UserGender,
                    UserPhoneNumber = userinfo.UserPhoneNumber,
                    UserAddress = "",
                    UserUrl = "",
                    RoleId = 1,
                    UserUsername = userRecord.Email.Split('@')[0] + Guid.NewGuid().ToString().Substring(0, 5)
                };
                // create user wallet
                var userWallet = new UserWallet
                {
                    UserId = userRecord.Uid,
                    Balance = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.UserWallets.Add(userWallet);
                await _context.UserWallets.AddAsync(userWallet);

                _context.UserAccounts.Add(newUser);
                await _context.SaveChangesAsync();

                // Prepare response
                response.Message = "Registration successful. Please check your email to verify your account";
                response.StatusCode = 200;
                object data = new
                {
                    User = newUser,
                    UserRecord = userRecord
                };
                response.Data = data;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500;
                return response;
            }
        }

        public async Task<string> GenerateVerificationLink(string email)
        {
            try
            {
                var actionCodeSettings = new ActionCodeSettings()
                {
                    Url = "http://localhost:3000/login",
                    HandleCodeInApp = true,
                };

                // Tạo liên kết xác minh email
                var link = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(email, actionCodeSettings);
                var idToken = await GenerateAndFetchIdToken((await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email)).Uid);
                return link;
            }
            catch (Exception ex)
            {
                return $"Error generating verification link: {ex.Message}";
            }
        }

        public async Task<Response<string>> LoginWithEmailPassword(LoginDto loginInfo)
        {
            var response = new Response<string>();
            var user_email = "";
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(loginInfo.Email) || string.IsNullOrEmpty(loginInfo.Password))
            {
                response.Message = string.IsNullOrEmpty(loginInfo.Email) ? "Email is required." : "Password is required.";
                response.StatusCode = 400; // Bad Request
                return await Task.FromResult(response);
            }

            if (loginInfo.Email.Contains("@"))
            {
                user_email = loginInfo.Email;
                var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.UserEmail == loginInfo.Email);
                if (user == null)
                {
                    response.Message = "Account not found";
                    response.StatusCode = 404; // Not Found
                    return response;
                }
            }
            else
            {
                // tim user theo username
                var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.UserUsername == loginInfo.Email);
                if (user == null)
                {
                    response.Message = "Username or password invalid";
                    response.StatusCode = 404; // Not Found
                    return response;
                }
                user_email = user.UserEmail;
            }

            try
            {
                // Gọi Firebase API để xác thực người dùng với email và password
                var firebaseConfig = ConfigHelper.LoadFirebaseConfig();
                string firebaseApiKey = firebaseConfig.API_KEY;

                var requestData = new
                {
                    email = user_email,
                    password = loginInfo.Password,
                    returnSecureToken = true
                };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    // Gửi request xác thực người dùng với email và password
                    var firebaseResponse = await client.PostAsync(
                        $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={firebaseApiKey}",
                        content
                    );

                    // Nếu Firebase trả về lỗi
                    if (!firebaseResponse.IsSuccessStatusCode)
                    {
                        var errorResponse = await firebaseResponse.Content.ReadAsStringAsync();
                        var errorDetails = JsonConvert.DeserializeObject<dynamic>(errorResponse);
                        string errorMessage = errorDetails?.error?.message;

                        // Kiểm tra thông báo lỗi cụ thể từ Firebase
                        if (errorMessage == "EMAIL_NOT_FOUND")
                        {
                            response.Message = "Username or password invalid";
                        }
                        else if (errorMessage == "INVALID_PASSWORD")
                        {
                            response.Message = "Username or password invalid";
                        }
                        else if (errorMessage == "INVALID_LOGIN_CREDENTIALS")
                        {
                            response.Message = "Username or password invalid";
                        }
                        else
                        {
                            response.Message = $"Login failed: {errorMessage}";
                        }
                        response.StatusCode = (int)firebaseResponse.StatusCode;
                        return response;
                    }

                    var responseContent = await firebaseResponse.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    string uid = result?.localId;

                    if (!string.IsNullOrEmpty(uid))
                    {
                        // Gọi hàm GenerateAndFetchIdToken để lấy ID Token từ UID
                        var idTokenResponse = await GenerateAndFetchIdToken(uid);
                        // check email is verified
                        var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(user_email);
                        if (!user.EmailVerified)
                        {
                            response.Message = "Please check your email to verify your account";
                            response.StatusCode = 400;
                            return response;
                        }
                        if (idTokenResponse.StatusCode == 200)
                        {
                            response.Data = idTokenResponse.Data;
                            response.Message = "Login successfully";
                            response.StatusCode = 200; // OK
                        }
                        else
                        {
                            response.Message = idTokenResponse.Message;
                            response.StatusCode = idTokenResponse.StatusCode;
                        }
                    }
                    else
                    {
                        response.Message = "Failed to retrieve UID from Firebase.";
                        response.StatusCode = 500; // Internal Server Error
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                response.Message = $"HTTP error: {httpEx.Message}";
                response.StatusCode = 500;
            }
            catch (Exception ex)
            {
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500;
            }
            return response;
        }

        public async Task<Response<string>> LoginWithGoogle(string idToken)
        {
            // log idToken to debug
            Console.WriteLine(idToken);
            Response<string> response = new Response<string>();
            if (string.IsNullOrEmpty(idToken))
            {
                response.Message = "idToken is required.";
                response.StatusCode = 400;
                return response;
            }

            try
            {
                // Verify token
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);

                // Extract claims
                string? email = decodedToken.Claims.TryGetValue("email", out var emailClaim) ? emailClaim.ToString() : null;

                if (string.IsNullOrEmpty(email))
                {
                    response.Message = "Email not found in the token.";
                    response.StatusCode = 400;
                    return response;
                }

                // Check if user exists
                var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.UserId == decodedToken.Uid);

                if (user == null)
                {
                    // Create new user
                    string baseUsername = email.Split('@')[0];
                    string uniqueUsername = baseUsername;
                    int counter = 0;
                    while (await _context.UserAccounts.AnyAsync(x => x.UserUsername == uniqueUsername))
                    {
                        counter++;
                        uniqueUsername = $"{baseUsername}{counter}";
                    }

                    var newUser = new UserAccount
                    {
                        UserId = decodedToken.Uid,
                        UserEmail = email,
                        UserGender = "Male",
                        UserPhoneNumber = "",
                        UserAddress = "",
                        UserUrl = "",
                        RoleId = 1,
                        UserFirstName = decodedToken.Claims.TryGetValue("name", out var name) && name != null ? name.ToString() : "",
                        UserLastName = decodedToken.Claims.TryGetValue("family_name", out var familyName) && familyName != null ? familyName.ToString() : "",
                        UserUsername = email.Split('@')[0] + Guid.NewGuid().ToString().Substring(0, 5)
                    };

                    // create user wallet
                    var userWallet = new UserWallet
                    {
                        UserId = decodedToken.Uid,
                        Balance = 0,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.UserWallets.Add(userWallet);
                    await _context.UserWallets.AddAsync(userWallet);

                    _context.UserAccounts.Add(newUser);
                    await _context.SaveChangesAsync();

                    response.Message = "Successfully created a new account with Google Account";
                    response.Data = idToken;
                    response.StatusCode = 200;
                }
                else
                {
                    response.Message = "Login successfully";
                    response.Data = idToken;
                    response.StatusCode = 200;
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500;
                return response;
            }
        }

        public async Task<Response<string>> Logout(string UserId)
        {
            Response<string> response = new Response<string>();
            if (string.IsNullOrEmpty(UserId))
            {
                response.Message = "UserId is required.";
                response.StatusCode = 400;
                return response;
            }

            try
            {
                await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(UserId);
                response.Message = "Logout successfully.";
                response.StatusCode = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500;
                return response;
            }
        }

        public async Task<Response<string>> UpdateProfile(UpdateProfileDto updateProfileDto)
        {
            Response<string> response = new Response<string>();
            if (string.IsNullOrEmpty(updateProfileDto.UserId))
            {
                response.Message = "UserId is required.";
                response.StatusCode = 400;
                return response;
            }
            try
            {
                var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.UserId == updateProfileDto.UserId);
                if (user == null)
                {
                    response.Message = "User not found.";
                    response.StatusCode = 404;
                    return response;
                }
                user.UserGender = updateProfileDto.UserGender;
                user.UserPhoneNumber = updateProfileDto.UserPhoneNumber;
                user.UserFirstName = updateProfileDto.UserFirstName;
                user.UserLastName = updateProfileDto.UserLastName;
                user.UserAddress = updateProfileDto.UserAddress;
                user.UserUrl = updateProfileDto.ImageUrl;
                user.UserUsername = updateProfileDto.UserName;
                await _context.SaveChangesAsync();
                response.Message = "Update profile successfully.";
                response.StatusCode = 200;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500;
                return response;
            }
        }

        public async Task<Response<List<HistorySearch>>> getHistorySearch(string UserId)
        {
            Response<List<HistorySearch>> response = new Response<List<HistorySearch>>();
            try
            {
                var history = await _context.HistorySearches.Where(v=>v.UserId == UserId).OrderByDescending(v=>v.CreatedAt).Take(20).ToListAsync();
                if(history == null)
                {
                    response.StatusCode = 400;
                    response.Data = [];
                    response.Message = "Hiện không có dữ liệu search";
                    return response;
                }
                response.StatusCode = 200;
                response.Data = history;
                response.Message = "Lấy dữ liệu thành công";
                return response;
            }
            catch (Exception ex) {
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500;
                response.Data = [];
                return response;
            }
        }
        public async Task<Response<Boolean>> saveHistorySearch(string userId, string keyword)
        {
            Response<Boolean> response = new Response<Boolean>();
            try
            {

                var isExists = await _context.HistorySearches.Where(p=>p.TextSearch.ToLower() == keyword.ToLower()).CountAsync();
                if (isExists > 0) {
                    response.StatusCode = 400;
                    response.Data = false;
                    response.Message = "Đã có lịch sử này";
                    return response;
                }
                var newHistory = await _context.HistorySearches.AddAsync(new HistorySearch
                {
                    UserId = userId,
                    TextSearch = keyword,
                    CreatedAt = DateTime.Now,
                });

                await _context.SaveChangesAsync();

                response.StatusCode = 200;
                response.Data = true;
                response.Message = "Lấy dữ liệu thành công";
                return response;
            }
            catch (Exception ex)
            {
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500;
                response.Data = false;
                return response;
            }
        }


        public Task<Response<string>> ForgotPassword(string email)
        {
            Response<string> response = new Response<string>();
            if (string.IsNullOrEmpty(email))
            {
                response.Message = "Email is required.";
                response.StatusCode = 400;
                return Task.FromResult(response);
            }

            // check email exist in data base
            var user = _context.UserAccounts.FirstOrDefault(x => x.UserEmail == email);
            if (user == null)
            {
                response.Message = "Email not found.";
                response.StatusCode = 404;
                return Task.FromResult(response);
            }
            try
            {
                // Send password reset email
                var actionCodeSettings = new ActionCodeSettings()
                {
                    Url = "http://localhost:3000/login",
                    HandleCodeInApp = true,
                };
                var link = FirebaseAuth.DefaultInstance.GeneratePasswordResetLinkAsync(email, actionCodeSettings);
                // Send email
                var mailer = new MailerHelper();
                var sendEmail = mailer.SendForgotPasswordEmailAsync(email, link.Result);
                response.Message = "Please check email to reset password";
                response.StatusCode = 200;
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500;
                return Task.FromResult(response);
            }
        }

        public async Task<Response<string>> ChangePassword(ChangePasswordDto changePasswordInfo)
        {
            // check valid input
            Response<string> response = new Response<string>();
            List<string> errorMessage = new List<string>();
            if (string.IsNullOrEmpty(changePasswordInfo.UserID))
            {
                errorMessage.Add("UserId ");
            }
            if (string.IsNullOrEmpty(changePasswordInfo.CurrentPassword))
            {
                errorMessage.Add("Current password ");
            }
            if (string.IsNullOrEmpty(changePasswordInfo.NewPassword))
            {
                errorMessage.Add("New password ");
            }
            if(errorMessage.Count > 0)
            {
                response.Message = string.Join(",", errorMessage) + "is required.";
                response.StatusCode = 400;
                return response;
            }
            try
            {
                var user = await _context.UserAccounts.FirstOrDefaultAsync(x => x.UserId == changePasswordInfo.UserID);
                if (user == null)
                {
                    response.Message = "User not found.";
                    response.StatusCode = 404;
                    return response;
                }

                // Verify current password
                var firebaseConfig = ConfigHelper.LoadFirebaseConfig();
                string firebaseApiKey = firebaseConfig.API_KEY;

                var requestData = new
                {
                    email = user.UserEmail,
                    password = changePasswordInfo.CurrentPassword,
                    returnSecureToken = true
                };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    var firebaseResponse = await client.PostAsync(
                        $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={firebaseApiKey}",
                        content
                    );

                    if (!firebaseResponse.IsSuccessStatusCode)
                    {
                        response.Message = "Current password is incorrect.";
                        response.StatusCode = 400;
                        return response;
                    }
                }

                // Update password in Firebase
                var updateRequest = new UserRecordArgs
                {
                    Uid = changePasswordInfo.UserID,
                    Password = changePasswordInfo.NewPassword,
                };

                await FirebaseAuth.DefaultInstance.UpdateUserAsync(updateRequest);

                // revoke all refresh tokens to logout
                //await Logout(changePasswordInfo.UserID);


                response.Message = "Password changed successfully.";
                response.StatusCode = 200;
                return response;
            }
            catch (FirebaseAuthException ex)
            {
                response.Message = $"Firebase Authentication error: {ex.Message}";
                response.StatusCode = 500;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500;
                return response;
            }
        }
    }
}