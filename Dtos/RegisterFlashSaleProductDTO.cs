using nike_website_backend.Models;

namespace nike_website_backend.Dtos
{
    public class RegisterFlashSaleProductDTO
    {

        public int? RegisterFlashSaleProduct1 { get; set; }
        public long? Sold {  get; set; }
        public int? Quantity { get; set; }
        public int? FlashSaleTimeFrameId { get; set; }
        public DateTime? started_at { get; set; }
        public DateTime? ended_at { get; set; }
        public string? status { get; set; }
    }
}
