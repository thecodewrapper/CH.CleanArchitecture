using System;
using CH.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CH.CleanArchitecture.Infrastructure.Auditing
{
    /// <summary>
    /// DB Entity of audit
    /// </summary>
    [NotAuditable]
    public class AuditHistory : IDataEntity<int>
    {
        /// <summary>
        /// Gets or sets the primary key.
        /// </summary>
        /// <value>The id.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the source row id.
        /// </summary>
        /// <value>The source row id.</value>
        public string RowId { get; set; }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the json about the changing.
        /// </summary>
        /// <value>The json about the changing.</value>
        public string Changed { get; set; }

        /// <summary>
        /// Gets or sets the change kind.
        /// </summary>
        /// <value>The change kind.</value>
        public EntityState Kind { get; set; }

        /// <summary>
        /// Gets or sets the create time.
        /// </summary>
        /// <value>The create time.</value>
        public DateTime Created { get; set; } = DateTime.Now;

        /// <summary>
        /// Username of the user made the action.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The structured values contained on the property <see cref="Changed"/>.
        /// </summary>
        public AuditHistoryDetails AuditHistoryDetails { get; set; } = new AuditHistoryDetails();
    }
}