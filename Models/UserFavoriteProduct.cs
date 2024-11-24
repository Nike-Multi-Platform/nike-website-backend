using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class UserFavoriteProduct
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual UserAccount User { get; set; } = null!;
}
