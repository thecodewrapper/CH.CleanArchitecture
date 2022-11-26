using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CH.EventStore.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CH.EventStore.EntityFramework.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventStoreEFCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEventStore, EFEventStore>();
            services.AddScoped<IEventStoreSnapshotProvider, EFEventStoreSnapshotProvider>();
            services.AddScoped<IRetroactiveEventsService, EFRetroactiveEventsService>();

            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<EventStoreDbContext>(options =>
                    options.UseInMemoryDatabase("EventStoreDb"));
            }
            else
            {
                services.AddDbContext<EventStoreDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ApplicationConnection")));
            }
            return services;
        }
    }
}
