using Microsoft.EntityFrameworkCore;
using nike_website_backend.Dtos;
using nike_website_backend.Helpers;
using nike_website_backend.Interfaces;
using nike_website_backend.Models;
using System.Net.WebSockets;

namespace nike_website_backend.Services
{
    public class ProductService : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<ProductDetailDto>> GetProductDetail(int productId)
        {
            Response<ProductDetailDto> response = new Response<ProductDetailDto>();
            var ProductDetailDto = await _context.Products.Where(p => p.ProductId == productId).Select(p => new ProductDetailDto
            {
                ProductId = p.ProductId,
                MoreInfo = p.ProductMoreInfo,
                ProductImage = p.ProductImg,
                SizeAndFit = p.ProductSizeAndFit,
                StyleCode = p.ProductStyleCode,
                ColorShown = p.ProductColorShown,
                salePrice = p.SalePrices,
                ProductImageDtos = p.ProductImgs.Select(p => new ProductImageDto
                {
                    ProductImageId = p.ProductImgId,
                    ImageFileName = p.ProductImgFileName
                }).ToList(),
                ProductSizeDtos = p.ProductSizes.Select(p => new ProductSizeDto
                {
                    ProductSizeId = p.ProductSizeId,
                    Quantity = p.Soluong,
                    SizeDto = new SizeDto
                    {
                        SizeId = p.Size.SizeId,
                        SizeName = p.Size.SizeName
                    }
                }).ToList(),
                ProductReviewDtos = p.ProductReviews.Select(p => new ProductReviewDto
                {
                    ProductReviewId = p.ProductReviewId,
                    ProductReviewTitle = p.ProductReviewTitle,
                    ProductReviewContent = p.ProductReviewContent,
                    ProductRating = p.ProductReviewRate,
                    ProductReviewDate = p.ProductReviewTime
                }).ToList()
            }).FirstOrDefaultAsync();

            response.StatusCode = 200;
            response.Message = "Lấy dữ liệu thành công";
            response.Data = ProductDetailDto;
            return response;
        }


        public async Task<Response<List<ProductObjectDto>>> GetProductObjects()
        {
            Response<List<ProductObjectDto>> response = new Response<List<ProductObjectDto>>();
            var productObjectDtos = await _context.ProductObjects.Select(p => new ProductObjectDto
            {
                ProductId = p.ProductObjectId,
                ProductName = p.ProductObjectName,
                Categories = p.Categories.Select(c => new CategoryDto
                {
                    CategoryId = c.CategoriesId,
                    CategoryName = c.CategoriesName,
                    SubCategories = c.SubCategories.Select(s => new SubCategoryDto
                    {
                        SubCategoryId = s.SubCategoriesId,
                        SubCategoryName = s.SubCategoriesName
                    }).ToList()
                }).ToList()
            }).ToListAsync();
            response.StatusCode = 200;
            response.Message = "Lấy dữ liệu thành công";
            response.Data = productObjectDtos;
            return response;
        }

