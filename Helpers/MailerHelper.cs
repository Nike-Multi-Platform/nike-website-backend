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
                    Subject = "Verify Your Email Address",
                    Body = $"Hello,\n\nPlease verify your email address by clicking the link below:\n{verificationLink}\n\nIf you did not request this verification, please ignore this email.",
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

        public async Task SendForgotPasswordEmailAsync(string toEmail, string resetPasswordLink)
        {

            if (string.IsNullOrEmpty(toEmail) || string.IsNullOrEmpty(resetPasswordLink))
            {
                Console.WriteLine("Invalid email or resetPassword link.");
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
                    Subject = "Reset Your Password",
                    Body = $"Hello,\n\nPlease reset your password by clicking the link below:\n{resetPasswordLink}\n\nIf you did not request a password reset, please ignore this email.",
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
