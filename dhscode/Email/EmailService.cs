using DHSOnlineStore.EmailService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System;

namespace DHSOnlineStore.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
                {
                    // Setup SMTP client credentials
                    client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    client.EnableSsl = _smtpSettings.EnableSsl;

                    // Create the email message
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(recipientEmail);

                    // Send the email asynchronously
                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (SmtpException smtpEx)
            {
                // Handle SMTP specific exceptions (e.g., connection issues, authentication errors)
                Console.Error.WriteLine($"SMTP error: {smtpEx.Message}");
                throw new Exception("Error sending email via SMTP.", smtpEx);
            }
            catch (Exception ex)
            {
                // General exception handler for other potential errors
                Console.Error.WriteLine($"General error: {ex.Message}");
                throw new Exception("An error occurred while sending the email.", ex);
            }
        }
    }
}
