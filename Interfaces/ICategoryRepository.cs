using nike_website_backend.Dtos;

namespace nike_website_backend.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Response<CategoryDto>> getSubCategoriesByCategoryId(int categoryId);
        Task<Response<List<CategoryDto>>> getCategoriesByCatAndObject(int categoryId, int productObjectId);
    }
}