using System;
using System.Linq;
using CH.CleanArchitecture.Common.Constants;
using CH.CleanArchitecture.Infrastructure.DbContexts;
using CH.CleanArchitecture.Infrastructure.Models;
using CH.EventStore.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    internal class DbInitializerService : IDbInitializerService
    {
        #region Fields
        private readonly IdentityDbContext _identityContext;
        private readonly ApplicationDbContext _applicationContext;
        private readonly EventStoreDbContext _eventStoreContext;
        private readonly IConfiguration _configuration;

        #endregion Fields

        #region Constructor

        public DbInitializerService(IdentityDbContext identityContext,
            ApplicationDbContext applicationContext,
            EventStoreDbContext eventStoreContext, IConfiguration configuration) {
            _identityContext = identityContext;
            _applicationContext = applicationContext;
            _eventStoreContext = eventStoreContext;
            _configuration = configuration;
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
                var userRole = new ApplicationRole() { Id = "e5adff57-b654-4f30-b6a7-c818e86cda8e", ConcurrencyStamp = "6a1bfaad-4414-4593-895c-a100aedd1741", Name = "User", NormalizedName = "USER" };
                var adminRole = new ApplicationRole() { Id = "40e668a2-8a53-4907-817c-e4f8c8f72fb4", ConcurrencyStamp = "b47ee50f-0b94-42bd-858c-2f4bacd4bb50", Name = "Admin", NormalizedName = "ADMIN" };
                var superAdminRole = new ApplicationRole() { Id = "8fa3842a-98a4-475b-8926-fce6efdc3e6f", ConcurrencyStamp = "b3a92cb9-8d66-47d4-9670-4e110447b887", Name = "SuperAdmin", NormalizedName = "SUPERADMIN" };
                applicationRoles.AddRange(userRole, adminRole, superAdminRole);

                CreateSuperAdminUser(userRole.Id, adminRole.Id, superAdminRole.Id);
            }

            var appConfigs = _applicationContext.ApplicationConfigurations;
                var applicationConfigurations = new ApplicationConfigurationEntity[] {
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.AUDIT.PURGE_HISTORYTABLE_INTERVAL_DAYS, Value = "60", Description = "Declares how many days the system keeps the audit history. Set it to 0 if you wish to leave the audit history for ever." },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.AUDIT.PURGE_SERVICE_INTERVAL_HOURS, Value = "23", Description = "The interval in hours that the purging of Audit History will get place. Set it to 0 if you actually want to disable the service. Please keep in mind that you must manually restart the maintenance windows service and update the Hangfire job in order for your change to take place." },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.EMAIL.SMTP_SETTINGS, Value = "smtp.test.com|587|true|false|username|Password", Description = "SMTP settings used to send emails. Please use this format for the configuration value : '{SMTP host Address}|{SMTP host port}|{Enable SSL}|{Use Default Credentials}|Username|Password'." },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.EMAIL.FROM_ADDRESS, Value = "test@test.com", Description = "The 'From' email address for all emails send through the system" },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.EMAIL.BCC_ADDRESS, Value = "", Description = "The email address to bcc all outgoing emails. Leave empty to disable."},
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.EMAIL.USE_BCC, Value = "false", Description = "Whether to attach a bcc on all outgoing emails. The BCC address is determined by [EmailBccAddress] configuration."},
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.EMAIL.SENDGRID_API_KEY, Value = "{apiKey}", Description = "[Optional] The API key used for the SendGrid email service"},
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.CRYPTO.JWT_SYMMETRIC_KEY, IsEncrypted = false, Value = "ENTER SYMMETRIC KEY HERE", Description = "The symmetric key to use for JWT signing" },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.CRYPTO.JWT_ISSUER, IsEncrypted = false, Value = "ENTER ISSUER HERE", Description = "The issuer for the generated JWT tokens" },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.CRYPTO.JWT_AUTHORITY, IsEncrypted = false, Value = "ENTER AUTHORITY HERE", Description = "The issuer for the generated JWT tokens" },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.SECURITY.GOOGLE_RECAPTCHA_CLIENTKEY, Value = @"{clientKey}", Description = "Use this in the HTML code your site serves to users"},
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.SECURITY.GOOGLE_RECAPTCHA_SECRETKEY, Value = @"{secretKey}", Description = "Use this for communication between the site and Google"},
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.EVENTSTORE.SNAPSHOT_FREQUENCY, IsEncrypted = false, Value = "50", Description = "The number of events after which a snapshot in the event store will be taken"},
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.NOTIFICATIONS.PURGE_HISTORYTABLE_INTERVAL, Value = "0", Description = "Declares how many days the system keeps the user notifications. Set it to 0 if you wish to leave the notifications for ever." },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.NOTIFICATIONS.PURGE_SERVICE_INTERVAL_HOURS, Value = "23", Description = "The interval in hours that the purging of User Notifications will get place. Set it to 0 if you actually want to disable the service." },
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.AZURE.STORAGE_CONNECTION_STRING, Value = "{connectionString}", Description = "Connection string for the Azure Storage account."},
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.AZURE.BLOB_STORAGE_BASE_URI, Value = "", Description = "The Azure Blob Storage base URI"},
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.AZURE.STORAGE_USE_PASSWORDLESS_AUTHENTICATION, Value = "false", Description = "Whether to use passwordless authentication (DefaultAzureCredential), or connection string, for connecting to Azure Storage. If this is true, the application configuration for Azure Storage Account Name needs to be set."},
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.AWS.S3_BUCKET_NAME, Value = "", Description = "The AWS S3 bucket name."},
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.AWS.S3_REGION, Value = "{systemName}", Description = "The AWS S3 region. Use the system name of the region'. Example: 'eu-west-1'"},
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.AWS.S3_ENDPOINT_FORMAT, Value = "https://{0}.s3.{1}.amazonaws.com/", Description = "The string format of S3 public url endpoint. Use '{0}' as a placeholder for the bucket name and '{1}' as a placeholder for the region"},
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.AWS.AWS_ACCESS_KEY_ID, Value = "", Description = "The AWS access key id. Used for authenticating with AWS"},
                    new ApplicationConfigurationEntity { Id = AppConfigKeys.AWS.AWS_SECRET_ACCESS_KEY, Value = "", Description = "The AWS secret access key. Used for authenticating with AWSA"}
                };

            foreach (var appConfig in applicationConfigurations) {
                if (_applicationContext.ApplicationConfigurations.SingleOrDefault(ac => ac.Id == appConfig.Id) == null) {
                    appConfigs.Add(appConfig);
                }
            }

            _identityContext.SaveChanges();
            _applicationContext.SaveChanges();
        }

        #endregion Public Methods

        private void CreateSuperAdminUser(params string[] roleIds) {
            if (!_identityContext.Users.Any(u => u.UserName == _configuration["Admin:Username"])) {
                var user = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = _configuration["Admin:Email"],
                    NormalizedEmail = _configuration["Admin:Email"].ToUpper(),
                    UserName = _configuration["Admin:Username"],
                    NormalizedUserName = _configuration["Admin:Username"].ToUpper(),
                    Name = "zeus",
                    Surname = "zeus",
                    PhoneNumber = "999999999999",
                    IsActive = true,
                    EmailConfirmed = true,
                    MustChangePassword = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, _configuration["Admin:Password"]);
                user.PasswordHash = hashed;

                _identityContext.Users.Add(user);
                foreach (string roleId in roleIds) {
                    _identityContext.UserRoles.Add(new ApplicationUserRole()
                    {
                        RoleId = roleId,
                        UserId = user.Id
                    });
                }
            }
        }
    }
}