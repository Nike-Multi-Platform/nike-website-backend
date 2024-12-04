namespace nike_website_backend.Helpers
{
    public class QueryObject
    {
        // Thể loại
        public int SubCategoryId { get; set; } = 0; // 0 tất cả
        // Tìm kiếm
        public string ProductName { get; set; } = "";
        // Giới tính
        public string productObjectId { get; set; } = "-1"; // -1 tất cả, 1 nam, 2 nữ, 3 kid
        // Mức giá
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 1000000000;
        // Sắp xếp
        public string SortBy { get; set; } = "price";
        public bool IsSortAscending { get; set; } = true;

        // Phân trang
        public int Page { get; set; } = 1;
        public byte PageSize { get; set; } = 10;
    }
}