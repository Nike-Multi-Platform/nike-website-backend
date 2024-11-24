﻿using FirebaseAdmin.Auth;
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
        Task<Response<string>> LoginWithGoogle(string googleIdToken);
    }
}
