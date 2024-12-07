namespace nike_website_backend.Dtos
{
    public class ReviewResponse
    {
        public String? UserId { get; set; }
        public int? UserOrderId { get; set; }
        public List<ReviewDTO>? Reviews { get; set; }
    }
}
