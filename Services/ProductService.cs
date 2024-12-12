using Microsoft.EntityFrameworkCore;
using nike_website_backend.Dtos;
using nike_website_backend.Helpers;
using nike_website_backend.Interfaces;
using nike_website_backend.Models;
using Quartz.Util;
using System.Linq;
using System.Net.WebSockets;
using System.Text.RegularExpressions;

namespace nike_website_backend.Services
{
    public class ProductService : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }
        private int ExtractSizeNumber(string sizeName)
        {
            var numberPart = Regex.Match(sizeName, @"\d+").Value;
            return int.TryParse(numberPart, out var sizeNumber) ? sizeNumber : int.MaxValue;
        }
        public async Task<Response<ProductDetailDto>> GetProductDetail(int productId)
        {
            Response<ProductDetailDto> response = new Response<ProductDetailDto>();
            var query = _context.Products.Where(p => p.ProductId == productId).Select(p => new ProductDetailDto
            {
                ProductId = p.ProductId,
                MoreInfo = p.ProductDescription,
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

            }).AsNoTracking().AsQueryable();

            var ProductDetailDto = await query.FirstOrDefaultAsync();
            if (ProductDetailDto == null)
            {
                response.StatusCode = 404;
                response.Message = "Product Not Found !!!";
                response.Data = null;
                return response;
            }
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
            var isAllOverSize = ProductDetailDto.ProductSizeDtos.All(s => s.SizeDto.SizeName.Equals("Oversize", StringComparison.OrdinalIgnoreCase));
            if (isAllOverSize)
            {
                response.StatusCode = 200;
                response.Message = "Lấy dữ liệu thành công";
                response.Data = ProductDetailDto;
                return response;
            }
            else
            {
                ProductDetailDto.ProductSizeDtos = ProductDetailDto.ProductSizeDtos.OrderBy(s =>
            sizeOrder.ContainsKey(s.SizeDto.SizeName)
                ? sizeOrder[s.SizeDto.SizeName]
                : ExtractSizeNumber(s.SizeDto.SizeName)
                ).ToList();
                response.StatusCode = 200;
                response.Message = "Lấy dữ liệu thành công";
                response.Data = ProductDetailDto;
                return response;
            }

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

            var currentDate = DateTime.Now;
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime localCurrentDate = TimeZoneInfo.ConvertTime(currentDate, localTimeZone);
            FlashSaleTimeFrame flashSaleTimeFrame = null;
            var flashSale = await _context.FlashSales.Where(f => f.StartedAt <= localCurrentDate && f.EndedAt > localCurrentDate && f.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

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
                    stock = t.ProductSizes.Any() ? t.ProductSizes.Sum(x => x.Soluong) : 0
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

            if (productParentDto == null)
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

        public async Task<Response<List<ProductParentDto>>> GetProductParents(QueryObject queryObject)
        {
            var offset = (queryObject.Page - 1) * queryObject.PageSize;
            Response<List<ProductParentDto>> response = new Response<List<ProductParentDto>>();
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var currentDate = DateTime.Now;
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime localCurrentDate = TimeZoneInfo.ConvertTime(currentDate, localTimeZone);
            DateTime localThirtyDaysAgo = TimeZoneInfo.ConvertTime(thirtyDaysAgo, localTimeZone);

            FlashSaleTimeFrame flashSaleTimeFrame = null;
            var flashSale = await _context.FlashSales.Where(f => f.StartedAt <= localCurrentDate && f.EndedAt > localCurrentDate && f.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            if (flashSale != null)
            {
                flashSaleTimeFrame = await _context.FlashSaleTimeFrames.Where(t => t.FlashSaleId == flashSale.FlashSaleId && t.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            }
            var query = _context.ProductParents.OrderByDescending(p => p.Products.Sum(t => t.ProductSizes.Sum(s => s.Soluong))).Select(p => new ProductParentDto
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
                ProductObjectId = p.SubCategories.Categories.ProductObject.ProductObjectId,
                categoryWithObjectName = p.SubCategories.Categories.ProductObject.ProductObjectName + "'s " + p.SubCategories.Categories.CategoriesName,
                salePrice = p.Products.Any() ? p.Products.Where(p => p.SalePrices > 0).Min(p => p.SalePrices) : 0, // Tìm trong các product color có giá sale thì hiển thị, còn lại thì gán 0
                finalPrice = p.Products.Any() ? p.Products.Where(p => p.SalePrices > 0).Min(p => p.SalePrices) : p.ProductPrice, // Tìm trong các product color có giá sale thì hiển thị, còn lại thì hiển thị giá gốc
                RegisterFlashSaleProduct = flashSaleTimeFrame != null
            ? p.RegisterFlashSaleProducts.FirstOrDefault(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId)
            : null,
                quantityInStock = p.Products.Sum(t => t.ProductSizes.Sum(s => s.Soluong)),


            }).AsQueryable();

            // Xử lý ...
            // Lọc theo subCategoryId(xong)
            if (queryObject.SubCategoryId > 0)
            {
                query = query.Where(p => p.SubCategoriesId == queryObject.SubCategoryId);
            }

            // Tìm theo tên sản phẩm
            if (!queryObject.ProductName.Contains("-1"))
            {
                query = query.Where(p => p.ProductParentName.Contains(queryObject.ProductName));
            }

            // Theo min max price(xong)
            if (queryObject.MaxPrice > queryObject.MinPrice)
            {
                query = query.Where(p => p.ProductPrice >= queryObject.MinPrice && p.ProductPrice <= queryObject.MaxPrice);
            }

            // Theo giá salePrice

            // Sort ...
            // Theo giới tính(xong)
            string[] objectIdsString = queryObject.productObjectId.Split(',');
            int[] objectIds = Array.ConvertAll(objectIdsString, int.Parse);

            // Filter query based on productObjectIds
            if (objectIds.Intersect(new int[] { 1, 2 }).Any())
            {
                query = query.Where(p => objectIds.Contains(p.ProductObjectId));
            }
            // Sắp xếp theo tên (xong)
            if (queryObject.SortBy == "productName")
            {
                query = queryObject.IsSortAscending ? query.OrderBy(p => p.ProductParentName) : query.OrderByDescending(p => p.ProductPrice);
            }
            // Sắp xếp theo createAt (xong)
            if (queryObject.SortBy == "createAt")
            {
                query = queryObject.IsSortAscending ? query.OrderBy(p=>p.CreatedAt) : query.OrderByDescending(p => p.ProductPrice);
            }
            // Sắp xếp theo giá (xong)
            if (queryObject.SortBy == "price")
            {
                query = queryObject.IsSortAscending ? query.OrderBy(p => p.finalPrice) : query.OrderByDescending(p => p.ProductPrice);
            }


            // Tính tổng số trang
            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / queryObject.PageSize);

            // Phân trang(xong)
            var skip = (queryObject.Page - 1) * queryObject.PageSize;
            var take = queryObject.PageSize;
            query = query.Skip(skip).Take(take);


            var productParentDtos = await query.ToListAsync();

            response.StatusCode = 200;
            response.Message = "Lấy dữ liệu thành công";
            response.Data = productParentDtos;
            response.TotalPages = totalPages;
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
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime localCurrentDate = TimeZoneInfo.ConvertTime(currentDate, localTimeZone);
            DateTime localThirtyDaysAgo = TimeZoneInfo.ConvertTime(thirtyDaysAgo, localTimeZone);

            FlashSaleTimeFrame flashSaleTimeFrame = null;
            var flashSale = await _context.FlashSales.Where(f => f.StartedAt <= localCurrentDate && f.EndedAt > localCurrentDate && f.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            if (flashSale != null)
            {
                flashSaleTimeFrame = await _context.FlashSaleTimeFrames.Where(t => t.FlashSaleId == flashSale.FlashSaleId && t.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            }
            var query = _context.ProductParents.Where(p => p.CreatedAt >= localThirtyDaysAgo && p.CreatedAt <= localCurrentDate).OrderByDescending(p => p.Products.Sum(t => t.ProductSizes.Sum(s => s.Soluong))).Select(p => new ProductParentDto
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

            var productParents = await query.Where(p => p.quantityInStock != 0).Skip(offset).Take(limit).ToListAsync();
            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / limit);
            response.StatusCode = 200;
            response.Message = "Lấy dữ liệu thành công";
            response.Data = productParents;
            response.TotalPages = totalPages;
            return response;
        }

        public async Task<Response<List<ProductParentDto>>> GetProductByObjectID(int page, int limit, int objectId)
        {
            var offset = (page - 1) * limit;
            Response<List<ProductParentDto>> res = new Response<List<ProductParentDto>>();
            FlashSaleTimeFrame flashSaleTimeFrame = null;
            var currentDate = DateTime.Now;
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime localCurrentDate = TimeZoneInfo.ConvertTime(currentDate, localTimeZone);
            var flashSale = await _context.FlashSales.Where(f => f.StartedAt <= localCurrentDate && f.EndedAt > localCurrentDate && f.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            if (flashSale != null)
            {
                flashSaleTimeFrame = await _context.FlashSaleTimeFrames.Where(t => t.FlashSaleId == flashSale.FlashSaleId && t.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            }

            var query = _context.ProductParents.Where(p => p.SubCategories.Categories.ProductObjectId == objectId).OrderByDescending(p => p.Products.Sum(t => t.ProductSizes.Sum(s => s.Soluong))).Select(p => new ProductParentDto
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
            var productParents = await query.Where(p => p.quantityInStock != 0).Skip(offset).Take(limit).ToListAsync();
            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / limit);
            res.StatusCode = 200;
            res.Message = "Lấy dữ liệu thành công";
            res.Data = productParents;
            res.TotalPages = totalPages;
            return res;
        }

        public async Task<Response<List<ProductReviewDto>>> getReviewsOfColor(int productId, int page, int limit, string sortBy, double rating)
        {
            var offset = (page - 1) * limit;
            Response<List<ProductReviewDto>> res = new Response<List<ProductReviewDto>>();
            var query = _context.ProductReviews.Where(x => x.ProductId == productId && (x.ProductReviewRate == rating || rating == 6)).Select(r => new ProductReviewDto
            {
                ProductReviewId = r.ProductReviewId,
                ProductRating = r.ProductReviewRate,
                ProductSizeName = r.ProductSizeName,
                ProductReviewContent = r.ProductReviewContent,
                ProductReviewDate = r.ProductReviewTime,
                ProductReviewTitle = r.ProductReviewTitle,
                UserAccount = r.User
            });
            query = sortBy switch
            {
                "newest" => query.OrderByDescending(r => r.ProductReviewDate),
                "oldest" => query.OrderBy(r => r.ProductReviewDate),
                "highest-rating" => query.OrderByDescending(r => r.ProductRating),
                "lowest-rating" => query.OrderBy(r => r.ProductRating),
                _ => query // Không sắp xếp nếu `sortBy` không hợp lệ
            };

            var reviews = await query.Skip(offset).Take(limit).ToListAsync();
            var count = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)count / limit);
            res.StatusCode = 200;
            res.Data = reviews;
            res.TotalPages = totalPages;
            return res;


        }

        public async Task<Response<List<ProductParentDto>>> getRecommendProductParent(string? userId, int limit)
        {
            Response<List<ProductParentDto>> res = new Response<List<ProductParentDto>>();
            List<HistorySearch> historySearch = new List<HistorySearch>();

            if (!string.IsNullOrEmpty(userId))
            {
                historySearch = _context.HistorySearches
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(10)
                    .ToList();
            }

            FlashSaleTimeFrame flashSaleTimeFrame = null;
            var currentDate = DateTime.Now;
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime localCurrentDate = TimeZoneInfo.ConvertTime(currentDate, localTimeZone);
            var flashSale = await _context.FlashSales
                .Where(f => f.StartedAt <= localCurrentDate && f.EndedAt > localCurrentDate && f.Status.Equals("active"))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (flashSale != null)
            {
                flashSaleTimeFrame = await _context.FlashSaleTimeFrames
                    .Where(t => t.FlashSaleId == flashSale.FlashSaleId && t.Status.Equals("active"))
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }

            var query = _context.ProductParents.Select(p => new ProductParentDto
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
                salePrice = p.Products.Any(pr => pr.SalePrices > 0)
    ? p.Products.Where(pr => pr.SalePrices > 0).Min(pr => pr.SalePrices)
    : 0,

                RegisterFlashSaleProduct = flashSaleTimeFrame != null
                    ? p.RegisterFlashSaleProducts.FirstOrDefault(r => r.FlashSaleTimeFrameId == flashSaleTimeFrame.FlashSaleTimeFrameId)
                    : null,
                quantityInStock = p.Products.Sum(t => t.ProductSizes.Sum(s => s.Soluong)),
            }).AsQueryable();

            List<ProductParentDto> prioritizedProducts = new();
            List<ProductParentDto> otherProducts = new();

            if (historySearch.Any())
            {
                var searchKeywords = historySearch.Select(h => h.TextSearch).Distinct().ToList();

                // Lọc sản phẩm có liên quan đến lịch sử tìm kiếm
                prioritizedProducts = await query
                    .Where(p => searchKeywords.Any(keyword => p.ProductParentName.Contains(keyword)) && p.quantityInStock != 0)
                    .AsNoTracking()
                    .ToListAsync();

                // Loại bỏ các sản phẩm đã được ưu tiên
                otherProducts = await query
                    .Where(p => !searchKeywords.Any(keyword => p.ProductParentName.Contains(keyword)))
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {

                otherProducts = await query.Where(p => p.quantityInStock != 0).AsNoTracking().ToListAsync();
            }

            var random = new Random();


            prioritizedProducts = prioritizedProducts.OrderBy(_ => random.Next()).ToList();
            otherProducts = otherProducts.OrderBy(_ => random.Next()).ToList();


            var finalResult = prioritizedProducts.Concat(otherProducts).Take(limit).ToList();

            res.StatusCode = 200;
            res.Message = "Lấy dữ liệu thành công";
            res.Data = finalResult;

            return res;
        }

        public async Task<Response<List<ProductParentDto>>> SearchFilter(SearchFilterQueryObject queryObject)
        {
            Response<List<ProductParentDto>> res = new Response<List<ProductParentDto>>();
            // if product name != "" => search by product name dont care about subcategory, SubCategoryId, productObjectId, product_color_shown
            var offset = (queryObject.Page - 1) * queryObject.PageSize;
            var currentDate = DateTime.Now;
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime localCurrentDate = TimeZoneInfo.ConvertTime(currentDate, localTimeZone);

            FlashSaleTimeFrame flashSaleTimeFrame = null;
            var flashSale = await _context.FlashSales.Where(f => f.StartedAt <= localCurrentDate && f.EndedAt > localCurrentDate && f.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            if (flashSale != null)
            {
                flashSaleTimeFrame = await _context.FlashSaleTimeFrames.Where(t => t.FlashSaleId == flashSale.FlashSaleId && t.Status.Equals("active")).AsNoTracking().FirstOrDefaultAsync();

            }
            // Khởi tạo query ban đầu
            var query = _context.ProductParents.AsQueryable();

            // Lọc theo tên sản phẩm
            if (!string.IsNullOrWhiteSpace(queryObject.searchText))
            {
                var searchText = queryObject.searchText.ToLower();
                query = query.Where(p =>
                    p.ProductParentName.ToLower().Contains(searchText) ||
                    p.Products.Any(x => x.ProductStyleCode.ToLower().Contains(searchText))
                );
            }
            // Lọc theo SubCategoryId
            if (queryObject.sub_categories_id > 0)
            {
                query = query.Where(p => p.SubCategoriesId == queryObject.sub_categories_id);
            }

            // Lọc theo productObjectId
            if (queryObject.productObjectId != null && queryObject.productObjectId.Any(id => id != -1))
            {
                query = query.Where(p => queryObject.productObjectId.Contains(p.SubCategories.Categories.ProductObjectId ?? -1));
            }
            // lọc theo product_color_shown
            if (queryObject.product_color_shown != null && queryObject.product_color_shown.Any())
            {
                var normalizedColors = queryObject.product_color_shown.Select(color => color.ToLower()).ToList();

                query = query.Where(p => p.Products.Any(pr =>
                    !string.IsNullOrEmpty(pr.ProductColorShown) &&
                    normalizedColors.Contains(pr.ProductColorShown.ToLower())
                ));
            }
            // Lọc theo MinPrice và MaxPrice
            if (queryObject.MinPrice > 0 && queryObject.MaxPrice > 0 && queryObject.MinPrice <= queryObject.MaxPrice)
            {
                query = query.Where(p => p.Products.Any(prod =>
                    prod.SalePrices > 0
                    ? prod.SalePrices >= queryObject.MinPrice && prod.SalePrices <= queryObject.MaxPrice
                    : p.ProductPrice >= queryObject.MinPrice && p.ProductPrice <= queryObject.MaxPrice));
            }
            // Thực hiện phân trang
            var totalProducts = await query.CountAsync();
            var productParents = await query
                .Skip(offset)
                .Take(queryObject.PageSize)
                .Select(p => new ProductParentDto
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
                    salePrice = p.Products.Any(prod => prod.SalePrices > 0)
                        ? p.Products.Where(prod => prod.SalePrices > 0).Min(prod => prod.SalePrices)
                        : 0,
                    finalPrice = p.Products.Any(prod => prod.SalePrices > 0)
                        ? p.Products.Where(prod => prod.SalePrices > 0).Min(prod => prod.SalePrices)
                        : p.ProductPrice,
                    quantityInStock = p.Products.Sum(prod => prod.ProductSizes.Sum(size => size.Soluong)),
                    sold = p.Products.Sum(prod => prod.Sold),
                })
                .ToListAsync();

            // sort by final price, sold , createAt
            if (queryObject.SortBy == "price")
            {
                productParents = queryObject.IsSortAscending
                    ? productParents.OrderBy(p => p.finalPrice).ToList()
                    : productParents.OrderByDescending(p => p.finalPrice).ToList();
            }

            if (queryObject.SortBy == "sold")
            {
                productParents = productParents.OrderBy(p => p.sold).ToList();
            }

            if (queryObject.SortBy == "createAt")
            {
                productParents = productParents.OrderBy(p => p.CreatedAt).ToList();
            }

            if (totalProducts == 0)
            {
                res.StatusCode = 404;
                res.Message = "Không tìm thấy sản phẩm";
                res.Data = null;
                return res;
            }

            // Tính toán tổng số trang
            var totalPages = (int)Math.Ceiling((double)totalProducts / queryObject.PageSize);

            // Trả về kết quả
            res.StatusCode = 200;
            res.Message = "Lấy dữ liệu thành công";
            res.Data = productParents;
            res.TotalPages = totalPages;
            return res;
        }

      
    }
}