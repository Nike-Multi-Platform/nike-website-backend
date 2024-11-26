using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class HistorySearch
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string TextSearch { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual UserAccount User { get; set; } = null!;
}
