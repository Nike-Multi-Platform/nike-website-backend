namespace nike_website_backend.Dtos
{
    public class ReviewDTO
    {
        public int? ProductId { get; set; }
        public string? Title {  get; set; }
        public int? Rating { get; set; }
        public string? Review { get; set; }
        public string? SizeName { get; set; }
    }
}
