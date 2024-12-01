using Microsoft.EntityFrameworkCore;
using nike_website_backend.Dtos;
using nike_website_backend.Interfaces;
using nike_website_backend.Models;

namespace nike_website_backend.Services
{
    public class BagService:IBagRepository
    {
        private readonly ApplicationDbContext _context;

        public BagService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Boolean>> addToBag(String user_id, int product_size_id, int amount)
        {
            Response<Boolean> response = new Response<Boolean>();
            var productSize = await _context.ProductSizes.Where(p => p.ProductSizeId == product_size_id).FirstOrDefaultAsync();
            if(productSize == null)
            {
                response.StatusCode = 404;
                response.Message = "Product Size not found !!!";
                response.Data = false;
                return response;
            }
            var bagItems = await _context.Bags.Where(p => p.UserId == user_id && p.ProductSizeId == product_size_id).FirstOrDefaultAsync();

            if (bagItems != null)
            {
                if (productSize.Soluong < amount + bagItems.Amount)
                {
                    response.StatusCode = 400;
                    response.Message = $"Not enough stock. Only {productSize.Soluong} items available.";

                }else
                {
                    bagItems.Amount += amount;
                    _context.Bags.Update(bagItems);

                }
            }else
            {
                var newBagItem = new Bag
                {
                    UserId = user_id,
                    ProductSizeId = product_size_id,
                    Amount = amount,

                };
                await _context.Bags.AddAsync(newBagItem);
            }

            await _context.SaveChangesAsync();
            response.StatusCode = 200;
            response.Message = "Product added to cart successfully";
            response.Data = true;
            return response;

        }

        //public async Task<Response<List<BagDto>> getBag (String userId)
        //{
        //    Response<List<BagDto>> res = new Response<List<BagDto>> ();
            
        //}
    }
}
