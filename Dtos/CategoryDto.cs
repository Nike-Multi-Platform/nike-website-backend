namespace nike_website_backend.Dtos
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<SubCategoryDto> SubCategories { get; set; }
    }
}