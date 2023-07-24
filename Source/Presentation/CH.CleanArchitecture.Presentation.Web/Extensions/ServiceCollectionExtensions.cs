using System;
using AutoMapper.Extensions.ExpressionMapping;
using Blazored.Modal;
using Blazored.Toast;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Application.Extensions;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Infrastructure.Extensions;
using CH.CleanArchitecture.Infrastructure.Resources;
using CH.CleanArchitecture.Presentation.Framework;
using CH.CleanArchitecture.Presentation.Framework.Interfaces;
using CH.CleanArchitecture.Presentation.Framework.Services;
using CH.CleanArchitecture.Presentation.Web.Helpers;
using CH.CleanArchitecture.Presentation.Web.Mappings;
using CH.CleanArchitecture.Presentation.Web.Services;
using FluentValidation;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CH.CleanArchitecture.Presentation.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add MVC/Web App related services here
        /// </summary>
        /// <param name="services"></param>
        public static void AddApplication(this IServiceCollection services) {
            services.AddApplicationLayer();
            services.AddScoped(typeof(RolesToMultiSelectResolver<>));
            services.AddScoped<INotificationService, NotificationService>();

            services.AddScoped<UserHelper>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            services.AddBlazoredToast();
            services.AddBlazoredModal();
            services.AddScoped<IModalService, ModalService>();
            services.AddScoped<IToastService, ToastService>();
            services.AddScoped<IAuthorizationStateProvider, AuthorizationStateProvider>();
            services.AddTransient<IAuthenticatedUserService, AuthenticatedUserService>();

            services.AddScoped<LocalizedRolesResolver>();
            services.AddScoped<LoaderService>();
            services.AddAutoMapper(config =>
            {
                config.AddProfile<AppProfile>();
                config.AddExpressionMapping();
            });

            services.AddApplicationCookie();

            //Configure Hangfire dashboard authorization
            services.AddApplicationAuthorization((options) => options.AddPolicy(WebFrameworkConstants.HANGFIRE_DASHBOARD_POLICY_NAME, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(RoleEnum.SuperAdmin.ToString());
            }));

            services.AddHangfireDashboardAuthorizationFilter();
        }

        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
            services.AddInfrastructureLayer(configuration);
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<ILocalizationKeyProvider, LocalizationKeyProvider>();
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

        private static void AddHangfireDashboardAuthorizationFilter(this IServiceCollection services) {
            // Register the HangfireDashboardAuthorizationFilter using a factory method
            services.AddTransient<IDashboardAuthorizationFilter>(serviceProvider =>
            {
                var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
                var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                return new HangfireDashboardAuthorizationFilter(authorizationService, httpContextAccessor);
            });
        }
    }
}