using System.Collections.Generic;

namespace CH.CleanArchitecture.Infrastructure.Auditing
{
    /// <summary>
    /// Changed Values details
    /// </summary>
    public class AuditHistoryDetails
    {
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