namespace nike_website_backend.Dtos
{
    public class ProductReviewDto
    {
        public int ProductReviewId { get; set; }
        public string ProductReviewTitle { get; set; }
        public string ProductReviewContent { get; set; }
        public double? ProductRating { get; set; }
        public DateTime? ProductReviewDate { get; set; }

        // public string UserId { get; set; }
        // ... more properties
        
    }
}