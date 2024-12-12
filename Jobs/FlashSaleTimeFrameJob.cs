using Quartz;
using nike_website_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace nike_website_backend.Jobs
{
    public class FlashSaleTimeFrameJob : IJob
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public FlashSaleTimeFrameJob(DbContextOptions<ApplicationDbContext> options)
        {
            _options = options;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var dbContext = new ApplicationDbContext(_options))
            {
             
                var now = DateTime.Now;
                TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
                DateTime localCurrentDate = TimeZoneInfo.ConvertTime(now, localTimeZone);
                // Kích hoạt các Time Frame đang chờ và đến thời gian bắt đầu
                var timeFramesToActivate = await dbContext.FlashSaleTimeFrames
                    .Where(tf => tf.StartedAt <= localCurrentDate && tf.EndedAt > localCurrentDate && tf.Status == "waiting")
                    .ToListAsync();

                foreach (var timeFrame in timeFramesToActivate)
                {
                    timeFrame.Status = "active";
                    Console.WriteLine($"[{localCurrentDate}] Kích hoạt Flash Sale Time Frame: ID = {timeFrame.FlashSaleTimeFrameId}");
                }

                // Kết thúc các Time Frame đã hết thời gian
                var timeFramesToEnd = await dbContext.FlashSaleTimeFrames
                    .Where(tf => tf.EndedAt <= localCurrentDate && tf.Status == "active")
                    .ToListAsync();

                foreach (var timeFrame in timeFramesToEnd)
                {
                    timeFrame.Status = "ended";
                    Console.WriteLine($"[{localCurrentDate}] Kết thúc Flash Sale Time Frame: ID = {timeFrame.FlashSaleTimeFrameId}");
                }

                // Lưu các thay đổi vào cơ sở dữ liệu
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"[{localCurrentDate}] Hoàn thành cập nhật trạng thái Flash Sale Time Frames.");
            }
        }
    }
}
