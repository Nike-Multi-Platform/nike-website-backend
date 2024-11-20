﻿using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class HistorySearch
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? TextSearch { get; set; }

    public virtual UserAccount? User { get; set; }
}
