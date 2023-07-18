using System.Linq;
using CH.CleanArchitecture.Common.Constants;
using CH.CleanArchitecture.Infrastructure.DbContexts;
using CH.CleanArchitecture.Infrastructure.Models;
using CH.EventStore.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    internal class DbInitializerService : IDbInitializerService
    {
        #region Fields
        private readonly IdentityDbContext _identityContext;
        private readonly ApplicationDbContext _applicationContext;
        private readonly EventStoreDbContext _eventStoreContext;

        #endregion Fields

        #region Constructor

        public DbInitializerService(IdentityDbContext identityContext,
            ApplicationDbContext applicationContext,
            EventStoreDbContext eventStoreContext) {
            _identityContext = identityContext;
            _applicationContext = applicationContext;
            _eventStoreContext = eventStoreContext;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Execute migrations
        /// </summary>
        public void Migrate() {
            _identityContext.Database.Migrate();
            _applicationContext.Database.Migrate();
            _eventStoreContext.Database.Migrate();
        }

        public void Seed() {
            var applicationRoles = _identityContext.Roles;
            if (!applicationRoles.Any()) {
                applicationRoles.Add(new ApplicationRole() { Id = "e5adff57-b654-4f30-b6a7-c818e86cda8e", ConcurrencyStamp = "6a1bfaad-4414-4593-895c-a100aedd1741", Name = "User", NormalizedName = "USER" });
                applicationRoles.Add(new ApplicationRole() { Id = "40e668a2-8a53-4907-817c-e4f8c8f72fb4", ConcurrencyStamp = "b47ee50f-0b94-42bd-858c-2f4bacd4bb50", Name = "Admin", NormalizedName = "ADMIN" });
                applicationRoles.Add(new ApplicationRole() { Id = "8fa3842a-98a4-475b-8926-fce6efdc3e6f", ConcurrencyStamp = "b3a92cb9-8d66-47d4-9670-4e110447b887", Name = "SuperAdmin", NormalizedName = "SUPERADMIN" });
            }

            var appConfigs = _applicationContext.ApplicationConfigurations;
            if (!_applicationContext.ApplicationConfigurations.Any()) {
                var applicationConfigurations = new ApplicationConfigurationEntity[] {
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.AUDIT.PURGE_HISTORYTABLE_INTERVAL, Value = "0", Description = "Declares how many days the system keeps the audit history. Set it to 0 if you wish to leave the audit history for ever." },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.AUDIT.PURGE_SERVICE_INTERVAL_HOURS, Value = "24", Description = "The interval in hours that the purging of Audit History will get place. Set it to 0 if you actually want to disable the service. Please keep in mind that you must manually restart the maintenance windows service and update the Hangfire job in order for your change to take place." },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.EMAIL.SMTP_SETTINGS, Value = "smtp.test.com|587|true|false|username|Password", Description = "SMTP settings used to send emails. Please use this format for the configuration value : '{SMTP host Address}|{SMTP host port}|{Enable SSL}|{Use Default Credentials}|Username|Password'." },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.EMAIL.FROM_ADDRESS, Value = "test@test.com", Description = "The 'From' email address for all emails send through the system" },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.CRYPTO.JWT_SYMMETRIC_KEY, IsEncrypted = false, Value = "ENTER SYMMETRIC KEY HERE", Description = "The symmetric key to use for JWT signing" },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.CRYPTO.JWT_ISSUER, IsEncrypted = false, Value = "ENTER ISSUER HERE", Description = "The issuer for the generated JWT tokens" },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.CRYPTO.JWT_AUTHORITY, IsEncrypted = false, Value = "ENTER AUTHORITY HERE", Description = "The issuer for the generated JWT tokens" },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.SECURITY.GOOGLE_RECAPTCHA_CLIENTKEY, Value = @"{clientKey}", Description = "Use this in the HTML code your site serves to users"},
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.SECURITY.GOOGLE_RECAPTCHA_SECRETKEY, Value = @"{secretKey}", Description = "Use this for communication between the site and Google"},
                };
                appConfigs.AddRange(applicationConfigurations);
            }

            _identityContext.SaveChanges();
            _applicationContext.SaveChanges();
        }

        #endregion Public Methods
    }
}