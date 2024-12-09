using nike_website_backend.Dtos;
using nike_website_backend.Models;

namespace nike_website_backend.Interfaces
{
    public interface IUserOrderRepository
    {
        Task<Response<List<UserOrderDTO>>> GetUserOrder(string userId, int userOrderStatusId, string? text, int limit, int page);
        Task<ResponseGHN<dynamic>> GetOrderDetailGHNAsync(string orderCode);
        Task<Response<List<UserOrderStatus>>> GetStatusList();
        Task<Response<UserOrderDTO>> GetOrderDetail(int userOrderId);
        Task<Response<Boolean>> WriteReviews(ReviewResponse review);
        Task<Response<Boolean>> SendRequest(RequestDTO request);
    }
}
