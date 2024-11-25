namespace nike_website_backend.Dtos
{
    public class ProductDetailDto
    {
        public int ProductId { get; set; }
        public string MoreInfo { get; set; }
        public string ProductImage { get; set; }
        public string SizeAndFit { get; set; }
        public string StyleCode { get; set; }
        public string ColorShown { get; set; }
        public List<ProductImageDto> ProductImageDtos { get; set; }
        public List<ProductSizeDto> ProductSizeDtos { get; set; }
        public List<ProductReviewDto> ProductReviewDtos { get; set; }
        public decimal? salePrice { get; set; }
        // public string Description { get; set; }
        // ... more properties
    }
}