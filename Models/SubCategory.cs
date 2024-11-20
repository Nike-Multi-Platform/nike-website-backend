using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class SubCategory
{
    public int SubCategoriesId { get; set; }

    public string? SubCategoriesName { get; set; }

    public int? CategoriesId { get; set; }

    public virtual Category? Categories { get; set; }

    public virtual ICollection<ProductParent> ProductParents { get; set; } = new List<ProductParent>();
}
