using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class ProductIcon
{
    public int ProductIconsId { get; set; }

    public string? IconName { get; set; }

    public string? Thumbnail { get; set; }

    public virtual ICollection<ProductParent> ProductParents { get; set; } = new List<ProductParent>();
}
