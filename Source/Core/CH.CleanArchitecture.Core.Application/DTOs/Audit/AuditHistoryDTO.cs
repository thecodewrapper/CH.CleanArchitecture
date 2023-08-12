using System;

namespace CH.CleanArchitecture.Core.Application.DTOs
{
    /// <summary>
    /// DTO representing a single audit record
    /// </summary>
    public class AuditHistoryDTO
    {
        /// <summary>
        /// Gets or sets the primary key.
        /// </summary>
        /// <value>The id.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the change kind.
        /// </summary>
        /// <value>The change kind.</value>
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the create time.
        /// </summary>
        /// <value>The create time.</value>
        public DateTime Created { get; set; }

        /// <summary>
        /// Username of the user made the action.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Contains the old values and new values of the audit history record
        /// </summary>
        public AuditHistoryDetailsDTO AuditHistoryDetails { get; set; }
    }
}