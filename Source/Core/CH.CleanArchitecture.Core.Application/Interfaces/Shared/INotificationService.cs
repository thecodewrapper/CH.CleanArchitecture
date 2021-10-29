using System;
using CH.CleanArchitecture.Common;

namespace CH.CleanArchitecture.Core.Application
{
    /// <summary>
    /// Notification service interface
    /// </summary>
    public partial interface INotificationService
    {
        /// <summary>
        /// Display notification
        /// </summary>
        /// <param name="type">Notification type</param>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        void Notification(NotificationType type, string message, bool encode = true);

        /// <summary>
        /// Display success notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        void SuccessNotification(string message, bool encode = true);

        /// <summary>
        /// Display warning notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        void WarningNotification(string message, bool encode = true);

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encode">A value indicating whether the message should not be encoded</param>
        void ErrorNotification(string message, bool encode = true);

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="exception">Exception</param>
        void ErrorNotification(Exception exception);
    }
}
