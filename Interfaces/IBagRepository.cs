using nike_website_backend.Dtos;
using nike_website_backend.Helpers;
using nike_website_backend.Models;
namespace nike_website_backend.Interfaces
{
    public interface IBagRepository
    {
        Task<Response<Boolean>> addToBag(String user_id, int product_size_id, int amount);
    }
}
