namespace nike_website_backend.Dtos
{
    public class ChangePasswordDto
    {
        public string UserID { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
