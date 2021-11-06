using System;
using AutoMapper.Extensions.ExpressionMapping;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Infrastructure.DbContexts;
using CH.CleanArchitecture.Infrastructure.Extensions;
using CH.CleanArchitecture.Infrastructure.Identity.Factories;
using CH.CleanArchitecture.Infrastructure.Models;
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
            services.AddInfrastructureLayer(configuration);
            services.AddSharedServices();
            services.AddApplicationCookie();

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

        private static void AddApplicationCookie(this IServiceCollection services) {
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