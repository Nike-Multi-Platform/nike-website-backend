using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class Category
{
    public int CategoriesId { get; set; }

    public string? CategoriesName { get; set; }

    public int? ProductObjectId { get; set; }

    public virtual ProductObject? ProductObject { get; set; }

    public virtual ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}
