namespace nike_website_backend.Dtos
{
    public class UserOrderDTO
    {
        public int? UserOrderId { get; set; }
        public string? UserId { get; set; }
        public int? UserOrderStatusId { get; set; }
        public string? UserOrderStatusName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? Email {  get; set; }
        public string? PhoneNumber { get; set; }
        public string? PaymentMethod { get; set; }
        public string? OrderCode { get; set; }
        public string? OrderCodeReturn { get; set; }
        public DateTime? return_expiration_date { get; set; }
        public int? IsReviewed { get; set; }
        public int? IsProcessed { get; set; }
        public string? VoucherApplied { get; set; }
        public int? IsCanceledBy { get; set; }
        public decimal? ShippingFee { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? DiscountPrice {  get; set; }
        public decimal? FinalPrice { get; set; }
        public string? GHNService {  get; set; }
        public List<BagDto>? bagItems { get; set; }
        public List<UserOrderItemDTO>? userOrderItems { get; set; } 
        public GHNDto? serviceLogs { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Boolean? isSendCancelRequest { get; set; }
        public Boolean? isSendRefundRequest {  get; set; }
     
    }
}
