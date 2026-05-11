using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SMRS.Web.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, string toName, string verificationLink);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendVerificationEmailAsync(string toEmail, string toName, string verificationLink)
        {
            var host = _config["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            var port = int.Parse(_config["EmailSettings:SmtpPort"] ?? "587");
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderPassword = _config["EmailSettings:SenderPassword"];
            var senderName = _config["EmailSettings:SenderName"];

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword) || senderPassword == "YOUR_APP_PASSWORD_HERE")
            {
                _logger.LogWarning("Email sending bypassed. SMTP credentials are not configured properly.");
                return;
            }

            try
            {
                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = "Action Required: Verify your SMRS Application",
                    IsBodyHtml = true,
                    Body = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e2e8f0; border-radius: 8px;'>
                            <h2 style='color: #4f46e5;'>Verify Your Application</h2>
                            <p>Hello {toName},</p>
                            <p>Thank you for your interest in Saint Mary's Orthodox College. To continue your application, please verify your email address by clicking the secure link below.</p>
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{verificationLink}' style='background-color: #4f46e5; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: bold;'>
                                    Verify & Continue Application
                                </a>
                            </div>
                            <p style='color: #64748b; font-size: 14px;'>This link will expire in 24 hours.</p>
                            <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 20px 0;' />
                            <p style='color: #94a3b8; font-size: 12px;'>Saint Mary's Recruitment System (SMRS)</p>
                        </div>"
                };

                mailMessage.To.Add(new MailAddress(toEmail, toName));

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Verification email sent successfully to {email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {email}", toEmail);
                // Optionally throw or handle it
            }
        }
    }
}
