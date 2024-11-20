using nike_website_backend.Dtos;
using nike_website_backend.Helpers;

namespace nike_website_backend.Interfaces
{
    public interface IProductRepository
    {
        Task<Response<List<ProductObjectDto>>> GetProductObjects();
        Task<Response<List<ProductParentDto>>> GetProductParents(int subCategoryId, QueryObject queryObject);
        Task<Response<ProductParentDto>> GetProductParentDetail(int productParentId);
        Task<Response<ProductDetailDto>> GetProductDetail(int productId);
    }
}