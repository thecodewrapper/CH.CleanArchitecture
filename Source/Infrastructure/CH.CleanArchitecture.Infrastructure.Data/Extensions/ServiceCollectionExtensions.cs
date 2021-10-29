using AutoMapper.Extensions.ExpressionMapping;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Infrastructure.Data.Mappings;
using CH.CleanArchitecture.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CH.CleanArchitecture.Infrastructure.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructureLayer(this IServiceCollection services) {
            services.AddRepositories();
            services.AddScoped<OrderAddressResolver>();
            services.AddAutoMapper(config =>
            {
                config.AddExpressionMapping();
                config.AddProfile<AppProfile>();
                config.AddProfile<EventProfile>();
                config.AddProfile<UserProfile>();
            });
            services.AddScoped<IEventStore, EFEventStore>();
            services.AddScoped<IEventStoreSnapshotProvider, EFEventStoreSnapshotProvider>();
            services.AddScoped<IRetroactiveEventsService, RetroactiveEventsService>();
        }

        private static void AddRepositories(this IServiceCollection services) {
            services.AddScoped(typeof(IEntityRepository<,>), typeof(DataEntityRepository<,>));
            services.AddScoped(typeof(IESRepository<,>), typeof(ESRepository<,>));
            services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}