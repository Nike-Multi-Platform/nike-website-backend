
using Microsoft.EntityFrameworkCore;
using nike_website_backend.Dtos;
using nike_website_backend.Interfaces;
using nike_website_backend.Libraries;
using nike_website_backend.Models;
using System.Net.WebSockets;
using static Google.Apis.Requests.BatchRequest;

namespace nike_website_backend.Services
{
    public class PaymentService : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public PaymentService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<Response<string>> CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            Response<string> response = new Response<string>();
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["Vnpay:PaymentBackReturnUrl"];
            DateTime expireDate = DateTime.Now.AddMinutes(5);
            Console.WriteLine($"{_configuration["Vnpay:TmnCode"]}");
            Console.WriteLine($"{_configuration["Vnpay:CurrCode"]}");
            Console.WriteLine($"{pay.GetIpAddress(context)}");
            Console.WriteLine($"{urlCallBack}");
            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", (model.Amount*100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            pay.AddRequestData("vnp_OrderType","other");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);
            pay.AddRequestData("vnp_ExpireDate", expireDate.ToString("yyyyMMddHHmmss"));
            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
            object isSave = await saveOrder(model.Order, tick);

            if (isSave is ResponseFunc resFunc)
            {
                bool success = resFunc.Success;
                string message = resFunc.Message;
                Console.WriteLine($"Message: {message}");

                if (!success)
                {
                    response.StatusCode = 400;
                    response.Message = message;
                    response.Data = null;
                    return response;
                }


            }


