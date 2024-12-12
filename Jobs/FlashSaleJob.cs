using Quartz;
using nike_website_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace nike_website_backend.Jobs
{
    public class FlashSaleJob : IJob
    {
        private readonly ApplicationDbContext _context;

        public FlashSaleJob(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.Now;
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime localCurrentDate = TimeZoneInfo.ConvertTime(now, localTimeZone);
            // Lấy các Flash Sale cần kích hoạt
            var flashSalesToActivate = await _context.FlashSales
                .Where(f => f.StartedAt <= localCurrentDate && f.EndedAt > localCurrentDate && f.Status == "waiting")
                .ToListAsync();

            foreach (var flashSale in flashSalesToActivate)
            {
                flashSale.Status = "active"; 
                Console.WriteLine($"[{localCurrentDate}] Kích hoạt Flash Sale: ID = {flashSale.FlashSaleId}, Tên = {flashSale.FlashSaleName}");
            }

            // Lấy các Flash Sale cần kết thúc
            var flashSalesToEnd = await _context.FlashSales
                .Where(f => f.EndedAt <= localCurrentDate && f.Status == "active")
                .ToListAsync();

            foreach (var flashSale in flashSalesToEnd)
            {
                flashSale.Status = "ended";
                Console.WriteLine($"[{localCurrentDate}] Kết thúc Flash Sale: ID = {flashSale.FlashSaleId}, Tên = {flashSale.FlashSaleName}");
            }

            await _context.SaveChangesAsync();

            Console.WriteLine($"[{localCurrentDate}] Hoàn thành cập nhật trạng thái Flash Sale.");
        }
    }
}
