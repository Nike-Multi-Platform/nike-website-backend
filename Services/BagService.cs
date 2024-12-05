using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using nike_website_backend.Dtos;
using nike_website_backend.Interfaces;
using nike_website_backend.Models;
using System.Net.WebSockets;
using System.Text.RegularExpressions;

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
                    response.Data = false;
                    return response;
                } else if (amount + bagItems.Amount > 10) {
                    response.StatusCode = 400;
                    response.Message = $"Products in the bag can only have a maximum of 10 quantities.";
                    response.Data = false;
                    return response;
                } else
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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = $"Error occurred while saving: {ex.Message}";
                response.Data = false;
                return response;
            }
            response.StatusCode = 200;
            response.Message = "Product added to bag successfully";
            response.Data = true;
            return response;

        }

        public async Task<Response<List<BagDto>>> getBag(String userId)
        {
            Response<List<BagDto>> res = new Response<List<BagDto>>();
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var currentDate = DateTime.Now;
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime localCurrentDate = TimeZoneInfo.ConvertTime(currentDate, localTimeZone);
            FlashSaleTimeFrame flashSaleTimeFrame = null;
            var flashSale = await _context.FlashSales.Where(f => f.StartedAt <= localCurrentDate && f.EndedAt > localCurrentDate && f.Status.Equals("active") ).FirstOrDefaultAsync();

            if (flashSale != null)
            {
     
            
                flashSaleTimeFrame = await _context.FlashSaleTimeFrames.Where(t => t.FlashSaleId == flashSale.FlashSaleId && t.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            }
            //p.SubCategories.Categories.ProductObject.ProductObjectName + "'s " + p.SubCategories.Categories.CategoriesName

            var query = _context.Bags.Where(b => b.UserId == userId).Select(b=> 
            new BagDto
            {
                bagId = b.BagId,
                userId = b.UserId,
                product_size_id = b.ProductSizeId,
                ProductSizeName = b.ProductSize.Size.SizeName,
                amount = b.Amount,
                is_selected = b.IsSelected,
                details = new ProductDto
                {
                   ProductId = b.ProductSize.ProductId,
                   ProductName = b.ProductSize.Product.ProductParent.ProductParentName,
                  
                   ProductParentId = b.ProductSize.Product.ProductParentId,
                   ProductImage = b.ProductSize.Product.ProductImg,
                   categoryWithObjectName = b.ProductSize.Product.ProductParent.SubCategories.Categories.ProductObject.ProductObjectName + "'s " + b.ProductSize.Product.ProductParent.SubCategories.Categories.CategoriesName,
                   stock = flashSaleTimeFrame != null &&
                         b.ProductSize.Product.ProductParent.RegisterFlashSaleProducts
                            .Any(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId &&
                                      r.Quantity - r.Sold > 0) ? (int)(b.ProductSize.Product.ProductParent.RegisterFlashSaleProducts.FirstOrDefault(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId).Quantity - b.ProductSize.Product.ProductParent.RegisterFlashSaleProducts.FirstOrDefault(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId).Sold) :  b.ProductSize.Soluong,
                   length = b.ProductSize.Product.ProductParent.Length,
                   weight = b.ProductSize.Product.ProductParent.Weight,
                   width = b.ProductSize.Product.ProductParent.Width,
                   height = b.ProductSize.Product.ProductParent.Height,
                   price = b.ProductSize.Product.ProductParent.ProductPrice,
                   salePrice = b.ProductSize.Product.SalePrices,
                   finalPrice = flashSaleTimeFrame != null &&
                         b.ProductSize.Product.ProductParent.RegisterFlashSaleProducts
                            .Any(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId &&
                                      r.Quantity - r.Sold > 0)
            ? b.ProductSize.Product.ProductParent.RegisterFlashSaleProducts.FirstOrDefault(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId).FlashSalePrice : b.ProductSize.Product.SalePrices > 0 ? b.ProductSize.Product.SalePrices : b.ProductSize.Product.ProductParent.ProductPrice,
                },
                RegisterFlashSaleProduct = flashSaleTimeFrame != null ? b.ProductSize.Product.ProductParent.RegisterFlashSaleProducts
            .Where(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId && r.Quantity - r.Sold > 0)
            .Select(r => new RegisterFlashSaleProductDTO
            {
                RegisterFlashSaleProduct1 = r.RegisterFlashSaleProduct1,
                Quantity = r.Quantity,
                Sold = r.Sold,
                FlashSaleTimeFrameId = r.FlashSaleTimeFrameId,
                started_at = r.FlashSaleTimeFrame.StartedAt,
                ended_at = r.FlashSaleTimeFrame.EndedAt,
                status = r.FlashSaleTimeFrame.Status
            })
            .FirstOrDefault() : null,
                

            }).AsNoTracking().AsQueryable();

            var bags = await query.ToListAsync();
            res.StatusCode = 200;
            res.Message = "Lấy dữ liệu thành công";
            res.Data = bags;
            return res;
        }
        public async Task<Response<DiscountVoucher>> applyVoucher(string userId, string promoCode)
        {
            Response<DiscountVoucher> response = new Response<DiscountVoucher>();
         
            var currentDate = DateTime.Now;
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime localCurrentDate = TimeZoneInfo.ConvertTime(currentDate, localTimeZone);
            var discountVoucher = await _context.DiscountVouchers.Where(v => v.VoucherCode == promoCode && v.StartedAt <= localCurrentDate && v.EndedAt > localCurrentDate && v.Quantity > 0).FirstOrDefaultAsync();
   
           
            if (discountVoucher == null)
            {
                response.StatusCode = 404;
                response.Message = "Voucher not available";
                response.Data = null;
                return response;
            }
   
            var usage = await _context.UserDiscountVouchers.Where(v => v.UserId == userId && v.DiscountVoucherId == discountVoucher.DiscountVoucherId).FirstOrDefaultAsync();
            if (usage != null)
            {
                if(usage.TotalUsed == discountVoucher.Usage)
                {
                    response.StatusCode = 404;
                    response.Message = "You have used up all your vouchers.";
                    response.Data = null;
                    return response;
                }
            }
            response.StatusCode = 200;
            response.Message = "Voucher applied Successfully";
            response.Data = discountVoucher;
            return response;

        }
        public async Task<Response<Boolean>> removeBagItem (int bag_id)
        {
            Response<Boolean> res = new Response<Boolean>();

            var query = _context.Bags.Where(b=>b.BagId == bag_id).AsQueryable();

            var bagItem = await query.FirstOrDefaultAsync();
            if(bagItem == null)
            {
                res.StatusCode = 404;
                res.Message = "Item Not found !!!";
                res.Data = false; 
                return res;
            }
            _context.Bags.Remove(bagItem);
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
            res.Message = "Product removed from bag successfully";
            res.Data = true;
            return res;

            

        }
        public async Task<Response<Boolean>> updateItemQuantity (int bag_id, int quantity)
        {
            Response<Boolean> res = new Response<Boolean>();
            var bagItem = await _context.Bags
            .Where(b => b.BagId == bag_id)
            .Select(b => new BagDto
            {
               bagId = b.BagId,
                userId =  b.UserId,
                product_size_id = b.ProductSizeId,
               amount =  b.Amount,
                is_selected = b.IsSelected,
                details = new ProductDto
                {
                    stock = b.ProductSize.Soluong
                }// Số lượng tồn kho của sản phẩm
            })
            .FirstOrDefaultAsync();
            if (bagItem == null)
            {
                res.StatusCode = 404;
                res.Message = "Item Not found !!!";
                res.Data = false;
                return res;
            }
            if (quantity > bagItem.details.stock)
            {
                res.StatusCode = 400;
                res.Message = "Not enough stock to increase quantity.";
                res.Data = false;
                return res;
            }
        

            if (quantity < 1)
            {
                res.StatusCode = 200;
                res.Message = "Quantity is already at minimum. No decrease applied.";
                res.Data = true;
                return res;
            }
            bagItem.amount = quantity;


            var bagToUpdate = await _context.Bags.FirstOrDefaultAsync(b => b.BagId == bag_id);
            if (bagToUpdate != null)
            {
                bagToUpdate.Amount = (int)bagItem.amount;
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
            }
            res.StatusCode = 200;
            res.Message = "Item quantity updated successfully.";
            res.Data = true;
            return res;
        }
        public async Task<Response<Boolean>> updateSelected (int bag_id, Boolean isSelected)
        {
            Response<Boolean> res= new Response<Boolean>();
            var bagItem = await _context.Bags.Where(b=>b.BagId == bag_id).FirstOrDefaultAsync();
            if (bagItem == null)
            {

                res.StatusCode = 404;
                res.Message = "Item Not found !!!";
                res.Data = false;
                return res;
            }
            bagItem.IsSelected = isSelected;
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
            res.Message = "Item selection updated successfully";
            res.Data = true;
            return res;
        }
        private int ExtractSizeNumber(string sizeName)
        {
            var numberPart = Regex.Match(sizeName, @"\d+").Value;
            return int.TryParse(numberPart, out var sizeNumber) ? sizeNumber : int.MaxValue;
        }

        public async Task<Response<List<ProductSizeDto>>> getProductSizes(int product_id)
        {
            Response<List<ProductSizeDto>> response = new Response<List<ProductSizeDto>>();

          
            var query = _context.ProductSizes.Where(s => s.ProductId == product_id).Select(s => new ProductSizeDto
            {
                ProductSizeId = s.ProductSizeId,
                Quantity = s.Soluong,
                SizeDto = new SizeDto
                {
                    SizeId = s.SizeId,
                    SizeName = s.Size.SizeName
                }
            }).AsQueryable();
            var sizeOrder = new Dictionary<string, int>
            {
                { "XS", 1 },
                { "S", 2 },
                { "M", 3 },
                { "L", 4 },
                { "XL", 5 },
                { "XXL", 6 },
                { "XXXL", 7 },
            };
            var sizes = await query.ToListAsync();
            var isAllOverSize = sizes.All(s => s.SizeDto.SizeName.Contains("oversize", StringComparison.OrdinalIgnoreCase));

            if (isAllOverSize)
            {
               
                response.StatusCode = 200;
                response.Message = "Lấy dữ liệu thành công";
                response.Data = sizes;
                return response;
            }else
            {
                sizes = sizes.OrderBy(s =>
                 sizeOrder.ContainsKey(s.SizeDto.SizeName)
                     ? sizeOrder[s.SizeDto.SizeName]
                     : ExtractSizeNumber(s.SizeDto.SizeName)
             ).ToList();
                response.StatusCode = 200;
                response.Message = "Lấy dữ liệu thành công";
                response.Data = sizes;
                return response;
            }



        }
        public async Task<Response<Boolean>> updateSize(int bag_id,String userId, int product_size_id)
        {
            Response<Boolean> res = new Response<Boolean>();
            var bagItem = await _context.Bags.Where(b=> b.BagId == bag_id).FirstOrDefaultAsync();
            if(bagItem == null)
            {
                res.StatusCode = 404;
                res.Message = "Item Not found !!!";
                res.Data = false;
                return res;
            }
            var productSize = await _context.ProductSizes.Where(b=> b.ProductSizeId == product_size_id).FirstOrDefaultAsync();
            if (productSize == null) {
                res.StatusCode = 404;
                res.Message = "Size Not found !!!";
                res.Data = false;
                return res;
            }
            if(productSize.Soluong == 0)
            {
                res.StatusCode = 404;
                res.Message = "Not enough stock to update size. !!!";
                res.Data = false;
                return res;
            }
            var existingItem = await _context.Bags
             .Where(b => b.UserId == userId && b.ProductSizeId == product_size_id)
             .FirstOrDefaultAsync();
            if (existingItem == null) {
                if (bagItem.Amount > productSize.Soluong)
                {
                    bagItem.ProductSizeId = product_size_id;
                    bagItem.Amount = (int)productSize.Soluong;
                }
                else
                {
                    bagItem.ProductSizeId = product_size_id;
                }
            }else
            {
                int combinedAmount = bagItem.Amount + existingItem.Amount;

                if (combinedAmount > productSize.Soluong)
                {
                    
                    existingItem.Amount = (int)productSize.Soluong;
                }
                else if (combinedAmount <= 10)
                {
           
                    existingItem.Amount += bagItem.Amount;
                }
                else
                {
           
                    existingItem.Amount = 10;
                }
                _context.Bags.Remove(bagItem);
            }
        

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
            res.Message = "Item size updated successfully";
            res.Data = true;
            return res;
        }

        public async Task<Response<int>> getTotalAmount(String userId)
        {
            Response<int> res = new Response<int>();

            var totalAmount = await _context.Bags.AsNoTracking()
                .Where(p => p.UserId == userId && p.ProductSize.Soluong > 0)
                .CountAsync();


            res.StatusCode = 200;
            res.Message = "Data fetched successfully.";
            res.Data = totalAmount;
            return res;
        }
    }
}
