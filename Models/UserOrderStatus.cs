using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class UserOrderStatus
{
    public int UserOrderStatusId { get; set; }

    public string? UserOrderStatusName { get; set; }

    public virtual ICollection<UserOrder> UserOrders { get; set; } = new List<UserOrder>();
}
