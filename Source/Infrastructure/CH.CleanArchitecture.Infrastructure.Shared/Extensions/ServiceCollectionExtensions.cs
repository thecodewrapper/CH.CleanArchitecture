using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Infrastructure.Data.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CH.CleanArchitecture.Infrastructure.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSharedServices(this IServiceCollection services) {
            services.AddScoped<IPasswordGeneratorService, PasswordGeneratorIdentityService>();
            services.AddScoped<IServiceBus, ServiceBusMediator>();
            services.AddScoped<IApplicationConfigurationService, ApplicationConfigurationService>();
            services.AddScoped<IApplicationUserService, ApplicationUserService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
            services.AddScoped<DbInitializerService>();
        }
    }
}