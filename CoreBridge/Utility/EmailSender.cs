using Microsoft.AspNetCore.Identity.UI.Services;
using CodeBridge.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using CoreBridge.Models;

namespace CoreBridge.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {

            string apiKey = Environment.GetEnvironmentVariable("SENDGRID_KEY");
            ISendGridClient _sendGridClient = new SendGridClient(apiKey);

            string fromEmailAddress = Environment.GetEnvironmentVariable("SENDGRID_FROM_EMAIL");
            if (string.IsNullOrEmpty(fromEmailAddress))
                throw new InvalidParameterException("SENDGRID_FROM_EMAIL is not defined.");

            EmailAddress from = new(fromEmailAddress, "Automatic e-mail address.");

            EmailAddress to = new(email, "okyakusama");

            SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
            var result = _sendGridClient.SendEmailAsync(msg);

            if (!result.IsCompletedSuccessfully)
            {
                string body = result.Result.Body.ReadAsStringAsync().Result;

            }

            return Task.CompletedTask;
        }
    }
}
