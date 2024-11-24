using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class Bag
{
    public int BagId { get; set; }

    public string? UserId { get; set; }

    public int? ProductSizeId { get; set; }

    public int? Amount { get; set; }

    public virtual ProductSize? ProductSize { get; set; }

    public virtual UserAccount? User { get; set; }
}
