using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class FlashSale
{
    public int FlashSaleId { get; set; }

    public int? ProductId { get; set; }

    public decimal? DiscountPercent { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public virtual Product? Product { get; set; }
}
