using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class UserOrder
{
    public int UserOrderId { get; set; }

    public int? UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public int? IsProcessed { get; set; }

    public string? PaymentMethod { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? IsReviewed { get; set; }

    public virtual ICollection<UserOrderProduct> UserOrderProducts { get; set; } = new List<UserOrderProduct>();
}
