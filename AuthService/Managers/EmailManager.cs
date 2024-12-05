using AuthService.Interfaces;
using System.Net.Mail;
using System.Net;

namespace AuthService.Managers
{
    public class EmailManager : IEmailManager
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("dashakoshelek@gmail.com", "ntbm jfgi upmt lzyr"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("dashakoshelek@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };

            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
