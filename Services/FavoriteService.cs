

using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using nike_website_backend.Dtos;
using nike_website_backend.Interfaces;
using nike_website_backend.Models;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using static Google.Apis.Requests.BatchRequest;

namespace nike_website_backend.Services
{
    public class FavoriteService : IFavoriteRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoriteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Boolean>> addToFavorite(String userId,int product_id)
        {
            Response<Boolean> res = new Response<Boolean>();
            var item = await  _context.UserFavoriteProducts.Where(p => p.UserId == userId && p.ProductId == product_id).FirstOrDefaultAsync();
            if (item != null)
            {
                res.StatusCode = 400;
                res.Message = "The product is already in the wishlist.";
                res.Data = false;
                return res;
            }
            var newItem = new UserFavoriteProduct
            {
                UserId = userId,
                ProductId = product_id,
            };

            await _context.UserFavoriteProducts.AddAsync(newItem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                res.Message = $"Error occurred while saving: {ex.Message}";
                res.Data = false;
                return res;
            }

            res.StatusCode = 200;
            res.Message = "Added to favorites successfully";
            res.Data = true;
            return res;


        }

        public async Task<Response<Boolean>> removeFromFavorite(int id)
        {
            Response<Boolean> res = new Response<Boolean>();
            var item = await _context.UserFavoriteProducts.Where(p=>p.Id == id).FirstOrDefaultAsync();
            if (item == null) {
                res.StatusCode = 400;
                res.Message = "Item not found";
                res.Data = false;
                return res;
            }
            
            _context.UserFavoriteProducts.Remove(item);
            await _context.SaveChangesAsync();
            res.StatusCode=200;
            res.Message = "Removed from favorites successfully";
            res.Data = true;
            return res;
        }

        public async Task<Response<List<UserFavoriteProductsDto>>> getFavorites(string userId,int page,int limit)
        {
            Response<List<UserFavoriteProductsDto>> res = new Response<List<UserFavoriteProductsDto>>();
            var offset = (page - 1) * limit;
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var currentDate = DateTime.Now;
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime localCurrentDate = TimeZoneInfo.ConvertTime(currentDate, localTimeZone);
            FlashSaleTimeFrame flashSaleTimeFrame = null;
            var flashSale = await _context.FlashSales.Where(f => f.StartedAt <= localCurrentDate && f.EndedAt > localCurrentDate && f.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            if (flashSale != null)
            {
                flashSaleTimeFrame = await _context.FlashSaleTimeFrames.Where(t => t.FlashSaleId == flashSale.FlashSaleId && t.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            }
            var query = _context.UserFavoriteProducts.Where(p => p.UserId == userId).Select(p => new UserFavoriteProductsDto
            {
                Id = p.Id,
                ProductId = p.ProductId,
                Product = new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.Product.ProductParent.ProductParentName,
                    categoryWithObjectName = p.Product.ProductParent.SubCategories.Categories.ProductObject.ProductObjectName + "'s " + p.Product.ProductParent.SubCategories.Categories.CategoriesName,
                    ProductImage = p.Product.ProductImg,
                    ProductParentId = p.Product.ProductParentId,
                    price = p.Product.ProductParent.ProductPrice,
                    stock = p.Product.ProductParent.Products.Sum(t => t.ProductSizes.Sum(s => s.Soluong)),
                    salePrice = flashSaleTimeFrame != null &&
                         p.Product.ProductParent.RegisterFlashSaleProducts
                            .Any(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId &&
                                      r.Quantity - r.Sold > 0) ? p.Product.ProductParent.RegisterFlashSaleProducts.FirstOrDefault(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId).FlashSalePrice : p.Product.SalePrices > 0 ? p.Product.SalePrices : 0,
                    finalPrice = flashSaleTimeFrame != null &&
                         p.Product.ProductParent.RegisterFlashSaleProducts
                            .Any(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId &&
                                      r.Quantity - r.Sold > 0)
            ? p.Product.ProductParent.RegisterFlashSaleProducts.FirstOrDefault(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId).FlashSalePrice : p.Product.SalePrices > 0 ? p.Product.SalePrices : p.Product.ProductParent.ProductPrice,
                
            }
            }).OrderByDescending(p=>p.Id).AsNoTracking().AsQueryable();

            var favorites = await query.Skip(offset).Take(limit).ToListAsync();
            var count = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)count / limit);
            res.StatusCode = 200;
            res.Message = "Lấy dữ liệu thành công";
            res.Data = favorites;
            res.TotalPages = totalPages;
            return res;

        }
        
      
    }
}
