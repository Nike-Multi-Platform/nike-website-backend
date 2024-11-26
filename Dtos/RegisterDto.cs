using nike_website_backend.Models;

namespace nike_website_backend.Dtos
{
    public class RegisterDto
    {
        public string UserEmail { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string UserGender { get; set; } = null!;
        public string UserPhoneNumber { get; set; } = null!;
        public string UserFirstName { get; set; } = null!;
        public string UserLastName { get; set; } = null!;
    }
}
