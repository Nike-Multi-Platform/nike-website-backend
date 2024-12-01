namespace nike_website_backend.Helpers
{
    public class QueryObject
    {
        // Tìm kiếm
        public string ProductName { get; set; } = "";
        // Sắp xếp
        public string SortBy { get; set; } = "price";
        public bool IsSortAscending { get; set; } = true;

        // Phân trang
        public int Page { get; set; } = 1;
        public byte PageSize { get; set; } = 10;
    }
}