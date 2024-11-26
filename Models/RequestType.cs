using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class RequestType
{
    public int RequestTypeId { get; set; }

    public string? RequestTypeName { get; set; }

    public virtual ICollection<ReturnRequest> ReturnRequests { get; set; } = new List<ReturnRequest>();
}
