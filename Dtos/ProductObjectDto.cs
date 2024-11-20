namespace nike_website_backend.Dtos
{
    public class ProductObjectDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }
}