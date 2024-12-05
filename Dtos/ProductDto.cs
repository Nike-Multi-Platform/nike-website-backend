namespace nike_website_backend.Dtos
{
    public class ProductDto
    {
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? ProductParentId { get; set; }
        public string? ProductImage { get; set; }
        public string? categoryWithObjectName { get; set; }
        public int? stock { get; set; }
        public decimal? price { get; set; }
        public decimal? salePrice { get; set; }
        public decimal? finalPrice { get; set; }
        public int? weight { get; set; }
        public int? height { get; set; }
        public int? length { get; set; }
        public int? width { get; set; }
        // ... more properties
    }
}