            response.StatusCode = 200;
            response.Message = "Tạo url thành công";
            response.Data = paymentUrl;
            return response;
        }

        public async Task<Response<Boolean>> checkoutByCOD(PaymentInformationModel model)
        {
            Response<Boolean> response = new Response<Boolean>();

            object isSave = await saveOrder(model.Order);

            if (isSave is ResponseFunc resFunc)
            {
                bool success = resFunc.Success;
                string message = resFunc.Message;
                Console.WriteLine($"Message: {message}");

                if (!success)
                {
                    response.StatusCode = 400;
                    response.Message = message;
                    response.Data = false;
                    return response;
                }


            }
            response.StatusCode = 200;
            response.Message = "Place Order Successfully.";
            response.Data = true;
            return response;
        }

        public async Task<object> saveOrder(UserOrderDTO Order, string transactionCode = null)
        {
            Console.WriteLine("Save order");
            var currentDate = DateTime.Now;
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime localCurrentDate = TimeZoneInfo.ConvertTime(currentDate, localTimeZone);
            // Define the response object
            var response = new ResponseFunc
            {
                Message = "",
                Success = false,
            };

            if (Order == null)
            {
                response.Message = "Order cannot be null.";
                Console.WriteLine("Chưa save được");
                return response;
            }

            try
            {
                if (Order.bagItems.Count > 0)
                {

                    foreach (var bagItem in Order.bagItems)
                    {
                        Console.WriteLine($"BagItem: {bagItem.bagId}");
                        if (bagItem.RegisterFlashSaleProduct != null)
                        {


                            var registerFlashSaleTimeFrame = await _context.RegisterFlashSaleProducts.Where(v => v.RegisterFlashSaleProduct1 == bagItem.RegisterFlashSaleProduct.RegisterFlashSaleProduct1 && v.FlashSaleTimeFrame.StartedAt <= localCurrentDate && v.FlashSaleTimeFrame.EndedAt > localCurrentDate).FirstOrDefaultAsync();
                            if (registerFlashSaleTimeFrame == null)
                            {
                                response.Message = $"Flash Sale ended!!!";
                                response.Success = false;
                                return response;
                            }

                            else if (registerFlashSaleTimeFrame != null && registerFlashSaleTimeFrame.Quantity - registerFlashSaleTimeFrame.Sold < bagItem.amount)
                            {
                                response.Message = $"Not enough stock in Flash Sale. Only {registerFlashSaleTimeFrame.Quantity - registerFlashSaleTimeFrame.Sold} items available.";
                                response.Success = false;
                                return response;
                            }
                            else
                            {
                                registerFlashSaleTimeFrame.Sold += bagItem.amount;
                                registerFlashSaleTimeFrame.UpdatedAt = DateTime.Now;
                            }
                        }
                        var productSize = await _context.ProductSizes.Include(ps=>ps.Product)
                         .Where(ps => ps.ProductSizeId == bagItem.product_size_id)
                         .FirstOrDefaultAsync();
                        Console.WriteLine($"productSize: {productSize.ProductSizeId}");
                        if (productSize != null)
                        {
                            if (productSize.Soluong < bagItem.amount)
                            {
                                response.Message = $"Not enough stock. Only {productSize.Soluong} items available.";
                                response.Success = false;
                                return response;
                            }
                           

                        }else
                        {
                            response.Message = $"Product Size Not Found";
                            response.Success = false;
                            return response;
                        }
                        productSize.Product.Sold += bagItem.amount;
                        productSize.Soluong -= bagItem.amount;

                        // Update the ProductSize in the database
                        _context.ProductSizes.Update(productSize);


                    }
                    Console.WriteLine("Save được rồi ne");
                    if (Order.VoucherApplied != null)
                    {

                        var discountVoucherAppliedId = int.Parse(Order.VoucherApplied.ToString());
                        Console.WriteLine($"voucher applied: {discountVoucherAppliedId}");
                        var discountVoucher = await _context.DiscountVouchers
                            .Where(v => v.DiscountVoucherId == discountVoucherAppliedId && v.StartedAt <= localCurrentDate && v.EndedAt > localCurrentDate &v.Quantity > 0)
                            .FirstOrDefaultAsync();

                        if (discountVoucher != null)
                        {

                            var discountVoucherUsage = await _context.UserDiscountVouchers
                                .Where(v => v.DiscountVoucherId == discountVoucher.DiscountVoucherId && v.UserId == Order.UserId)
                                .FirstOrDefaultAsync();

                            if (discountVoucherUsage == null)
                            {
                                var newDiscountVoucherUsage = new UserDiscountVoucher
                                {
                                    DiscountVoucherId = discountVoucher.DiscountVoucherId,
                                    UserId = Order.UserId,
                                    TotalUsed = 1,
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                };
                                await _context.UserDiscountVouchers.AddAsync(newDiscountVoucherUsage);
                            }
                            else if (discountVoucherUsage != null && discountVoucherUsage.TotalUsed >= discountVoucher.Usage)
                            {
                                response.Message = "Voucher limit reached.";
                                response.Success = false;
                                return response;
                            }
                            else
                            {
                                discountVoucherUsage.TotalUsed += 1;
                                discountVoucherUsage.UpdatedAt = DateTime.Now;
                            }

                            discountVoucher.Quantity -= 1;
                        }
                        else
                        {
                            response.Message = "Voucher not available";
                            response.Success = false;
                            return response;
                        }
                    }
                    var userOrder = await _context.UserOrders.AddAsync(new UserOrder
                    {
                        UserId = Order.UserId,
                        UserOrderStatusId = (int)Order.UserOrderStatusId,
                        FirstName = Order.FirstName,
                        LastName = Order.LastName,
                        Address = Order.Address,
                        Email = Order.Email,
                        PhoneNumber = Order.PhoneNumber,
                        PaymentMethod = Order.PaymentMethod,
                        VouchersApplied = Order.VoucherApplied,
                        ShippingFee = (decimal)Order.ShippingFee,
                        TotalQuantity = (int)Order.TotalQuantity,
                        TotalPrice = (decimal)Order.TotalPrice,
                        DiscountPrice = (decimal)Order.DiscountPrice,
                        FinalPrice = (decimal)Order.FinalPrice,
                        GhnService = Order.GHNService,
                        TransactionCode = transactionCode,
                    });
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Save được rồi ne {userOrder.Entity.UserOrderId}");

                    var orderItems = Order.bagItems.Select(bagItem => new UserOrderProduct
                    {
                        UserOrderId = userOrder.Entity.UserOrderId,
                        ProductSizeId = bagItem.product_size_id,
                        Amount = (int)bagItem.amount,
                        Thumbnail = bagItem.details.ProductImage,
                        ProductName = bagItem.details.ProductName,
                        SizeName = bagItem.ProductSizeName,
                        Price = (decimal)bagItem.details.finalPrice,
                        OnRegisterFlashSalesId = bagItem.RegisterFlashSaleProduct != null ? bagItem.RegisterFlashSaleProduct.RegisterFlashSaleProduct1 : null,
                    }).ToList();
                    Console.WriteLine("Save được rồi ne");
                    var bagItemIds = Order.bagItems.Select(bagItem => bagItem.bagId).ToList();
                    var bagItemsToRemove = await _context.Bags
                    .Where(bag => bagItemIds.Contains(bag.BagId))
                    .ToListAsync();

                    if (bagItemsToRemove.Any())
                    {
                        _context.Bags.RemoveRange(bagItemsToRemove);
                        await _context.SaveChangesAsync();
                        Console.WriteLine("Bag items removed successfully");
                    }
                    await _context.UserOrderProducts.AddRangeAsync(orderItems);
                    
                }
                // Commit changes to the database
                await _context.SaveChangesAsync();
               
                // If everything is successful, return success message
                response.Message = "Order saved successfully!";
                response.Success = true;
            }
            catch (Exception ex)
            {
                // If an exception occurs, handle it here
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }


        public async Task<Response<String>> PaymentExecute(IQueryCollection collections)
        {
            // Khởi tạo đối tượng Response để trả về
            var res = new Response<String>();
        
            var pay = new VnPayLibrary();
 
            // Lấy dữ liệu phản hồi từ VnPay và giải mã
            //var response =  pay.GetResponseData("vnp_ResponseCode");
           
            string vnpTxnRef = collections["vnp_TxnRef"];
            string responseCode = collections["vnp_ResponseCode"];


            var foundOrder = await _context.UserOrders.Where(v => v.TransactionCode == vnpTxnRef).FirstOrDefaultAsync();
            if (foundOrder == null) {
                res.StatusCode = 404;
                res.Message = "Order Not Found";
                res.Data = "fail";
                return res;
            }
            if (responseCode != "00" && foundOrder.UserOrderStatusId != 5)
            {
                foundOrder.UserOrderStatusId = 5;
                foundOrder.IsCanceledBy = 0;
                foundOrder.IsProcessed = 1;
                if (foundOrder.VouchersApplied != null)
                {
                    var voucherId = int.Parse(foundOrder.VouchersApplied);
                    var voucher = await _context.DiscountVouchers.Where(v => v.DiscountVoucherId == voucherId).FirstOrDefaultAsync();
                    if (voucher != null)
                    {
                        voucher.Quantity += 1;
                        var usage = await _context.UserDiscountVouchers.Where(v => v.UserId == foundOrder.UserId && v.DiscountVoucherId == voucher.DiscountVoucherId).FirstOrDefaultAsync();
                        if (usage != null)
                        {
                            usage.TotalUsed -= 1;
                        }
                    }
                }

                var orderItems = await _context.UserOrderProducts.Where(v => v.UserOrderId == foundOrder.UserOrderId).ToListAsync();
                foreach (var item in orderItems)
                {
                    var flashSaleItems = await _context.RegisterFlashSaleProducts.Where(v => v.RegisterFlashSaleProduct1 == item.OnRegisterFlashSalesId).FirstOrDefaultAsync();
                    if (flashSaleItems != null)
                    {
                        flashSaleItems.Sold -= item.Amount;
                    }
                    var productSize = await _context.ProductSizes.Include(p=>p.Product).Where(v => v.ProductSizeId == item.ProductSizeId).FirstOrDefaultAsync();
                    if (productSize != null)
                    {
                        productSize.Soluong += item.Amount;
                        productSize.Product.Sold -= item.Amount;
                    }
                }
                await _context.SaveChangesAsync();
            }
            
            


            // Kiểm tra nếu có dữ liệu trả về

            switch (responseCode)
            {
                case "00":
                    // Transaction success
                    res.StatusCode = 200;
                    res.Message = "Thank you for supporting Hamans Vietnam. Your order will be shipped to you soon.";
                    res.Data = "success";
                    break;

                case "07":
                    // Money successfully deducted, suspicious transaction (related to fraud, unusual transaction)
                    res.StatusCode = 200;
                    res.Message = "Transaction successful, but suspicious (related to fraud or unusual transaction).";
                    res.Data = "warning";
                    break;

                case "09":
                    // Transaction failed: Customer's card/account is not registered for Internet Banking service
                    res.StatusCode = 400;
                    res.Message = "Transaction failed: Customer's card/account is not registered for Internet Banking service.";
                    res.Data = "fail";
                    break;

                case "10":
                    // Transaction failed: Customer has entered incorrect card/account information more than 3 times
                    res.StatusCode = 400;
                    res.Message = "Transaction failed: Customer entered incorrect card/account information more than 3 times.";
                    res.Data = "fail";
                    break;

                case "11":
                    // Transaction failed: Payment waiting time expired. Please try again
                    res.StatusCode = 400;
                    res.Message = "Transaction failed: Payment waiting time expired. Please try again.";
                    res.Data = "fail";
                    break;

                case "12":
                    // Transaction failed: Customer's card/account is locked
                    res.StatusCode = 400;
                    res.Message = "Transaction failed: Customer's card/account is locked.";
                    res.Data = "fail";
                    break;

                case "13":
                    // Transaction failed: Incorrect OTP entered by customer
                    res.StatusCode = 400;
                    res.Message = "Transaction failed: Incorrect OTP entered by customer. Please try again.";
                    res.Data = "fail";
                    break;

                case "24":
                    // Transaction failed: Customer canceled the transaction
                    res.StatusCode = 400;
                    res.Message = "Transaction failed: Customer canceled the transaction.";
                    res.Data = "fail";
                    break;

                case "51":
                    // Transaction failed: Customer's account has insufficient balance to complete the transaction
                    res.StatusCode = 400;
                    res.Message = "Transaction failed: Customer's account has insufficient balance to complete the transaction.";
                    res.Data = "fail";
                    break;

                case "65":
                    // Transaction failed: Customer's account has exceeded the daily transaction limit
                    res.StatusCode = 400;
                    res.Message = "Transaction failed: Customer's account has exceeded the daily transaction limit.";
                    res.Data = "fail";
                    break;

                case "75":
                    // Payment gateway maintenance
                    res.StatusCode = 503;
                    res.Message = "Payment gateway is under maintenance.";
                    res.Data = "fail";
                    break;

                case "79":
                    // Transaction failed: Customer entered the wrong payment password too many times
                    res.StatusCode = 400;
                    res.Message = "Transaction failed: Customer entered the wrong payment password too many times. Please try again.";
                    res.Data = "fail";
                    break;

                case "99":
                    // Other errors (not listed in the error codes)
                    res.StatusCode = 500;
                    res.Message = "Other errors: Unspecified issue.";
                    res.Data = "fail";
                    break;

                default:
                    // Unknown response code
                    res.StatusCode = 400;
                    res.Message = "Unknown response code.";
                    res.Data = "fail";
                    break;
            }

            return res;
        }





    }
}