        public async Task<Response<ProductParentDto>> GetProductParentDetail(int productParentId)
        {
            Response<ProductParentDto> response = new Response<ProductParentDto>();
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var currentDate = DateTime.Now;
            FlashSaleTimeFrame flashSaleTimeFrame = null;
            var flashSale = await _context.FlashSales.Where(f => f.StartedAt <= currentDate && f.EndedAt > currentDate && f.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            if (flashSale != null)
            {
                flashSaleTimeFrame = await _context.FlashSaleTimeFrames.Where(t => t.FlashSaleId == flashSale.FlashSaleId && t.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            }
            var productParentDto = await _context.ProductParents.Where(p => p.ProductParentId == productParentId).Select(p => new ProductParentDto
            {

                ProductParentId = p.ProductParentId,
                ProductParentName = p.ProductParentName,
                ProductIcon = new ProductIconDto
                {
                    ProductIconId = p.ProductIcons.ProductIconsId,
                    ProductIconName = p.ProductIcons.IconName,
                    Thumbnail = p.ProductIcons.Thumbnail
                },
                Products = p.Products.Select(t => new ProductDto
                {
                    ProductId = t.ProductId,
                    ProductImage = t.ProductImg,
                   stock = t.ProductSizes.Any() ? t.ProductSizes.Sum(x=>x.Soluong) : 0
                }).ToList(),
                Thumbnail = p.Thumbnail,
                ProductPrice = p.ProductPrice,
                IsNew = p.IsNewRelease,
                categoryWithObjectName = p.SubCategories.Categories.ProductObject.ProductObjectName + "'s " + p.SubCategories.Categories.CategoriesName,
                RegisterFlashSaleProduct = flashSaleTimeFrame != null
            ? p.RegisterFlashSaleProducts.FirstOrDefault(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId)
            : null,
                quantityInStock = p.Products.Sum(t => t.ProductSizes.Sum(s => s.Soluong)),
            }).FirstOrDefaultAsync();

            if(productParentDto == null)
            {
                response.StatusCode = 404;
                response.Message = "Không tìm thấy sản phẩm";
                response.Data = null;
            }

            response.StatusCode = 200;
            response.Message = "Lấy dữ liệu thành công";
            response.Data = productParentDto;
            return response;
        }

        public async Task<Response<List<ProductParentDto>>> GetProductParents(int subCategoryId, QueryObject queryObject)
        {
            Response<List<ProductParentDto>> response = new Response<List<ProductParentDto>>();
            var query = _context.ProductParents.Where(p => p.SubCategoriesId == subCategoryId).Select(p => new ProductParentDto
            {
                ProductParentId = p.ProductParentId,
                ProductParentName = p.ProductParentName,
                ProductIcon = new ProductIconDto
                {
                    ProductIconId = p.ProductIcons.ProductIconsId,
                    ProductIconName = p.ProductIcons.IconName,
                    Thumbnail = p.ProductIcons.Thumbnail
                },
                Products = p.Products.Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductImage = p.ProductImg,

                    // ... more properties
                }).ToList(),
                Thumbnail = p.Thumbnail,
                ProductPrice = p.ProductPrice,
                IsNew = p.IsNewRelease
            }).AsQueryable();

            // Tìm theo tên sản phẩm
            if (!string.IsNullOrEmpty(queryObject.ProductName))
            {
                query = query.Where(p => p.ProductParentName.Contains(queryObject.ProductName));
            }
            // Sắp xếp
            if (queryObject.SortBy == "price")
            {
                query = queryObject.IsSortAscending ? query.OrderBy(p => p.ProductPrice) : query.OrderByDescending(p => p.ProductPrice);
            }
            // Phân trang
            var skip = (queryObject.Page - 1) * queryObject.PageSize;
            var take = queryObject.PageSize;
            query = query.Skip(skip).Take(take);

            var productParentDtos = await query.ToListAsync();

            response.StatusCode = 200;
            response.Message = "Lấy dữ liệu thành công";
            response.Data = productParentDtos;
            return response;
        }
        public async Task<Response<List<ProductIcon>>> GetIcons(int page, int limit)
        {
            var offset = (page - 1) * limit;
            Response<List<ProductIcon>> response = new Response<List<ProductIcon>>();
            var icons = await _context.ProductIcons.Skip(offset).Take(limit).ToListAsync();
            response.StatusCode = 200;
            response.Message = "Lấy dữ liệu thành công";
            response.Data = icons;
            return response;
        }

