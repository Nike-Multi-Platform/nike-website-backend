using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class UserWallet
{
    public int UserWalletId { get; set; }

    public string? UserId { get; set; }

    public long? Balance { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual UserAccount? User { get; set; }

    public virtual ICollection<UserWalletTransaction> UserWalletTransactions { get; set; } = new List<UserWalletTransaction>();
}
