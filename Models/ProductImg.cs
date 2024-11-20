using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class ProductImg
{
    public int ProductImgId { get; set; }

    public int? ProductId { get; set; }

    public string? ProductImgFileName { get; set; }

    public virtual Product? Product { get; set; }
}
