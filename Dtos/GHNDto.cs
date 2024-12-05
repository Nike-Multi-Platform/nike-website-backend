namespace nike_website_backend.Dtos
{
    public class GHNDto
    {
        public string? GhnStatus { get; set; }
        public string? GhnStatusDescription { get; set; }
        public string? UpdatedDate { get; set; }
        public List<LogEntry>? Logs { get; set; }

    }
}
