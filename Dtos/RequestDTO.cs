namespace nike_website_backend.Dtos
{
    public class RequestDTO
    {
        public int? UserOrderId { get; set; }
        public int? RequestTypeId { get; set; }
        public string? Reason { get; set; }
        public List<RequestImgsDTO>? RequestImages { get; set; }
    }
}
