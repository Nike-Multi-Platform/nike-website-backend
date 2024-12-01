using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class UserDiscountVoucher
{
    public int DiscountVoucherId { get; set; }

    public string UserId { get; set; } = null!;

    public int TotalUsed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual DiscountVoucher DiscountVoucher { get; set; } = null!;

    public virtual UserAccount User { get; set; } = null!;
}
