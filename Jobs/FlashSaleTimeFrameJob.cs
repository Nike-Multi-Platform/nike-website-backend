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

                // Kích hoạt các Time Frame đang chờ và đến thời gian bắt đầu
                var timeFramesToActivate = await dbContext.FlashSaleTimeFrames
                    .Where(tf => tf.StartedAt <= now && tf.EndedAt > now && tf.Status == "waiting")
                    .ToListAsync();

                foreach (var timeFrame in timeFramesToActivate)
                {
                    timeFrame.Status = "active";
                    Console.WriteLine($"[{now}] Kích hoạt Flash Sale Time Frame: ID = {timeFrame.FlashSaleTimeFrameId}");
                }

                // Kết thúc các Time Frame đã hết thời gian
                var timeFramesToEnd = await dbContext.FlashSaleTimeFrames
                    .Where(tf => tf.EndedAt <= now && tf.Status == "active")
                    .ToListAsync();

                foreach (var timeFrame in timeFramesToEnd)
                {
                    timeFrame.Status = "ended";
                    Console.WriteLine($"[{now}] Kết thúc Flash Sale Time Frame: ID = {timeFrame.FlashSaleTimeFrameId}");
                }

                // Lưu các thay đổi vào cơ sở dữ liệu
                await dbContext.SaveChangesAsync();
                Console.WriteLine($"[{now}] Hoàn thành cập nhật trạng thái Flash Sale Time Frames.");
            }
        }
    }
}
