namespace nike_website_backend.Dtos
{
    public class SubCategoryDto
    {
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public int? CategoryId { get; set; } // Mã danh mục cha để truy vấn các danh mục con liên quan
    }
}