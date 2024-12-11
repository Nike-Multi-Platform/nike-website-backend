using nike_website_backend.Dtos;
using nike_website_backend.Helpers;
using nike_website_backend.Models;

namespace nike_website_backend.Interfaces
{
    public interface IFlashSaleRepository
    {
        Task<Response<FlashSaleTimeFrameDto>> getActiveFlashSale(int limit);
        Task<Response<List<FlashSaleTimeFrame>>> getAvailableFlashSaleTimeFrame();
        Task<Response<List<ProductParentDto>>> getProductsByTimeFrameId(int timeFrameId, int page, int limit);
    }
}
