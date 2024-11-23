using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class UserAccount
{
    public int UserId { get; set; }

    public string? UserUsername { get; set; }

    public string? UserPassword { get; set; }

    public string? UserGender { get; set; }

    public string? UserEmail { get; set; }

    public string? UserPhoneNumber { get; set; }

    public string? UserAddress { get; set; }

    public string? UserFirstName { get; set; }

    public string? UserLastName { get; set; }

    public int? UserMemberTier { get; set; }

    public int? UserPoint { get; set; }

    public string? UserUrl { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<Bag> Bags { get; set; } = new List<Bag>();

    public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();

    public virtual ICollection<HistorySearch> HistorySearches { get; set; } = new List<HistorySearch>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<UserDiscountVoucher> UserDiscountVouchers { get; set; } = new List<UserDiscountVoucher>();

    public virtual ICollection<UserFavoriteProduct> UserFavoriteProducts { get; set; } = new List<UserFavoriteProduct>();
}
