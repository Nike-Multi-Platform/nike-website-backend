namespace nike_website_backend.Dtos
{
    public class ProductSizeDto
    {
        public int ProductSizeId { get; set; }
        public int? Quantity { get; set; }
        public SizeDto SizeDto { get; set; }
    }
}