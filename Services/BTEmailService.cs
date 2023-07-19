using BugTracker.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BugTracker.Services
{
    public class BTEmailService : IEmailSender
    {
        private readonly MailSettings _mailSettings;

        public BTEmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            MimeMessage email = new();

            email.Sender = MailboxAddress.Parse(_mailSettings.EMail);
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };

            email.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.MailHost, _mailSettings.MailPort, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.EMail, _mailSettings.MailPassword);

                await smtp.SendAsync(email);

                smtp.Disconnect(true);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
