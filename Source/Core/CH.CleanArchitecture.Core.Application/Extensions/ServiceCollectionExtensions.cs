using System;
using CH.CleanArchitecture.Core.Application.Authorization;
using CH.CleanArchitecture.Core.Application.Mappings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace CH.CleanArchitecture.Core.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services) {
            services.AddScoped<PhoneNumberToStringConverter>();
            services.AddScoped<StringToPhoneNumberConverter>();

            services.AddAutoMapper(config =>
            {
                config.ConstructServicesUsing(t => services.BuildServiceProvider().GetRequiredService(t));
                config.AddProfile<UserProfile>();
                config.AddProfile<OrderProfile>();
            });
        }

        public static IServiceCollection AddApplicationAuthorization(this IServiceCollection services, Action<AuthorizationOptions> configure = default) {
            if (configure == null) {
                services.AddAuthorizationCore();
            }
            else {
                services.AddAuthorizationCore(configure);
            }
            services.AddAuthorizationPolicies();

            return services;
        }

        private static void AddAuthorizationPolicies(this IServiceCollection services) {
            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();

            //Known issue in .NET7 prevents IAuthorizationHandler from being registered as scoped
            services.AddTransient<IAuthorizationHandler, UserOperationAuthorizationHandler>();
        }
    }
}