using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class FlashSale
{
    public int FlashSaleId { get; set; }

    public string? FlashSaleName { get; set; }

    public string? Thumbnail { get; set; }

    public string Status { get; set; } = null!;

    public DateTime StartedAt { get; set; }

    public DateTime EndedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<FlashSaleTimeFrame> FlashSaleTimeFrames { get; set; } = new List<FlashSaleTimeFrame>();
}
