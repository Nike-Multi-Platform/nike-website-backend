using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class TempImportsProduct
{
    public int TempId { get; set; }

    public int? ReceiptId { get; set; }

    public int? ProductId { get; set; }

    public int? ProductSizeId { get; set; }

    public decimal? ImportPrice { get; set; }

    public int? Quantity { get; set; }

    public decimal? TotalPrice { get; set; }
}
