using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class DiscountVoucher
{
    public int DiscountVoucherId { get; set; }

    public string VoucherName { get; set; } = null!;

    public string VoucherCode { get; set; } = null!;

    public string DiscountType { get; set; } = null!;

    public int Usage { get; set; }

    public int Quantity { get; set; }

    public int MinOrderValue { get; set; }

    public long DiscountValue { get; set; }

    public long DiscountMaxValue { get; set; }

    public string Description { get; set; } = null!;

    public DateTime StartedAt { get; set; }

    public DateTime EndedAt { get; set; }

    public virtual ICollection<UserDiscountVoucher> UserDiscountVouchers { get; set; } = new List<UserDiscountVoucher>();
}
