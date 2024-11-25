using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class UserAccount
{
    public string UserId { get; set; } = null!;

    public string? Password { get; set; }

    public string UserUsername { get; set; } = null!;

    public string UserGender { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public string UserPhoneNumber { get; set; } = null!;

    public string UserAddress { get; set; } = null!;

    public string UserFirstName { get; set; } = null!;

    public string UserLastName { get; set; } = null!;

    public string UserUrl { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Bag> Bags { get; set; } = new List<Bag>();

    public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();

    public virtual ICollection<HistorySearch> HistorySearches { get; set; } = new List<HistorySearch>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<UserDiscountVoucher> UserDiscountVouchers { get; set; } = new List<UserDiscountVoucher>();

    public virtual ICollection<UserFavoriteProduct> UserFavoriteProducts { get; set; } = new List<UserFavoriteProduct>();

    public virtual ICollection<UserOrder> UserOrders { get; set; } = new List<UserOrder>();

    public virtual ICollection<UserWallet> UserWallets { get; set; } = new List<UserWallet>();
}
