using System;
using AutoMapper.Extensions.ExpressionMapping;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Infrastructure.Data.DbContexts;
using CH.CleanArchitecture.Infrastructure.Data.Extensions;
using CH.CleanArchitecture.Infrastructure.Data.Identity.Factories;
using CH.CleanArchitecture.Infrastructure.Data.Models;
using CH.CleanArchitecture.Infrastructure.Resources;
using CH.CleanArchitecture.Infrastructure.Shared.Extensions;
using CH.CleanArchitecture.Presentation.Framework.Services;
using CH.CleanArchitecture.Presentation.Web.Mappings;
using CH.CleanArchitecture.Presentation.Web.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CH.CleanArchitecture.Presentation.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
            services.AddPersistenceContexts(configuration);
            services.AddInfrastructureLayer();
            services.AddSharedServices();
            services.SetupIdentity();

            services.AddScoped<LocalizedRolesResolver>();
            services.AddScoped<LoaderService>();
            services.AddAutoMapper(config =>
            {
                config.AddProfile<AppProfile>();
                config.AddExpressionMapping();
            });

            services.AddTransient<IAuthenticatedUserService, AuthenticatedUserService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddStorageOptions(configuration);
            services.AddValidatorsFromAssemblyContaining<Startup>();
        }

        private static void AddPersistenceContexts(this IServiceCollection services, IConfiguration configuration) {
            if (configuration.GetValue<bool>("UseInMemoryDatabase")) {
                services.AddDbContext<IdentityDbContext>(options =>
                    options.UseInMemoryDatabase("IdentityDb"));
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("ApplicationDb"));
                services.AddDbContext<EventStoreDbContext>(options =>
                    options.UseInMemoryDatabase("EventStoreDb"));
            }
            else {
                services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ApplicationConnection")));
                services.AddDbContext<EventStoreDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ApplicationConnection")));
            }
        }

        private static void SetupIdentity(this IServiceCollection services) {
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

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Shared/AccessDenied";
                options.Cookie.Name = "CH.CleanArchitecture.AUTH";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/auth/login";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });
        }

        private static void AddStorageOptions(this IServiceCollection services, IConfiguration configuration) {
            services.Configure<FileStorageOptions>(x => configuration.GetSection("Storage").Bind(x));
        }
    }
}