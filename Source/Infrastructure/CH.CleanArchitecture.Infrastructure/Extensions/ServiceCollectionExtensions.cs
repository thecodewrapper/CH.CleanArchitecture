using AutoMapper.Extensions.ExpressionMapping;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Application.Commands;
using CH.CleanArchitecture.Infrastructure.DbContexts;
using CH.CleanArchitecture.Infrastructure.Factories;
using CH.CleanArchitecture.Infrastructure.Handlers.Queries;
using CH.CleanArchitecture.Infrastructure.Mappings;
using CH.CleanArchitecture.Infrastructure.Models;
using CH.CleanArchitecture.Infrastructure.Options;
using CH.CleanArchitecture.Infrastructure.Repositories;
using CH.CleanArchitecture.Infrastructure.Resources;
using CH.CleanArchitecture.Infrastructure.Services;
using CH.Data.Abstractions;
using CH.EventStore.EntityFramework.Extensions;
using CH.Messaging.Abstractions;
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CH.CleanArchitecture.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration) {
            services.AddDatabasePersistence(configuration);
            services.AddRepositories();
            services.AddIdentity();
            services.AddEventStoreEFCore((o) =>
            {
                o.UseInMemoryDatabase = configuration.GetValue<bool>("UseInMemoryDatabase");
                o.ConnectionStringSQL = configuration.GetConnectionString("ApplicationConnection");
            });
            services.AddMapping();

            services.AddSharedServices();
            services.AddLocalizationServices();
            services.AddStorageServices(configuration);
            services.AddCommunicationServices(configuration);
            services.AddCryptoServices();
            services.AddAuthServices();
            services.AddScheduledJobs(configuration);
            services.AddServiceBusMediator();
        }

        private static void AddDatabasePersistence(this IServiceCollection services, IConfiguration configuration) {
            if (configuration.GetValue<bool>("UseInMemoryDatabase")) {
                services.AddDbContext<IdentityDbContext>(options => options.UseInMemoryDatabase("IdentityDb"));
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("ApplicationDb"));
            }
            else {
                services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ApplicationConnection")));
            }
            services.AddScoped<IDbInitializerService, DbInitializerService>();
            services.AddScoped<IAuthenticatedUserService, DefaultAuthenticatedUserService>();
        }

        private static void AddRepositories(this IServiceCollection services) {
            services.AddScoped(typeof(IEntityRepository<,>), typeof(DataEntityRepository<,>));
            services.AddScoped(typeof(IESRepository<,>), typeof(ESRepository<,>));
            services.AddScoped<IOrderRepository, OrderRepository>();
        }

        private static void AddServiceBusMediator(this IServiceCollection services) {
            services.AddScoped<IServiceBus, ServiceBusMediator>();
            services.AddMediator(x =>
            {
                #region Commands

                #region User

                x.AddConsumer<CreateUserCommandHandler>();
                x.AddConsumer<ActivateUserCommandHandler>();
                x.AddConsumer<DeactivateUserCommandHandler>();
                x.AddConsumer<AddRolesCommandHandler>();
                x.AddConsumer<RemoveRolesCommandHandler>();
                x.AddConsumer<ChangeUserPasswordCommandHandler>();
                x.AddConsumer<UpdateUserRolesCommandHandler>();
                x.AddConsumer<UpdateUserDetailsCommandHandler>();
                x.AddConsumer<CreateNewOrderCommandHandler>();

                #endregion User

                #endregion Commands

                #region Queries

                x.AddConsumer<GetAllUsersQueryHandler>();
                x.AddConsumer<GetUserQueryHandler>();
                x.AddConsumer<GetAllOrdersQueryHandler>();
                x.AddConsumer<GetOrderByIdQueryHandler>();

                #endregion
            });
        }

        private static void AddMapping(this IServiceCollection services) {
            services.AddAutoMapper(config =>
            {
                config.AddExpressionMapping();
                config.AddProfile<AppProfile>();
                config.AddProfile<EventProfile>();
                config.AddProfile<UserProfile>();
                config.AddProfile<OrderProfile>();
            });
        }

        private static void AddScheduledJobs(this IServiceCollection services, IConfiguration configuration) {
            services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("ApplicationConnection")));
            services.AddHangfireServer();
            services.AddScoped<IScheduledJobService, ScheduledJobService>();
        }

        private static void AddAuthServices(this IServiceCollection services) {
            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
            services.AddScoped<IApplicationUserService, ApplicationUserService>();
        }

        private static void AddSharedServices(this IServiceCollection services) {

            services.AddScoped<IApplicationConfigurationService, ApplicationConfigurationService>();
            services.AddScoped<IAuditHistoryService, AuditHistoryService>();
            services.AddScoped<INotificationService, NotificationService>();
        }

        private static void AddLocalizationServices(this IServiceCollection services) {
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ILocalizationKeyProvider, LocalizationKeyProvider>();
        }

        /// <summary>
        /// Adds storage services. Conditionally add the required storage services here. See <see cref="IResourceStore"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void AddStorageServices(this IServiceCollection services, IConfiguration configuration) {
            var options = services.AddStorageOptions(configuration);

            //TODO[CH]: refactor below
            if (options.StorageProvider == "azure") {
                services.AddScoped<IResourceStore, AzureStorageResourceStore>();
            }
            else if (options.StorageProvider == "aws") {
                services.AddScoped<IResourceStore, AWSS3ResourceStore>();
            }
        }

        private static void AddCommunicationServices(this IServiceCollection services, IConfiguration configuration) {
            var emailSenderOptions = GetEmailSenderOptions(configuration);
            if (emailSenderOptions.UseSendGrid) {
                services.AddScoped<IEmailService, EmailSendGridService>();
            }
            else {
                services.AddScoped<IEmailService, EmailSMTPService>();
            }

            services.AddScoped<ISMSService, SMSService>();
        }

        private static void AddCryptoServices(this IServiceCollection services) {
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<IUrlTokenService, UrlTokenService>();
            services.AddScoped<IPasswordGeneratorService, PasswordGeneratorIdentityService>();
        }

        private static void AddIdentity(this IServiceCollection services) {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddRoleManager<RoleManager<ApplicationRole>>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders()
                .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

            var passwordOptions = new PasswordOptions()
            {
                RequireDigit = false, //in accordance with ASVS 4.0
                RequiredLength = 10, //in accordance with ASVS 4.0
                RequireUppercase = false, //in accordance with ASVS 4.0
                RequireLowercase = false //in accordance with ASVS 4.0
            };

            services.AddScoped(a => passwordOptions);

            services.Configure<IdentityOptions>(options =>
            {
                options.Password = passwordOptions;

            });
        }

        private static StorageOptions AddStorageOptions(this IServiceCollection services, IConfiguration configuration) {
            StorageOptions storageOptions = new StorageOptions();
            configuration.GetSection("Storage").Bind(storageOptions);

            return storageOptions;
        }

        private static EmailSenderOptions GetEmailSenderOptions(IConfiguration configuration) {
            EmailSenderOptions emailSenderOptions = new EmailSenderOptions();
            configuration.GetSection("EmailSender").Bind(emailSenderOptions);

            return emailSenderOptions;
        }
    }
}