using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using nike_website_backend.Dtos;
using nike_website_backend.Helpers;
using nike_website_backend.Interfaces;
using nike_website_backend.Models;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace nike_website_backend.Services
{
    public class UserOrderService : IUserOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public UserOrderService(ApplicationDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public async Task<ResponseGHN<dynamic>> GetOrderDetailGHNAsync(string orderCode)
        {
            try
            {
                var url = "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/detail";

                var payload = new
                {
                    order_code = orderCode
                };

                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Token", _configuration["GHN:KeyToken"]);


                var response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var data = System.Text.Json.JsonSerializer.Deserialize<dynamic>(result);

                    return new ResponseGHN<dynamic>
                    {
                        StatusCode = (int)response.StatusCode,
                        Message = "Thành công",
                        Data = data
                    };
                }

                // Trường hợp lỗi từ GHN
                return new ResponseGHN<dynamic>
                {
                    StatusCode = (int)response.StatusCode,
                    Message = $"Lỗi từ GHN API: {response.ReasonPhrase}",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                // Trường hợp ngoại lệ
                return new ResponseGHN<dynamic>
                {
                    StatusCode = 500,
                    Message = $"Lỗi khi gọi API GHN: {ex.Message}",
                    Data = null
                };
            }
        }
        public async Task<GHNDto> GetOrderDetailGHNAsync1(string orderCode)
        {
     
            try
            {
                var statusDescriptions = new Dictionary<string, string>
    {
        { "ready_to_pick", "Order created" },
        { "picking", "Staff is picking the items" },
        { "cancel", "Order canceled" },
        { "money_collect_picking", "Collecting money from the sender" },
        { "picked", "Items have been picked by the staff" },
        { "storing", "Items are stored in the warehouse" },
        { "transporting", "Items are in transit" },
        { "sorting", "Items are being sorted" },
        { "delivering", "Staff is delivering to the recipient" },
        { "money_collect_delivering", "Collecting money from the recipient" },
        { "delivered", "Items successfully delivered" },
        { "delivery_fail", "Delivery failed" },
        { "waiting_to_return", "Waiting for the return of the items to the sender" },
        { "return", "Return items" },
        { "return_transporting", "Items are being returned in transit" },
        { "return_sorting", "Returned items are being sorted" },
        { "returning", "Staff is returning the items" },
        { "return_fail", "Return failed" },
        { "returned", "Items successfully returned" },
        { "exception", "Exception order not following the process" },
        { "damage", "Items are damaged" },
        { "lost", "Items are lost" }
    };
                var url = "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/detail";

                var payload = new
                {
                    order_code = orderCode
                };

                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Token", _configuration["GHN:KeyToken"]);


                var response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(result);
                    string status = data?.data?.status;
                    Console.WriteLine($"Status from API: {status}");

                    // Kiểm tra xem status có trong dictionary không
                    string statusDes = statusDescriptions.TryGetValue(status, out string description)
                        ? description
                        : "Status not defined";

                    var logList = data?.data?.log;
                    var enrichedLogs = new List<LogEntry>();

                    // Kiểm tra nếu logList không null và có ít nhất một phần tử
                    if (logList != null && logList.Count > 0)
                    {
                        Console.WriteLine($"LogList: {logList.Count} items found.");

                        foreach (var logEntry in logList)
                        {
                            
        
                            enrichedLogs.Add(new LogEntry
                            {
                                Status = logEntry?.status,
                                // Đổi tên biến status để tránh xung đột
                                Description = logEntry?.status,
                                Timestamp = logEntry?.updated_date // Điều chỉnh tùy theo cấu trúc dữ liệu thực tế
                            });
                        }
                    }
                    else
                    {
                        // Nếu logList là null hoặc rỗng
                        Console.WriteLine("No logs found or logList is null.");
                    }




                    var service = new GHNDto
                    {
                        GhnStatus = data?.data?.status,
                        // Add description for the main status as well
                        GhnStatusDescription = statusDes,
                        Logs = enrichedLogs, // Return the enriched logs
                        UpdatedDate = data?.data?.updated_date,
                    };
         
                    return service;
        
                }
                return null;
                // Trường hợp lỗi từ GHN
                //return new ResponseGHN<dynamic>
                //{
                //    StatusCode = (int)response.StatusCode,
                //    Message = $"Lỗi từ GHN API: {response.ReasonPhrase}",
                //    Data = null
                //};
            }
            catch (Exception ex)
            {
                return null;
                // Trường hợp ngoại lệ
                //return new ResponseGHN<dynamic>
                //{
                //    StatusCode = 500,
                //    Message = $"Lỗi khi gọi API GHN: {ex.Message}",
                //    Data = null
                //};
            }
        }
        private async Task<GHNDto> GetServiceLogsFromGHN(string orderCode)
        {
            var statusDescriptions = new Dictionary<string, string>
    {
        { "ready_to_pick", "Order created" },
        { "picking", "Staff is picking the items" },
        { "cancel", "Order canceled" },
        { "money_collect_picking", "Collecting money from the sender" },
        { "picked", "Items have been picked by the staff" },
        { "storing", "Items are stored in the warehouse" },
        { "transporting", "Items are in transit" },
        { "sorting", "Items are being sorted" },
        { "delivering", "Staff is delivering to the recipient" },
        { "money_collect_delivering", "Collecting money from the recipient" },
        { "delivered", "Items successfully delivered" },
        { "delivery_fail", "Delivery failed" },
        { "waiting_to_return", "Waiting for the return of the items to the sender" },
        { "return", "Return items" },
        { "return_transporting", "Items are being returned in transit" },
        { "return_sorting", "Returned items are being sorted" },
        { "returning", "Staff is returning the items" },
        { "return_fail", "Return failed" },
        { "returned", "Items successfully returned" },
        { "exception", "Exception order not following the process" },
        { "damage", "Items are damaged" },
        { "lost", "Items are lost" }
    };

            try
            {
                var response = await GetOrderDetailGHNAsync(orderCode);
                //var result = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine(response.Data);
                Console.WriteLine(JsonConvert.SerializeObject(response.Data, Formatting.Indented));
          
                if (response.Data.code == 200 && response.Data != null)
                {
                    var orderData = response.Data.data;

                    Console.WriteLine($"Order Code: {orderData.order_code}");
                    Console.WriteLine($"Return Name: {orderData.return_name}");
                    Console.WriteLine($"From Name: {orderData.from_name}");
                    // Enrich logs with descriptions from the dictionary
                    var enrichedLogs = new List<LogEntry>();
                    if (response.Data.data.log != null && response.Data.data.log.Any())
                    {
                        foreach (var logEntry in response.Data.data.log)
                        {
                            enrichedLogs.Add(new LogEntry
                            {
                                Status = logEntry.status,
                                // Get the description from the dictionary, or use a default message if not found
                                Description = statusDescriptions.TryGetValue(logEntry.status, out string description)
                                    ? description
                                    : "Status not defined",
                                Timestamp = logEntry.timestamp // Adjust this based on the actual data structure
                            });
                        }
                    }

                    var service = new GHNDto
                    {
                        GhnStatus = response.Data.data.status,
                        // Add description for the main status as well
                        GhnStatusDescription = statusDescriptions.TryGetValue(response.Data.status, out string statusDescription)
                            ? statusDescription
                            : "Status not defined",
                        Logs = enrichedLogs, // Return the enriched logs
                        UpdatedDate = response.Data.updated_date,
                    };

                    return service;
                }

                return null; // Return null if no logs or failed API response
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., logging the exception)
                return null;
            }
        }



        public async Task<Response<List<UserOrderDTO>>> GetUserOrder(string userId, int userOrderStatusId, string text,int limit,int page)
        {
            
            
            try
            {
                if (!string.IsNullOrEmpty(text) && !text.All(char.IsDigit))
                {
                    return new Response<List<UserOrderDTO>>
                    {
                        StatusCode = 500,
                        Data = null,
                        Message = "The search string must be full digit."
                    };
                }
                var offset = (page  - 1) * limit;
                var userOrderId = int.Parse(text);

                // Tạo query cơ bản để lọc đơn hàng theo userId và userOrderStatusId
                var userOrders = await _context.UserOrders
    .Where(v => v.UserId == userId && v.UserOrderStatusId == userOrderStatusId && v.UserOrderId == userOrderId)
    .OrderByDescending(v => v.CreatedAt)
    .ToListAsync();

                var userOrderDTOs = new List<UserOrderDTO>();

                foreach (var userOrder in userOrders)
                {
                    var code = userOrder.OrderCodeReturn != null ? userOrder.OrderCodeReturn : userOrder.OrderCode;
                    
                    var serviceLogs = await GetOrderDetailGHNAsync1(code);

                    userOrderDTOs.Add(new UserOrderDTO
                    {
                        UserOrderId = userOrder.UserOrderId,
                        UserId = userOrder.UserId,
                        UserOrderStatusId = userOrder.UserOrderStatusId,
                        FirstName = userOrder.FirstName,
                        LastName = userOrder.LastName,
                        Address = userOrder.Address,
                        Email = userOrder.Email,
                        PhoneNumber = userOrder.PhoneNumber,
                        PaymentMethod = userOrder.PaymentMethod,
                        CreatedAt = userOrder.CreatedAt,
                        UpdatedAt = userOrder.UpdatedAt,
                        OrderCode = userOrder.OrderCode,
                        OrderCodeReturn = userOrder.OrderCodeReturn,
                        return_expiration_date = userOrder.ReturnExpirationDate,
                        IsReviewed = userOrder.IsReviewed,
                        IsProcessed = userOrder.IsProcessed,
                        VoucherApplied = userOrder.VouchersApplied,
                        IsCanceledBy = userOrder.IsCanceledBy,
                        ShippingFee = userOrder.ShippingFee,
                        TotalQuantity = userOrder.TotalQuantity,
                        TotalPrice = userOrder.TotalPrice,
                        DiscountPrice = userOrder.DiscountPrice,
                        FinalPrice = userOrder.FinalPrice,
                        GHNService = userOrder.GhnService,
                        serviceLogs = serviceLogs,
                    });
                }

                // Trả về danh sách đơn hàng trong một đối tượng Response
                return new Response<List<UserOrderDTO>>
                {
                    StatusCode = 200,
                    Data = userOrderDTOs,
                    Message = "Lấy danh sách đơn hàng thành công."
                };
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return new Response<List<UserOrderDTO>>
                {
                    StatusCode = 500,
                    Data = [],
                    Message = $"Đã xảy ra lỗi khi lấy đơn hàng: {ex.Message}"
                };
            }
        }


    }
}
