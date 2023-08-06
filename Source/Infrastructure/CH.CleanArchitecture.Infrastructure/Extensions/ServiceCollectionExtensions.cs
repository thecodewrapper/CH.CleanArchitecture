using AutoMapper.Extensions.ExpressionMapping;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Infrastructure.DbContexts;
using CH.CleanArchitecture.Infrastructure.Factories;
using CH.CleanArchitecture.Infrastructure.Mappings;
using CH.CleanArchitecture.Infrastructure.Models;
using CH.CleanArchitecture.Infrastructure.Repositories;
using CH.CleanArchitecture.Infrastructure.Services;
using CH.Data.Abstractions;
using CH.EventStore.EntityFramework.Extensions;
using CH.Messaging.Abstractions;
using Hangfire;
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
            services.AddAutoMapper(config =>
            {
                config.AddExpressionMapping();
                config.AddProfile<AppProfile>();
                config.AddProfile<EventProfile>();
                config.AddProfile<UserProfile>();
            });

            services.AddSharedServices();
            services.AddStorageServices(configuration);
            services.AddCommunicationServices();
            services.AddCryptoServices();
            services.AddAuthServices();
            services.AddScheduledJobs(configuration);

            services.AddScoped<IServiceBus, ServiceBusMediator>();
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
        }

        private static void AddStorageServices(this IServiceCollection services, IConfiguration configuration) {
            services.AddStorageOptions(configuration);
            services.AddScoped<IFileStorageService, FileStorageService>();
        }

        private static void AddCommunicationServices(this IServiceCollection services) {
            services.AddScoped<IEmailService, EmailSMTPService>();
        }

        private static void AddCryptoServices(this IServiceCollection services) {
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<IUrlTokenService, UrlTokenService>();
            services.AddScoped<IPasswordGeneratorService, PasswordGeneratorIdentityService>();
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
        }

        private static void AddRepositories(this IServiceCollection services) {
            services.AddScoped(typeof(IEntityRepository<,>), typeof(DataEntityRepository<,>));
            services.AddScoped(typeof(IESRepository<,>), typeof(ESRepository<,>));
            services.AddScoped<IOrderRepository, OrderRepository>();
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

        private static void AddStorageOptions(this IServiceCollection services, IConfiguration configuration) {
            services.Configure<FileStorageOptions>(x => configuration.GetSection("Storage").Bind(x));
        }
    }
}