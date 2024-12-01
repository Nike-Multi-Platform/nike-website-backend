using nike_website_backend.Models;
namespace nike_website_backend.Dtos
{
    public class UserFavoriteProductsDto
    {
        public int Id { get; set; }
        public String userId { get; set; }
        public int ProductId { get; set; } 
        public ProductDto Product { get; set; }
    }
}
