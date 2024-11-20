namespace nike_website_backend.Dtos
{
    public class ProductParentDto
    {
        public int ProductParentId { get; set; }
        public string ProductParentName { get; set; }
        public ProductIconDto ProductIcon { get; set; }
        public List<ProductDto> Products { get; set; }
        public string Thumbnail { get; set; }
        public decimal? ProductPrice { get; set; }
        public bool? IsNew { get; set; }
    }
}