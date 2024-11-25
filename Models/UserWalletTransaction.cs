using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class UserWalletTransaction
{
    public int WalletTransactionId { get; set; }

    public int? UserWalletId { get; set; }

    public string? TransactionType { get; set; }

    public long? Amount { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string? Description { get; set; }

    public virtual UserWallet? UserWallet { get; set; }
}
