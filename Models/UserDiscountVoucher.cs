using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class UserDiscountVoucher
{
    public int DiscountVoucherId { get; set; }

    public int UserId { get; set; }

    public int? Quantity { get; set; }

    public virtual DiscountVoucher DiscountVoucher { get; set; } = null!;

    public virtual UserAccount User { get; set; } = null!;
}
