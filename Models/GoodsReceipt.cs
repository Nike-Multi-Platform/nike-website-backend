using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class GoodsReceipt
{
    public int GoodsReceiptId { get; set; }

    public int? SupplierId { get; set; }

    public string? UserId { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsHandle { get; set; }

    public virtual ICollection<GoodsReceiptDetail> GoodsReceiptDetails { get; set; } = new List<GoodsReceiptDetail>();

    public virtual Supplier? Supplier { get; set; }

    public virtual UserAccount? User { get; set; }
}
