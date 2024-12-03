using nike_website_backend.Models;
using System.Numerics;

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
        public bool? IsNew { get; set; }

        public int? ProductIconsId { get; set; }
        public decimal? salePrice { get; set; } // Giá khuyến mãi
        public decimal? ProductPrice { get; set; } // Giá gốc
        public int ProductObjectId { get; set; }
        public decimal? finalPrice { get; set; } // Giá cuối cùng (là giá khuyến mãi nếu có, không thì là giá gốc)
        public string? categoryWithObjectName { get; set; }
        public ProductIconDto ProductIcon { get; set; }
        public List<ProductDto> Products { get; set; }
        public  SubCategory? SubCategories { get; set; }
        public RegisterFlashSaleProduct RegisterFlashSaleProduct { get; set; }
        public long? sold { get; set; }
        public int? quantity { get; set; }
        public int? quantityInStock { get; set; }
    }
}