using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class ReturnRequestImg
{
    public int ReturnRequestImgsId { get; set; }

    public int? ReturnRequestId { get; set; }

    public string? ImgUrl { get; set; }

    public virtual ReturnRequest? ReturnRequest { get; set; }
}
