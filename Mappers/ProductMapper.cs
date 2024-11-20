using nike_website_backend.Dtos;
using nike_website_backend.Models;

namespace nike_website_backend.Mappers
{
    public static class ProductMapper
    {
        public static ProductObjectDto convertToProductObjectDto(this ProductObject productObject)
        {
            return new ProductObjectDto
            {
                ProductId = productObject.ProductObjectId,
                ProductName = productObject.ProductObjectName,
            };
        }
    }
}