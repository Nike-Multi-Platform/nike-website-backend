using nike_website_backend.Dtos;
using nike_website_backend.Helpers;
using nike_website_backend.Models;
namespace nike_website_backend.Interfaces
{
    public interface IProductRepository
    {
        Task<Response<List<ProductObjectDto>>> GetProductObjects();
        Task<Response<List<ProductParentDto>>> GetProductParents(QueryObject queryObject);
        Task<Response<ProductParentDto>> GetProductParentDetail(int productParentId);
        Task<Response<ProductDetailDto>> GetProductDetail(int productId);
        Task<Response<List<ProductIcon>>> GetIcons(int page,int limit);
        Task<Response<List<ProductParentDto>>> GetNewRelease(int page, int limit);
        Task<Response<List<ProductParentDto>>> GetProductByObjectID(int page, int limit, int objectId);
        Task<Response<List<ProductReviewDto>>> getReviewsOfColor(int productId, int page, int limit, string sortBy, double rating);
        Task<Response<List<ProductParentDto>>> getRecommendProductParent(string? userId, int limit); 
        Task<Response<List<ProductParentDto>>> SearchFilter(SearchFilterQueryObject queryObject);
    }
}