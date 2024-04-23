using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Infrastructure.Constants;
using CH.CleanArchitecture.Infrastructure.DbContexts;
using CH.CleanArchitecture.Infrastructure.Models;
using CH.EventStore.EntityFramework;

namespace CH.CleanArchitecture.Tests
{
    internal class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context) {
            AddApplicationConfigurations(context);
        }

        public static void Initialize(EventStoreDbContext context) {
            if (context.Events.Any()) {
                return;
            }
        }

        public static void Initialize(IdentityDbContext context) {
            AddUsers(context);
        }

        private static void AddUsers(IdentityDbContext context) {

            var activeUser = new ApplicationUser
            {
                UserName = "activeUser",
                Name = "Active User",
                Surname = "Active User",
                Id = "0a9da724-e527-46ca-9a6d-1d019b8f6f95",
                Email = "test@test.it",
                IsActive = true
            };

            var inactiveUser = new ApplicationUser
            {
                UserName = "inactiveUser",
                Name = "Inactive User",
                Surname = "Inactive User",
                Id = "0a9da724-e527-46ca-9a6d-1d019b8f6f96",
                Email = "test@test.it",
                IsActive = false
            };

            var superAdminUser = new ApplicationUser
            {
                UserName = "superAdminUser",
                Name = "Super Admin",
                Surname = "Super Admin",
                Id = Guid.NewGuid().ToString(),
                Email = "test@test.it",
                IsActive = true
            };

            superAdminUser.Roles.Add(new ApplicationUserRole
            {
                Role = new ApplicationRole { Name = RoleEnum.SuperAdmin.ToString() }
            });

            var basicUser = new ApplicationUser
            {
                UserName = "basicUser",
                Name = "Super Admin",
                Surname = "Super Admin",
                Id = Guid.NewGuid().ToString(),
                Email = "test@test.it",
                IsActive = true
            };

            basicUser.Roles.Add(new ApplicationUserRole
            {
                Role = new ApplicationRole { Name = RoleEnum.User.ToString() }
            });

            var users = new List<ApplicationUser>
            {
                activeUser,
                inactiveUser,
                superAdminUser,
                basicUser
            };
            context.Users.AddRange(users);
            context.SaveChanges();
        }

        private static void AddApplicationConfigurations(ApplicationDbContext context) {
            var appConfigs = new List<ApplicationConfigurationEntity>
            {
                new ApplicationConfigurationEntity { Id = AppConfigKeys.CRYPTO.JWT_SYMMETRIC_KEY, IsEncrypted = false, Value = "testsecrettestsecrettestsecrettestsecrettestsecret", Description = "The symmetric key to use for JWT signing" },
                new ApplicationConfigurationEntity { Id = AppConfigKeys.CRYPTO.JWT_ISSUER, IsEncrypted = false, Value = "Issuer", Description = "The issuer for the generated JWT tokens" },
                new ApplicationConfigurationEntity { Id = AppConfigKeys.CRYPTO.JWT_AUTHORITY, IsEncrypted = false, Value = "Authority", Description = "The issuer for the generated JWT tokens" },
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
    }
}