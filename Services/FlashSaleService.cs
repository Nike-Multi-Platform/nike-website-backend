using nike_website_backend.Interfaces;
using nike_website_backend.Models;
using Microsoft.EntityFrameworkCore;
using nike_website_backend.Dtos;
namespace nike_website_backend.Services
{
    public class FlashSaleService: IFlashSaleRepository
    {
        private readonly ApplicationDbContext _context;
        public FlashSaleService(ApplicationDbContext context)
        {
            _context = context;
        }
 
        public async Task<Response<FlashSaleTimeFrameDto>> getActiveFlashSale(int limit){
            Response<FlashSaleTimeFrameDto> res = new Response<FlashSaleTimeFrameDto>();
            var currentDate = DateTime.Now;
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime localCurrentDate = TimeZoneInfo.ConvertTime(currentDate, localTimeZone);
            var flashSale = await _context.FlashSales
                 .Where(f => f.StartedAt <= localCurrentDate && f.EndedAt > localCurrentDate && f.Status.Equals("active")
                      
                       )
                 .FirstOrDefaultAsync();
            if(flashSale == null)
            {
                flashSale = await _context.FlashSales.Where(f=> f.StartedAt > localCurrentDate && f.Status.Equals("waiting")).OrderBy(f=>f.StartedAt).FirstOrDefaultAsync();
            }
            if (flashSale == null) {
                flashSale = await _context.FlashSales.Where(f => f.Status.Equals("ended")).OrderByDescending(f => f.EndedAt).FirstOrDefaultAsync();
            }
            if (flashSale == null)
            {
                res.StatusCode = 404;
                res.Message = "Không có sự kiện nào được diễn ra";
                res.Data = null;
                return res;
            }
          
            var query =  _context.FlashSaleTimeFrames.Where(t => t.FlashSaleId == flashSale.FlashSaleId && t.Status.Equals("active")).Select(t => new FlashSaleTimeFrameDto
            {
                FlashSaleId = t.FlashSaleId,
                FlashSaleTimeFrameId = t.FlashSaleTimeFrameId,
                StartedAt = t.StartedAt,
                EndedAt = t.EndedAt,
                Status = t.Status,
                FlashSale = t.FlashSale,
                Products = t.RegisterFlashSaleProducts
                    .OrderBy(r => r.Sold)
                    .Take(limit)
                    .Select(r => new ProductParentDto
                    {
                        ProductParentId = r.ProductParentId,
                        ProductParentName = r.ProductParent.ProductParentName,
                        ProductIconsId = r.ProductParent.ProductIconsId,
                        Thumbnail = r.ProductParent.Thumbnail,
                        ProductPrice = r.ProductParent.ProductPrice,
                        IsNew = r.ProductParent.IsNewRelease,
                        SubCategoriesId = r.ProductParent.SubCategoriesId,
                        CreatedAt = r.ProductParent.CreatedAt,
                        UpdatedAt = r.ProductParent.UpdatedAt,
                        salePrice = r.FlashSalePrice,
                        categoryWithObjectName = $"{r.ProductParent.SubCategories.Categories.ProductObject.ProductObjectName}'s {r.ProductParent.SubCategories.Categories.CategoriesName}",
                        sold = r.Sold,
                        quantity = r.Quantity,
                        quantityInStock = r.ProductParent.Products.Sum(t => t.ProductSizes.Sum(s => s.Soluong)),

                    }).ToList()
            }).AsNoTracking().AsQueryable();

            var flashSaleTimeFrame = await query.FirstOrDefaultAsync();
            
            if(flashSaleTimeFrame == null)
            {
                flashSaleTimeFrame = await _context.FlashSaleTimeFrames.Where(t => t.FlashSaleId == flashSale.FlashSaleId && t.Status.Equals("waiting")).OrderBy(t=>t.StartedAt).Select(
                    t=> new FlashSaleTimeFrameDto
                    {
                        FlashSaleId = t.FlashSaleId,
                        FlashSaleTimeFrameId = t.FlashSaleTimeFrameId,
                        StartedAt = t.StartedAt,
                        EndedAt = t.EndedAt,
                        Status = t.Status,
                        FlashSale = t.FlashSale,
                        Products = t.RegisterFlashSaleProducts
                    .OrderBy(r => r.Sold)
                    .Take(limit)
                    .Select(r => new ProductParentDto
                    {
                        ProductParentId = r.ProductParentId,
                        ProductParentName = r.ProductParent.ProductParentName,
                        ProductIconsId = r.ProductParent.ProductIconsId,
                        Thumbnail = r.ProductParent.Thumbnail,
                        ProductPrice = r.ProductParent.ProductPrice,
                        IsNew = r.ProductParent.IsNewRelease,
                        SubCategoriesId = r.ProductParent.SubCategoriesId,
                        CreatedAt = r.ProductParent.CreatedAt,
                        UpdatedAt = r.ProductParent.UpdatedAt,
                        salePrice = r.FlashSalePrice,
                        categoryWithObjectName = $"{r.ProductParent.SubCategories.Categories.ProductObject.ProductObjectName}'s {r.ProductParent.SubCategories.Categories.CategoriesName}",
                        sold = r.Sold,
                        quantityInStock = r.ProductParent.Products.Sum(t => t.ProductSizes.Sum(s => s.Soluong)),
                    }).ToList()
                    }
                ).AsNoTracking().FirstOrDefaultAsync();
            }
            if (flashSaleTimeFrame == null)
            {
                flashSaleTimeFrame = await _context.FlashSaleTimeFrames.Where(t => t.FlashSaleId == flashSale.FlashSaleId && t.Status.Equals("ended")).OrderByDescending(t => t.EndedAt).Select(
                    t => new FlashSaleTimeFrameDto
                    {
                        FlashSaleId = t.FlashSaleId,
                        FlashSaleTimeFrameId = t.FlashSaleTimeFrameId,
                        StartedAt = t.StartedAt,
                        EndedAt = t.EndedAt,
                        Status = t.Status,
                        FlashSale = t.FlashSale,
                        Products = t.RegisterFlashSaleProducts
                  .OrderBy(r => r.Sold)
                    .Take(limit)
                    .Select(r => new ProductParentDto
                    {
                        ProductParentId = r.ProductParentId,
                        ProductParentName = r.ProductParent.ProductParentName,
                        ProductIconsId = r.ProductParent.ProductIconsId,
                        Thumbnail = r.ProductParent.Thumbnail,
                        ProductPrice = r.ProductParent.ProductPrice,
                        IsNew = r.ProductParent.IsNewRelease,
                        SubCategoriesId = r.ProductParent.SubCategoriesId,
                        CreatedAt = r.ProductParent.CreatedAt,
                        UpdatedAt = r.ProductParent.UpdatedAt,
                        salePrice = r.FlashSalePrice,
                        categoryWithObjectName = $"{r.ProductParent.SubCategories.Categories.ProductObject.ProductObjectName}'s {r.ProductParent.SubCategories.Categories.CategoriesName}",
                        sold = r.Sold,
                        quantityInStock = r.ProductParent.Products.Sum(t => t.ProductSizes.Sum(s => s.Soluong)),
                    }).ToList()
                    }
                ).AsNoTracking().FirstOrDefaultAsync();
            }

            if (flashSaleTimeFrame == null)
            {
                res.StatusCode = 404;
                res.Message = "Không có khung giờ nào";
                res.Data = null;
                return res;
            }

            res.StatusCode = 200;
            res.Message = "Lấy dữ liệu thành công";
            res.Data = flashSaleTimeFrame;
            return res;
        }

    }
}
