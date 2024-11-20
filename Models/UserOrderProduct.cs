using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class UserOrderProduct
{
    public int UserOrderId { get; set; }

    public int ProductSizeId { get; set; }

    public int? Amount { get; set; }

    public virtual ProductSize ProductSize { get; set; } = null!;

    public virtual UserOrder UserOrder { get; set; } = null!;
}
