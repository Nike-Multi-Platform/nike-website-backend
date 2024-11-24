using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int? ProductParentId { get; set; }

    public string? ProductMoreInfo { get; set; }

    public string? ProductImg { get; set; }

    public string? ProductSizeAndFit { get; set; }

    public string? ProductStyleCode { get; set; }

    public string? ProductColorShown { get; set; }

    public string? ProductDescription { get; set; }

    public string? ProductDescription2 { get; set; }

    public decimal? SalePrices { get; set; }

    public int? Sold { get; set; }

    public int? TotalStock { get; set; }

    public virtual ICollection<GoodsReceiptDetail> GoodsReceiptDetails { get; set; } = new List<GoodsReceiptDetail>();

    public virtual ICollection<ProductImg> ProductImgs { get; set; } = new List<ProductImg>();

    public virtual ProductParent? ProductParent { get; set; }

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();

    public virtual ICollection<UserFavoriteProduct> UserFavoriteProducts { get; set; } = new List<UserFavoriteProduct>();
}
