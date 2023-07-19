using System;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Common.Constants;
using CH.CleanArchitecture.Core.Application;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    internal class ScheduledJobService : IScheduledJobService
    {
        private readonly ILogger<ScheduledJobService> _logger;
        private readonly IApplicationConfigurationService _applicationConfigurationService;
        private readonly IAuditHistoryService _auditHistoryService;

        public ScheduledJobService(ILogger<ScheduledJobService> logger, IApplicationConfigurationService applicationConfigurationService, IAuditHistoryService auditHistoryService) {
            _logger = logger;
            _applicationConfigurationService = applicationConfigurationService;
            _auditHistoryService = auditHistoryService;
        }

        public Result<bool> EnrollAuditPurgingJob() {
            var serviceResult = new Result<bool>();
            try {
                var auditPurgeServiceIntervalHours = _applicationConfigurationService.GetValueInt(AppConfigKeys.AUDIT.PURGE_SERVICE_INTERVAL_HOURS).Unwrap();

                if (auditPurgeServiceIntervalHours > 0) {
                    var cronExpression = $"0 */{auditPurgeServiceIntervalHours} * * *";
                    RecurringJob.AddOrUpdate("AuditHistoryPurge", () => _auditHistoryService.PurgeHistoryAsync(default), cronExpression);
                }
                serviceResult.Data = true;

                return serviceResult;
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to enroll the Audit Purging Job");
                return serviceResult;
            }
        }
    }
}
