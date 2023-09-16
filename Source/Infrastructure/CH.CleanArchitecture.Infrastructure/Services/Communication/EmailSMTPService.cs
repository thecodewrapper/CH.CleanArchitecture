using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Common.Constants;
using CH.CleanArchitecture.Core.Application;
using Microsoft.Extensions.Logging;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    internal class EmailSMTPService : IEmailService
    {
        #region Private Fields

        private readonly IApplicationConfigurationService _applicationConfigurationService;
        private readonly string _fromEmail;
        private readonly string _bccEmail;
        private readonly bool _useBcc;
        private readonly SmtpClient _client;
        private readonly ILogger<EmailSMTPService> _logger;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Service constructor.
        /// </summary>
        /// <param name="logger">Logger object. See <see cref="Logger{P}"/>  for more information.</param>
        /// <param name="applicationConfigurationService">Application Configuration service. <see cref="IApplicationConfigurationService"/> for more information.</param>
        public EmailSMTPService(ILogger<EmailSMTPService> logger, IApplicationConfigurationService applicationConfigurationService) {
            _logger = logger;
            _applicationConfigurationService = applicationConfigurationService;
            _fromEmail = _applicationConfigurationService.GetValue(AppConfigKeys.EMAIL.FROM_ADDRESS).Unwrap().Trim();
            _bccEmail = _applicationConfigurationService.GetValue(AppConfigKeys.EMAIL.BCC_ADDRESS).Unwrap().Trim();
            _useBcc = _applicationConfigurationService.GetValueBool(AppConfigKeys.EMAIL.USE_BCC).Unwrap();
            _client = GetClient();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Send an email asynchronously
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<Result> SendEmailAsync(string from, string to, string subject, string message) {
            Result result = new Result();
            try {
                _logger.LogDebug($"Sending email from {from} to {to}. Subject: {subject}");
                var mailMessage = ConstructMail(from, to, subject, message);
                await _client.SendMailAsync(mailMessage);
                _logger.LogDebug($"Email dispatched from {from} to {to}. Subject: {subject}");
                result.Succeed();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref result, _logger, ex, $"Error while trying to send email to {to}. Subject: {subject}");
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
            Result result = new Result();
            try {
                _logger.LogDebug($"Sending email from {from} to {tos.Count} recipients. Subject: {subject}");
                var mailMessage = ConstructMail(from, tos, subject, message);
                await _client.SendMailAsync(mailMessage);
                _logger.LogDebug($"Email dispatched from {from} to {tos.Count} recipients. Subject: {subject}");
                result.Succeed();
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

        #endregion Public Methods

        #region Private Methods

        private MailMessage ConstructMail(string addressFrom, string AddressTo, string messageSubject, string messageBody) {
            _logger.LogTrace($"Entering {nameof(ConstructMail)}");
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(addressFrom);
            mailMessage.To.Add(AddressTo);
            mailMessage.Subject = messageSubject;
            mailMessage.Body = messageBody;
            mailMessage.IsBodyHtml = true;
            if (_useBcc && !string.IsNullOrWhiteSpace(_bccEmail))
                mailMessage.Bcc.Add(_bccEmail);

            return mailMessage;
        }

        private MailMessage ConstructMail(string addressFrom, List<string> addressesTo, string messageSubject, string messageBody) {
            _logger.LogTrace($"Entering {nameof(ConstructMail)}");
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(addressFrom);
            mailMessage.To.Add(string.Join(',', addressesTo));
            mailMessage.Subject = messageSubject;
            mailMessage.Body = messageBody;
            mailMessage.IsBodyHtml = true;
            if (_useBcc && !string.IsNullOrWhiteSpace(_bccEmail))
                mailMessage.Bcc.Add(_bccEmail);

            return mailMessage;
        }

        private SmtpClient GetClient() {
            _logger.LogTrace($"Entering {nameof(GetClient)}");
            try {
                #region Parse Settings
                var serviceResult = _applicationConfigurationService.GetMultiple(AppConfigKeys.EMAIL.SMTP_SETTINGS);
                if (serviceResult.IsFailed || serviceResult.Data.Length == 0)
                    throw new Exception("Error while retrieving SMTP Settings from DB.");
                string[] smtpSettingsArray = serviceResult.Data;
                if (smtpSettingsArray.Length < 6)
                    throw new Exception("SMTP Settings configuration is not correct");
                string smtpHost = smtpSettingsArray[0];
                int.TryParse(smtpSettingsArray[1], out int smtpPort);
                bool.TryParse(smtpSettingsArray[2], out bool smtpEnableSSL);
                bool.TryParse(smtpSettingsArray[3], out bool smtpUseDefaultCredentials);
                string smtpUsername = smtpSettingsArray[4];
                string smtpPassword = smtpSettingsArray[5];

                #endregion Parse Settings

                _logger.LogDebug("SMTP settings parsed successfully.");
                SmtpClient client = new SmtpClient(smtpHost);
                client.Port = smtpPort;
                client.EnableSsl = smtpEnableSSL;
                client.UseDefaultCredentials = smtpUseDefaultCredentials;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                return client;
            }
            catch {
                throw;
            }
        }

        #endregion Private Methods
    }
}
