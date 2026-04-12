using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MuseumWebApp.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendTicketEmailAsync(
            string toEmail,
            string visitorName,
            string ticketCode,
            string tourTitle,
            string tourDate,
            byte[] qrCodeBytes)
        {
            var smtp = _config.GetSection("Smtp");
            var host     = smtp["Host"]!;
            var port     = int.Parse(smtp["Port"]!);
            var username = smtp["Username"]!;
            var password = smtp["Password"]!;
            var fromName = smtp["FromName"] ?? "Музей";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, username));
            message.To.Add(new MailboxAddress(visitorName, toEmail));
            message.Subject = $"Ваш билет на экскурсию «{tourTitle}»";

            var builder = new BodyBuilder
            {
                HtmlBody = $@"
<div style='font-family:sans-serif; max-width:480px;'>
  <h2>Билет куплен ✅</h2>
  <p>Здравствуйте, <strong>{visitorName}</strong>!</p>
  <p>Ваш билет на экскурсию <strong>«{tourTitle}»</strong> ({tourDate}) успешно оформлен.</p>
  <p><strong>Код билета:</strong> <code style='font-size:1.1em;'>{ticketCode}</code></p>
  <p>Предъявите QR-код или код билета на входе. Оплата производится на месте.</p>
  <img src='cid:qrcode' alt='QR-код' style='width:180px;height:180px;' />
</div>"
            };

            var qrAttachment = builder.LinkedResources.Add("qrcode.png", qrCodeBytes, new MimeKit.ContentType("image", "png"));
            qrAttachment.ContentId = "qrcode";

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
