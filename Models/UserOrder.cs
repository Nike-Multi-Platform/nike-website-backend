using System;
using System.Collections.Generic;

namespace nike_website_backend.Models;

public partial class UserOrder
{
    public int UserOrderId { get; set; }

    public string UserId { get; set; } = null!;

    public int UserOrderStatusId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? OrderCode { get; set; }

    public string? OrderCodeReturn { get; set; }

    public DateTime? ReturnExpirationDate { get; set; }

    public int IsReviewed { get; set; }

    public int IsProcessed { get; set; }

    public string? VouchersApplied { get; set; }

    public byte? IsCanceledBy { get; set; }

    public decimal ShippingFee { get; set; }

    public int TotalQuantity { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal DiscountPrice { get; set; }

    public decimal FinalPrice { get; set; }

    public string GhnService { get; set; } = null!;

    public string? TransactionCode { get; set; }

    public virtual ICollection<ReturnRequest> ReturnRequests { get; set; } = new List<ReturnRequest>();

    public virtual UserAccount User { get; set; } = null!;

    public virtual ICollection<UserOrderProduct> UserOrderProducts { get; set; } = new List<UserOrderProduct>();

    public virtual UserOrderStatus UserOrderStatus { get; set; } = null!;
}
