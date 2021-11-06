using System.Collections.Generic;
using System.Linq;
using CH.CleanArchitecture.Infrastructure.DbContexts;
using CH.CleanArchitecture.Infrastructure.Models;

namespace CH.CleanArchitecture.Tests
{
    internal class DbInitializer
    {
        #region Application
        public static void Initialize(ApplicationDbContext context) {
            if (context.ApplicationConfigurations.Any()) {
                return;
            }

            Seed(context);
        }

        private static void Seed(ApplicationDbContext context) {
            // Seed additional data according to your application here
            var appConfigs = new List<ApplicationConfigurationEntity>
            {
                new ApplicationConfigurationEntity
                {
                    Id = "dummy1",
                    Value = "dummyvalue1",
                    Description = "dummydesc1"
                },
                new ApplicationConfigurationEntity
                {
                    Id = "dummy2",
                    Value = "dummyvalue2",
                    Description = "dummydesc2"
                },
                new ApplicationConfigurationEntity
                {
                    Id = "dummy3",
                    Value = "dummyvalue3",
                    Description = "dummydesc3"
                },
                new ApplicationConfigurationEntity
                {
                    Id = "dummy4",
                    Value = "dummyvalue3",
                    Description = "dummydesc3"
                }
            };

            context.ApplicationConfigurations.AddRange(appConfigs);
            context.SaveChanges();
        }
        #endregion Application

        #region Identity
        public static void Initialize(IdentityDbContext context) {

        }

        private static void Seed(IdentityDbContext context) {

        }
        #endregion Identity

        #region EventStore
        public static void Initialize(EventStoreDbContext context) {
            if (context.Events.Any()) {
                return;
            }

            Seed(context);
        }

        private static void Seed(EventStoreDbContext context) {
        }
        #endregion EventStore
    }
}