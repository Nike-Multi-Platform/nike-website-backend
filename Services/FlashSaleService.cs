using nike_website_backend.Interfaces;
using nike_website_backend.Models;
using Microsoft.EntityFrameworkCore;
using nike_website_backend.Dtos;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
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

        public async Task<Response<List<FlashSaleTimeFrame>>> getAvailableFlashSaleTimeFrame()
        {
            Response<List<FlashSaleTimeFrame>> response = new Response<List<FlashSaleTimeFrame>>();
            try
            {
                var currentDate = DateTime.Now;
                TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
                DateTime localCurrentDate = TimeZoneInfo.ConvertTime(currentDate, localTimeZone);
                var flashSaleTimeFrames = await _context.FlashSaleTimeFrames.Where(p => (p.Status.Equals("active") && p.StartedAt <= localCurrentDate && p.EndedAt > localCurrentDate) || (p.Status.Equals("waiting") && p.StartedAt > localCurrentDate) ).OrderBy(p=>p.StartedAt).Take(5).ToListAsync();
                if(flashSaleTimeFrames.Count == 0)
                {
                    flashSaleTimeFrames = await _context.FlashSaleTimeFrames.Where(p => p.Status.Equals("ended") ).OrderByDescending(p=>p.EndedAt).Take(1).ToListAsync();
                }
                response.Message = "Lấy dữ liệu thành công.";
                response.StatusCode = 200;
                response.Data = flashSaleTimeFrames;
                return response;
            }
            catch (Exception ex) {
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500;
                response.Data = [];
                return response;
            }
        }

        public async Task<Response<List<ProductParentDto>>> getProductsByTimeFrameId(int  timeFrameId, int page,int limit)
        {
            Response<List<ProductParentDto>> response = new Response<List<ProductParentDto>>();
            var offset = (page - 1) * limit;
            try
            {
                var query = _context.RegisterFlashSaleProducts.Where(p=>p.FlashSaleTimeFrameId == timeFrameId).Select(p => new ProductParentDto
                {
                    ProductParentId = p.ProductParentId,
                    ProductParentName = p.ProductParent.ProductParentName,
                    ProductIconsId = p.ProductParent.ProductIconsId,
                    Thumbnail = p.ProductParent.Thumbnail,
                    ProductPrice = p.ProductParent.ProductPrice,
                    IsNew = p.ProductParent.IsNewRelease,
                    SubCategoriesId = p.ProductParent.SubCategoriesId,
                    CreatedAt = p.ProductParent.CreatedAt,
                    UpdatedAt = p.ProductParent.UpdatedAt,
                    salePrice = p.FlashSalePrice,
                    categoryWithObjectName = $"{p.ProductParent.SubCategories.Categories.ProductObject.ProductObjectName}'s {p.ProductParent.SubCategories.Categories.CategoriesName}",
                    sold = p.Sold,
                    quantity = p.Quantity,
                    quantityInStock = p.ProductParent.Products.Sum(t => t.ProductSizes.Sum(s => s.Soluong)),
                }).AsNoTracking().AsQueryable();
                var products = await query.Skip(offset).Take(limit).ToListAsync();


                var count = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)count / limit);


                response.StatusCode = 200;
                response.Message = "Lấy dữ liệu thành công";
                response.Data = products;
                response.TotalPages = totalPages;
               
                return response;

            }catch(Exception ex)
            {
                response.Message = $"Unexpected error: {ex.Message}";
                response.StatusCode = 500;
                response.Data = [];
                return response;
            }
        }
    }
}
