using nike_website_backend.Dtos;
using nike_website_backend.Helpers;
using nike_website_backend.Models;
namespace nike_website_backend.Interfaces
{
    public interface IFavoriteRepository
    {
        Task<Response<Boolean>> addToFavorite(String userId, int product_id);
        Task<Response<Boolean>> removeFromFavorite(int id);
        Task<Response<List<UserFavoriteProductsDto>>> getFavorites(string userId, int page, int limit);
    }
}