        public async Task<Response<List<ProductParentDto>>> GetNewRelease(int page, int limit)
        {
            var offset = (page - 1) * limit;
            Response<List<ProductParentDto>> response = new Response<List<ProductParentDto>>();
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var currentDate = DateTime.Now;
            FlashSaleTimeFrame flashSaleTimeFrame = null;
            var flashSale = await _context.FlashSales.Where(f => f.StartedAt <= currentDate && f.EndedAt > currentDate && f.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            if (flashSale != null) {
                flashSaleTimeFrame = await _context.FlashSaleTimeFrames.Where(t => t.FlashSaleId == flashSale.FlashSaleId && t.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            }
            var query = _context.ProductParents.Where(p => p.CreatedAt >= thirtyDaysAgo && p.CreatedAt <= currentDate).OrderByDescending(p => p.Products.Sum(t => t.ProductSizes.Sum(s => s.Soluong))).Select(p => new ProductParentDto
            {
                ProductParentId = p.ProductParentId,
                ProductParentName = p.ProductParentName,
                ProductIconsId = p.ProductIconsId,
                Thumbnail = p.Thumbnail,
                ProductPrice = p.ProductPrice,
                IsNew = p.IsNewRelease,
                SubCategoriesId = p.SubCategoriesId,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                categoryWithObjectName = p.SubCategories.Categories.ProductObject.ProductObjectName + "'s " + p.SubCategories.Categories.CategoriesName,
                salePrice = p.Products.Any() ? p.Products.Where(p => p.SalePrices > 0).Min(p => p.SalePrices) : 0,
                RegisterFlashSaleProduct = flashSaleTimeFrame != null
            ? p.RegisterFlashSaleProducts.FirstOrDefault(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId)
            : null,
                quantityInStock = p.Products.Sum(t => t.ProductSizes.Sum(s => s.Soluong)),


            }).AsQueryable();

            var productParents = await query.Skip(offset).Take(limit).ToListAsync();
            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / limit);
            response.StatusCode = 200;
            response.Message = "Lấy dữ liệu thành công";
            response.Data = productParents;
            response.TotalPages = totalPages;
            return response;
        }

        public async Task<Response<List<ProductParentDto>>> GetProductByObjectID(int page, int limit,int objectId)
        {
            var offset = (page-1)*limit;
            Response<List<ProductParentDto>> res = new Response<List<ProductParentDto>>();
            FlashSaleTimeFrame flashSaleTimeFrame = null;
            var currentDate = DateTime.Now;
            var flashSale = await _context.FlashSales.Where(f => f.StartedAt <= currentDate && f.EndedAt > currentDate && f.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            if (flashSale != null)
            {
                flashSaleTimeFrame = await _context.FlashSaleTimeFrames.Where(t => t.FlashSaleId == flashSale.FlashSaleId && t.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            }

            var query = _context.ProductParents.Where(p=>p.SubCategories.Categories.ProductObjectId == objectId).OrderByDescending(p => p.Products.Sum(t => t.ProductSizes.Sum(s => s.Soluong))).Select(p => new ProductParentDto
            {
                ProductParentId = p.ProductParentId,
                ProductParentName = p.ProductParentName,
                ProductIconsId = p.ProductIconsId,
                Thumbnail = p.Thumbnail,
                ProductPrice = p.ProductPrice,
                IsNew = p.IsNewRelease,
                SubCategoriesId = p.SubCategoriesId,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                categoryWithObjectName = p.SubCategories.Categories.ProductObject.ProductObjectName + "'s " + p.SubCategories.Categories.CategoriesName,
                salePrice = p.Products.Any() ? p.Products.Where(p => p.SalePrices > 0).Min(p => p.SalePrices) : 0,
                RegisterFlashSaleProduct = flashSaleTimeFrame != null
        ? p.RegisterFlashSaleProducts.FirstOrDefault(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId)
        : null,
                quantityInStock = p.Products.Sum(t=>t.ProductSizes.Sum(s=>s.Soluong)),

            }).AsQueryable();
            var productParents = await query.Skip(offset).Take(limit).ToListAsync();
            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / limit);
            res.StatusCode = 200;
            res.Message = "Lấy dữ liệu thành công";
            res.Data = productParents;
            res.TotalPages = totalPages;
            return res;
        }
    }
       
}