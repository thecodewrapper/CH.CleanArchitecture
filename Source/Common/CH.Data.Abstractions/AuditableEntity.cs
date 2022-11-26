using System;

namespace CH.Data.Abstractions
{
    public abstract class AuditableEntity
    {
        /// <summary>
        /// The date the entity was created
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The date the entity was last modified
        /// </summary>
        public DateTime DateModified { get; set; }

        /// <summary>
        /// The user who created the entity
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// The user who last modified the entity
        /// </summary>
        public string ModifiedBy { get; set; }
    }
}