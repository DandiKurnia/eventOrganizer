using StaffEventOrganizer.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace StaffEventOrganizer.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();

                var senderEmail = _config["EmailSettings:SenderEmail"];
                var senderName = _config["EmailSettings:SenderName"];
                var smtpServer = _config["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"] ?? "587");
                var appPassword = _config["EmailSettings:AppPassword"];

                email.From.Add(new MailboxAddress(senderName, senderEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = body
                };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(senderEmail, appPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }

        public async Task SendVendorRequestEmailAsync(
            string vendorEmail,
            string vendorName,
            string clientName,
            string packageName,
            DateTime eventDate)
        {
            var subject = $"[Event Organizer] Request Baru untuk Event {eventDate:dd MMMM yyyy}";

            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .info-table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        .info-table td {{ padding: 10px; border-bottom: 1px solid #ddd; }}
        .info-table td:first-child {{ font-weight: bold; width: 40%; }}
        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
        .btn {{ display: inline-block; padding: 12px 24px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Request Event Baru</h1>
        </div>
        <div class='content'>
            <p>Halo <strong>{vendorName}</strong>,</p>
            <p>Anda mendapatkan request baru untuk event dengan detail sebagai berikut:</p>

            <table class='info-table'>
                <tr>
                    <td>Nama Client</td>
                    <td>{clientName}</td>
                </tr>
                <tr>
                    <td>Paket Event</td>
                    <td>{packageName}</td>
                </tr>
                <tr>
                    <td>Tanggal Event</td>
                    <td>{eventDate:dd MMMM yyyy}</td>
                </tr>
            </table>

            <p>Silakan login ke sistem untuk melihat detail lebih lanjut dan mengkonfirmasi ketersediaan Anda.</p>

            <p>Terima kasih atas kerjasamanya!</p>
        </div>
        <div class='footer'>
            <p>Email ini dikirim otomatis oleh sistem Event Organizer.</p>
            <p>Jangan reply email ini.</p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(vendorEmail, subject, body);
        }
    }
}
