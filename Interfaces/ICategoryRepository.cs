using nike_website_backend.Dtos;

namespace nike_website_backend.Interfaces
{
    public interface ICategoryRepository
    {
         Task<Response<CategoryDto>> getSubCategoriesByCategoryId(int categoryId);
    }
}