namespace nike_website_backend.Dtos
{
    public class UpdateProfileDto
    {
        public string UserId { get; set; } = null!; 
        public string UserGender { get; set; } = null!;
        public string UserPhoneNumber { get; set; } = null!;
        public string UserFirstName { get; set; } = null!;
        public string UserLastName { get; set; } = null!;
        public string UserAddress { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;   
    }
}
