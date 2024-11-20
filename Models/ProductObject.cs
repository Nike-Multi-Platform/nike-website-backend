using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class ProductObject
{
    public int ProductObjectId { get; set; }

    public string? ProductObjectName { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
