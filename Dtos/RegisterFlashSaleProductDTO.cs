using nike_website_backend.Models;

namespace nike_website_backend.Dtos
{
    public class RegisterFlashSaleProductDTO
    {
        public RegisterFlashSaleProduct RegisterFlashSaleProduct { get; set; }
        public DateTime? started_at { get; set; }
        public DateTime? ended_at { get; set; }
        public string? status { get; set; }
    }
}
