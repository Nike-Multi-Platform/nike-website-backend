using nike_website_backend.Models;

namespace nike_website_backend.Dtos
{
    public class BagDto
    {
        public int? bagId { get; set; }
        public String? userId { get; set; }
        public int product_size_id { get; set; }
        public string? ProductSizeName { get; set; }
        public int? amount { get; set; }
        public Boolean? is_selected { get; set; }
        public ProductDto? details { get; set; }
        public RegisterFlashSaleProductDTO? RegisterFlashSaleProduct { get; set; }
    }
}
