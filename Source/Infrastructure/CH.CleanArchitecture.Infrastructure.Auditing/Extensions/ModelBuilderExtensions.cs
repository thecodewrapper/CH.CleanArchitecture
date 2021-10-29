using Microsoft.EntityFrameworkCore;

namespace CH.CleanArchitecture.Infrastructure.Auditing
{
    /// <summary>
    /// Represents a plugin for Microsoft.EntityFrameworkCore to support automatically recording data changes history.
    /// </summary>
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Enables auditing change history.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/> to enable auto history functionality.</param>
        /// <returns>The <see cref="ModelBuilder"/> to enable auto history functionality.</returns>
        public static ModelBuilder EnableAuditHistory(this ModelBuilder modelBuilder) {
            modelBuilder.Entity<AuditHistory>().ToTable("AuditHistory", "Audit").Ignore(t => t.AutoHistoryDetails);
            modelBuilder.Entity<AuditHistory>(b =>
            {
                b.Property(c => c.Id).UseIdentityColumn(); //TODO: Possibly change this to avoid integer overflow, or cleanup every once in a while
                b.Property(c => c.RowId).IsRequired().HasMaxLength(128);
                b.Property(c => c.TableName).IsRequired().HasMaxLength(128);
                //b.Property(c => c.Changed).HasMaxLength(2048);
                b.Property(c => c.Username).HasMaxLength(128);
                // This MSSQL only
                //b.Property(c => c.Created).HasDefaultValueSql("getdate()");
            });

            return modelBuilder;
        }
    }
}