using System;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Infrastructure.Constants;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    internal class ScheduledJobService : IScheduledJobService
    {
        private readonly ILogger<ScheduledJobService> _logger;
        private readonly IApplicationConfigurationService _applicationConfigurationService;
        private readonly IAuditHistoryService _auditHistoryService;
        private readonly INotificationService _notificationService;

        public ScheduledJobService(ILogger<ScheduledJobService> logger, IApplicationConfigurationService applicationConfigurationService, IAuditHistoryService auditHistoryService, INotificationService notificationService) {
            _logger = logger;
            _applicationConfigurationService = applicationConfigurationService;
            _auditHistoryService = auditHistoryService;
            _notificationService = notificationService;
        }

        public Result EnrollAuditPurgingJob() {
            var result = new Result();
            try {
                var auditPurgeServiceIntervalHours = _applicationConfigurationService.GetValueInt(AppConfigKeys.AUDIT.PURGE_SERVICE_INTERVAL_HOURS).Unwrap();

                if (auditPurgeServiceIntervalHours > 0) {
                    var cronExpression = $"0 */{auditPurgeServiceIntervalHours} * * *";
                    RecurringJob.AddOrUpdate("AuditHistoryPurge", () => _auditHistoryService.PurgeHistoryAsync(default), cronExpression);
                }
                result.Succeed();

                return result;
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref result, _logger, ex, "Error while trying to enroll the Audit Purging Job");
                return result;
            }
        }

        /// <summary>
        /// Enroll recurring notifications purging job.
        /// </summary>
        public Result EnrollNotificationsPurgingJob() {
            var result = new Result();
            try {
                var notificationsPurgeServiceIntervalHours = _applicationConfigurationService.GetValueInt(AppConfigKeys.NOTIFICATIONS.PURGE_SERVICE_INTERVAL_HOURS).Unwrap();
                if (notificationsPurgeServiceIntervalHours > 0) {
                    var cronExpression = $"0 */{notificationsPurgeServiceIntervalHours} * * *";
                    RecurringJob.AddOrUpdate("NotificationsPurge", () => _notificationService.PurgeNotificationsAsync(default), cronExpression);
                }
                result.Succeed();

                return result;
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref result, _logger, ex, "Error while trying to enroll the Notifications Purging Job");
                return result;
            }
        }
    }
}
