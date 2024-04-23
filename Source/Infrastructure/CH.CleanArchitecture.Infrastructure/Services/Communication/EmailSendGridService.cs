using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Infrastructure.Constants;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    /// <summary>
    /// Email dispatch service using Twilio SendGrid
    /// </summary>
    internal class EmailSendGridService : IEmailService
    {
        private readonly ILogger<EmailSendGridService> _logger;
        private readonly string _fromEmail;
        private readonly string _apiKey;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="applicationConfigurationService"></param>
        public EmailSendGridService(ILogger<EmailSendGridService> logger, IApplicationConfigurationService applicationConfigurationService) {
            _logger = logger;
            _fromEmail = applicationConfigurationService.GetValue(AppConfigKeys.EMAIL.FROM_ADDRESS).Unwrap().Trim();
            _apiKey = applicationConfigurationService.GetValue(AppConfigKeys.EMAIL.SENDGRID_API_KEY).Unwrap().Trim();
        }

        /// <summary>
        /// Send an email asynchronously
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<Result> SendEmailAsync(string from, string to, string subject, string message) {
            Result result = new();
            try {
                _logger.LogDebug($"Sending email from {from} to {to}. Subject: {subject}");

                var client = new SendGridClient(_apiKey);
                var emailFrom = new EmailAddress(from);
                var emailTo = new EmailAddress(to);
                var msg = MailHelper.CreateSingleEmail(emailFrom, emailTo, subject, string.Empty, message);
                var response = await client.SendEmailAsync(msg);
                if (response.IsSuccessStatusCode) {
                    result.Succeed();
                    _logger.LogInformation($"Email dispatched from {from} to {to}. Subject: {subject}");
                }
                else {
                    result.Fail().WithError($"Unable to send email with subject {subject} to '{to}'");
                    _logger.LogError($"Unable to send email with subject {subject} to '{to}'");
                }
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref result, _logger, ex, "Error while trying to send email");
            }
            return result;
        }

        /// <summary>
        /// Send an email asynchronously from the system's default email address
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<Result> SendEmailAsync(string to, string subject, string message) {
            return this.SendEmailAsync(_fromEmail, to, subject, message);
        }

        /// <summary>
        /// Send an email asynchronously to multiple recipients
        /// </summary>
        /// <param name="from"></param>
        /// <param name="tos"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<Result> SendEmailAsync(string from, List<string> tos, string subject, string message) {
            Result result = new();
            try {
                _logger.LogInformation($"Sending email from {from} to {tos.Count} recipients. Subject: {subject}");
                var client = new SendGridClient(_apiKey);
                var emailFrom = new EmailAddress(from);

                List<EmailAddress> emailToAddresses = new List<EmailAddress>();
                foreach (string email in tos) {
                    emailToAddresses.Add(new EmailAddress(email));
                }

                var msg = MailHelper.CreateSingleEmailToMultipleRecipients(emailFrom, emailToAddresses, subject, string.Empty, message);
                var response = await client.SendEmailAsync(msg);
                if (response.IsSuccessStatusCode) {
                    result.Succeed();
                    _logger.LogInformation($"Email dispatched from {from} to {tos.Count} recipients. Subject: {subject}");
                }
                else {
                    result.Fail().WithError($"Unable to send email with subject {subject} to {tos.Count} recipients");
                    _logger.LogError($"Unable to send email with subject {subject} to {tos.Count} recipients");
                }
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref result, _logger, ex, "Error while trying to send email");
            }
            return result;
        }

        /// <summary>
        /// Send an email asynchronously from the system's default email address to multiple recipients
        /// </summary>
        /// <param name="tos"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<Result> SendEmailAsync(List<string> tos, string subject, string message) {
            return this.SendEmailAsync(_fromEmail, tos, subject, message);
        }
    }
}
