using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class ReturnRequest
{
    public int ReturnRequestId { get; set; }

    public int RequestTypeId { get; set; }

    public string? ResolverId { get; set; }

    public int UserOrderId { get; set; }

    public string ReturnRequestReason { get; set; } = null!;

    public int StatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual RequestType RequestType { get; set; } = null!;

    public virtual ICollection<ReturnRequestImg> ReturnRequestImgs { get; set; } = new List<ReturnRequestImg>();

    public virtual UserOrder UserOrder { get; set; } = null!;
}
