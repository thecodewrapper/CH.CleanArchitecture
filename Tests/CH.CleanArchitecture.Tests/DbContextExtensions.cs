using Microsoft.EntityFrameworkCore;

namespace CH.CleanArchitecture.Tests
{
    public static class DbContextExtensions
    {
        public static void DetachAll(this DbContext dbContext) {
            foreach (var dbEntityEntry in dbContext.ChangeTracker.Entries().ToArray()) {
                if (dbEntityEntry.Entity != null) {
                    dbEntityEntry.State = EntityState.Detached;
                }
            }
        }
    }
}
