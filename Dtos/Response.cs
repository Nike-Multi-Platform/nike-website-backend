
using Firebase.Auth;
using nike_website_backend.Models;

namespace nike_website_backend.Dtos
{
    public class Response<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int TotalPages { get; internal set; }
    }
}