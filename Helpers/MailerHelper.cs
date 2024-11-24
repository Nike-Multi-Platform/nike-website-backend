using System.Net.Mail;
using System.Net;

namespace nike_website_backend.Helpers
{
    public class MailerHelper
    {
        public async Task SendVerifyEmailAsync(string toEmail, string verificationLink)
        {
            if (string.IsNullOrEmpty(toEmail) || string.IsNullOrEmpty(verificationLink))
            {
                Console.WriteLine("Invalid email or verification link.");
                return;
            }

            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("boquangdieu2003@gmail.com", "bcrozehsoamutvkm"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("boquangdieu2003@gmail.com"),
                    Subject = "Xác thực email của bạn",
                    Body = $"Chào bạn,\n\nVui lòng xác thực email của bạn bằng cách nhấp vào liên kết dưới đây:\n{verificationLink}",
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);

                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}
