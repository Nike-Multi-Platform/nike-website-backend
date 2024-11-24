using nike_website_backend.Models;

namespace nike_website_backend.Dtos
{
    public class FlashSaleTimeFrameDto
    {
        public int FlashSaleTimeFrameId { get; set; }
        public int FlashSaleId { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string Status { get; set; }
        public List<ProductParentDto> Products { get; set; }
        public FlashSale FlashSale { get; set; }

    }
}
