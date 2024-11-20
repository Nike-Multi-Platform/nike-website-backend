using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class GoodsReceiptDetail
{
    public int GoodsReceiptDetailsId { get; set; }

    public int GoodReceiptId { get; set; }

    public int ProductId { get; set; }

    public int ProductSizeId { get; set; }

    public int? Quantity { get; set; }

    public decimal? ImportPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual GoodsReceipt GoodReceipt { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ProductSize ProductSize { get; set; } = null!;
}
