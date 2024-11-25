using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class ProductParent
{
    public int ProductParentId { get; set; }

    public string ProductParentName { get; set; } = null!;

    public int ProductIconsId { get; set; }

    public string Thumbnail { get; set; } = null!;

    public decimal ProductPrice { get; set; }

    public bool IsNewRelease { get; set; }

    public int SubCategoriesId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int Weight { get; set; }

    public int Height { get; set; }

    public int Length { get; set; }

    public int Width { get; set; }

    public virtual ProductIcon ProductIcons { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<RegisterFlashSaleProduct> RegisterFlashSaleProducts { get; set; } = new List<RegisterFlashSaleProduct>();

    public virtual SubCategory SubCategories { get; set; } = null!;
}
