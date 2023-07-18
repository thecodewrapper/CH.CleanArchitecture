using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CH.CleanArchitecture.Infrastructure.Auditing
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Ensures the automatic auditing history.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="username">User made the action.</param>
        public static void EnsureAuditHistory(this DbContext context, string username) {
            var entries = context.ChangeTracker.Entries().Where(e => !AuditUtilities.IsAuditDisabled(e.Entity.GetType()) && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)).ToArray();
            foreach (var entry in entries) {
                context.Add(entry.CreateAuditHistory(username));
            }
        }

        private static AuditHistory CreateAuditHistory(this EntityEntry entry, string username) {
            var history = new AuditHistory
            {
                TableName = entry.Metadata.GetTableName(),
                Username = username
            };

            // Get the mapped properties for the entity type.
            // (include shadow properties, not include navigations & references)
            var properties = entry.Properties.Where(p => !AuditUtilities.IsAuditDisabled(p.EntityEntry.Entity.GetType(), p.Metadata.Name));

            foreach (var prop in properties) {
                string propertyName = prop.Metadata.Name;
                if (prop.Metadata.IsPrimaryKey()) {
                    history.AutoHistoryDetails.NewValues[propertyName] = prop.CurrentValue;
                    continue;
                }

                switch (entry.State) {
                    case EntityState.Added:
                        history.RowId = "0";
                        history.Kind = EntityState.Added;
                        history.AutoHistoryDetails.NewValues.Add(propertyName, prop.CurrentValue);
                        break;

                    case EntityState.Modified:
                        history.RowId = entry.PrimaryKey();
                        history.Kind = EntityState.Modified;
                        history.AutoHistoryDetails.OldValues.Add(propertyName, prop.OriginalValue);
                        history.AutoHistoryDetails.NewValues.Add(propertyName, prop.CurrentValue);
                        break;

                    case EntityState.Deleted:
                        history.RowId = entry.PrimaryKey();
                        history.Kind = EntityState.Deleted;
                        history.AutoHistoryDetails.OldValues.Add(propertyName, prop.OriginalValue);
                        break;
                }
            }

            history.Changed = JsonSerializer.Serialize(history.AutoHistoryDetails);

            return history;
        }

        private static string PrimaryKey(this EntityEntry entry) {
            var key = entry.Metadata.FindPrimaryKey();

            var values = new List<object>();
            foreach (var property in key.Properties) {
                var value = entry.Property(property.Name).CurrentValue;
                if (value != null) {
                    values.Add(value);
                }
            }

            return string.Join(",", values);
        }
    }
}