using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Presentation.EventStoreWeb.Services;
using CH.CleanArchitecture.Infrastructure.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using CH.CleanArchitecture.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using CH.CleanArchitecture.Infrastructure.Extensions;
using CH.EventStore.EntityFramework;

namespace Company.WebApplication1
{
    public class Program
    {
        private static IConfigurationRoot _configurationRoot;
        
        public static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            var builder = WebApplication.CreateBuilder(args);
            _configurationRoot = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile($"appsettings.{environment}.json", true)
                                .AddEnvironmentVariables()
                                .Build();
            
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddMudServices();
            builder.Services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
            builder.Services.AddScoped<EventStoreService>();
            builder.Services.AddDbContext<EventStoreDbContext>(options => options.UseSqlServer(_configurationRoot.GetConnectionString("ApplicationConnection")));

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}