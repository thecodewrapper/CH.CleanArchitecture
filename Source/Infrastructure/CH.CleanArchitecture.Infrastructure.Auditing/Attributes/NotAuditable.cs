using System;

namespace CH.CleanArchitecture.Infrastructure.Auditing
{
    /// <summary>
    /// Custom data annotation to handle if a class(table) or property(field) is not auditable.By default is true.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class NotAuditableAttribute : Attribute
    {
        /// <summary>
        /// Defines if class(table) or property(field) is auditable or not
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nonAuditable">Is NOT auditable</param>
        public NotAuditableAttribute(bool nonAuditable = true) {
            this.Enabled = nonAuditable;
        }
    }
}