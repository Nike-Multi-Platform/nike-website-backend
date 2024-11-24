using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class ProductParent
{
    public int ProductParentId { get; set; }

    public string? ProductParentName { get; set; }

    public int? ProductIconsId { get; set; }

    public string? Thumbnail { get; set; }

    public decimal? ProductPrice { get; set; }

    public bool? IsNewRelease { get; set; }

    public int? SubCategoriesId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ProductIcon? ProductIcons { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<RegisterFlashSaleProduct> RegisterFlashSaleProducts { get; set; } = new List<RegisterFlashSaleProduct>();

    public virtual SubCategory? SubCategories { get; set; }
}
