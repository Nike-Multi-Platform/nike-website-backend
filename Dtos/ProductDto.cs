namespace nike_website_backend.Dtos
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductImage { get; set; }
        public string categoryWithObjectName { get; set; }
        public int? stock { get; set; }
        public decimal? price { get; set; }
        public decimal? salePrice { get; set; }
        // ... more properties
    }
}