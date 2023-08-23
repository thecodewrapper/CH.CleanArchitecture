using CH.CleanArchitecture.Presentation.Web.Enumerations;

namespace CH.CleanArchitecture.Presentation.Web.Models
{
    /// <summary>
    /// Message structure
    /// </summary>
    public struct TempNotificationData
    {
        /// <summary>
        /// Message type (success/warning/error)
        /// </summary>
        public TempNotificationType Type { get; set; }

        /// <summary>
        /// Message text
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Get a sets a value indicating whether the message should not be HTML encoded
        /// </summary>
        public bool Encode { get; set; }
    }
}
