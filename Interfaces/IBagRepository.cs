using nike_website_backend.Dtos;
using nike_website_backend.Helpers;
using nike_website_backend.Models;
namespace nike_website_backend.Interfaces
{
    public interface IBagRepository
    {
        Task<Response<Boolean>> addToBag(String user_id, int product_size_id, int amount);
        Task<Response<List<BagDto>>> getBag(String userId);
        Task<Response<Boolean>> removeBagItem(int bag_id);
        Task<Response<Boolean>> updateItemQuantity(int bag_id, string type);
        Task<Response<Boolean>> updateSelected(int bag_id, Boolean isSelected);
        Task<Response<Boolean>> updateSize(int bag_id, int product_size_id);
        Task<Response<List<ProductSizeDto>>> getProductSizes(int product_id);
    }
}
