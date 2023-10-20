using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application.DTOs;

namespace CH.CleanArchitecture.Core.Application
{
    /// <summary>
    /// Notification service interface
    /// </summary>
    public partial interface INotificationService
    {
        /// <summary>
        /// Sends a notification to recipients, specified by the type of notification
        /// </summary>
        /// <param name="dto">The request parameters object</param>
        /// <returns></returns>
        Task<Result<int>> SendNotificationAsync(SendNotificationDTO dto);

        /// <summary>
        /// Get all user notifications for the specified user
        /// </summary>
        /// <param name="user">User for which notifications will be retrieved.</param>
        /// <returns>IQueryable with Notification DTO's. See <see cref="NotificationDTO"/> for more information.</returns>
        Result<IQueryable<NotificationDTO>> GetAllForUser(string user);

        /// <summary>
        /// Get all user notifications
        /// </summary>
        /// <returns>IQueryable with Notification DTO's. See <see cref="NotificationDTO"/> for more information.</returns>
        Result<IQueryable<NotificationDTO>> GetAll();

        /// <summary>
        /// Purge the notifications DB table based on the configuration variable NotificationsPurgeHistoryTableInterval.
        /// </summary>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        Task<Result> PurgeNotificationsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks all notifications for the user as read
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Result> MarkAllAsReadForUserAsync(string user);

        /// <summary>
        /// Marks the notification specified by <paramref name="id"/> as read
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result> MarkAsReadAsync(Guid id);
    }
}
