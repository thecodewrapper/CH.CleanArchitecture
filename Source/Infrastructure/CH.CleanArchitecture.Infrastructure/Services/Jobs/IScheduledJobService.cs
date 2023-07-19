using CH.CleanArchitecture.Common;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    public interface IScheduledJobService
    {
        /// <summary>
        /// Enrolls the audit purging job
        /// </summary>
        Result<bool> EnrollAuditPurgingJob();
    }
}
