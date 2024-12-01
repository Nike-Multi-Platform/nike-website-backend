namespace nike_website_backend.Dtos
{
    public class BagDto
    {
        public int bagId { get; set; }
        public String userId { get; set; }
        public int product_size_id { get; set; }
        public int amount { get; set; }
        public Boolean is_selected { get; set; }
        public ProductDetailDto productDetail { get; set; }
        
    }
}
