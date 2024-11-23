using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string? SupplierName { get; set; }

    public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();
}
