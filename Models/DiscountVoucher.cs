using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class DiscountVoucher
{
    public int DiscountVoucherId { get; set; }

    public string? VoucherName { get; set; }

    public int? Quantity { get; set; }

    public virtual ICollection<UserDiscountVoucher> UserDiscountVouchers { get; set; } = new List<UserDiscountVoucher>();
}
