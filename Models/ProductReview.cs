using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class ProductReview
{
    public int ProductReviewId { get; set; }

    public string? UserId { get; set; }

    public int? ProductId { get; set; }

    public string? ProductReviewTitle { get; set; }

    public string? ProductReviewContent { get; set; }

    public DateTime? ProductReviewTime { get; set; }

    public double? ProductReviewRate { get; set; }

    public string? ProductSizeName { get; set; }

    public virtual Product? Product { get; set; }

    public virtual UserAccount? User { get; set; }
}
