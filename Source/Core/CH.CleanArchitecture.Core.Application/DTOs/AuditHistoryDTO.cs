using System;
using System.Collections.Generic;

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
        public DateTime Created { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Username of the user made the action.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The values after action.
        /// Key contains column name and Value the value of the column.
        /// </summary>
        public Dictionary<string, object> NewValues { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// The values before the action.
        /// Key contains column name and Value the value of the column.
        /// </summary>
        public Dictionary<string, object> OldValues { get; set; } = new Dictionary<string, object>();
    }
}