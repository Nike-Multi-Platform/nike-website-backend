using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class Bag
{
    public int BagId { get; set; }

    public string UserId { get; set; } = null!;

    public int ProductSizeId { get; set; }

    public int Amount { get; set; }

    public bool IsSelected { get; set; }

    public virtual ProductSize ProductSize { get; set; } = null!;

    public virtual UserAccount User { get; set; } = null!;
}
