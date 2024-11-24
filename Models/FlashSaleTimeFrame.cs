using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class FlashSaleTimeFrame
{
    public int FlashSaleTimeFrameId { get; set; }

    public int FlashSaleId { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual FlashSale FlashSale { get; set; } = null!;

    public virtual ICollection<RegisterFlashSaleProduct> RegisterFlashSaleProducts { get; set; } = new List<RegisterFlashSaleProduct>();
}
