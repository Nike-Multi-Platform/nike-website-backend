using nike_website_backend.Models;

namespace nike_website_backend.Dtos
{
    public class ProductParentDto
    {
        public int ProductParentId { get; set; }
        public string ProductParentName { get; set; }

        public int? SubCategoriesId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string Thumbnail { get; set; }
        public decimal? ProductPrice { get; set; }
        public bool? IsNew { get; set; }

        public int? ProductIconsId { get; set; }
        public decimal? salePrice { get; set; }
        public string? categoryWithObjectName { get; set; }
        public ProductIconDto ProductIcon { get; set; }
        public List<ProductDto> Products { get; set; }
        public  SubCategory? SubCategories { get; set; }
        public RegisterFlashSaleProduct RegisterFlashSaleProduct { get; set; }
     
    }
}