namespace nike_website_backend.Helpers
{
    public class SearchFilterQueryObject
    {
        public int sub_categories_id { get; set; } = 0;
        public string searchText { get; set; } = "";
        public List<int>? productObjectId { get; set; } = null;

        // "Black", "Blue", "Brown", "Green", "Grey", "White", "Yellow"
        public List<string>? product_color_shown { get; set; } = null; // Null for no filter
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 1000000000;
        // Sắp xếp

        // price , sold
        public string SortBy { get; set; } = "price";
        public bool IsSortAscending { get; set; } = true;

        // Phân trang
        public int Page { get; set; } = 1;
        public byte PageSize { get; set; } = 10;
    }
}
