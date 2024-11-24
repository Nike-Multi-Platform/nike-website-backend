using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class RegisterFlashSaleProduct
{
    public int RegisterFlashSaleProduct1 { get; set; }

    public int FlashSaleTimeFrameId { get; set; }

    public int ProductParentId { get; set; }

    public long? OriginalPrice { get; set; }

    public long? FlashSalePrice { get; set; }

    public int? Quantity { get; set; }

    public long? Sold { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual FlashSaleTimeFrame FlashSaleTimeFrame { get; set; } = null!;

    public virtual ProductParent ProductParent { get; set; } = null!;
}
