using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.DTOs;

namespace CH.CleanArchitecture.Core.Application
{
    /// <summary>
    /// Provides facilities to manipulate data from audit. See also <seealso cref="AuditHistoryDTO"/>.
    /// </summary>
    public interface IAuditHistoryService
    {
        /// <summary>
        /// Retrieves details for a specific audit record.
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <returns>Audit record details.</returns>
        Task<Result<AuditHistoryDTO>> Details(int? id);

        /// <summary>
        /// Retrieves all audit records from the database.
        /// </summary>
        /// <returns>IQueryable with audit records.</returns>
        Result<IQueryable<AuditHistoryDTO>> GetAll();

        /// <summary>
        /// Purge the audit history
        /// </summary>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>True if succeeds</returns>
        Task<Result<bool>> PurgeHistoryAsync(CancellationToken cancellationToken = default);
    }
}
