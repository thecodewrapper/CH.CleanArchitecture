using System.Reflection;
using Blazored.Modal;
using Blazored.Toast;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Application.Extensions;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Infrastructure.Resources;
using CH.CleanArchitecture.Infrastructure.Shared.Culture;
using CH.CleanArchitecture.Presentation.Framework;
using CH.CleanArchitecture.Presentation.Framework.Interfaces;
using CH.CleanArchitecture.Presentation.Framework.Services;
using CH.CleanArchitecture.Presentation.Web.Extensions;
using CH.CleanArchitecture.Presentation.Web.Helpers;
using CH.CleanArchitecture.Presentation.Web.Mappings;
using CH.CleanArchitecture.Presentation.Web.Services;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CH.CleanArchitecture.Presentation.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) {
            Configuration = configuration;
            HostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddHttpContextAccessor();

            //ROUTING
            services.AddRouting(options => options.LowercaseUrls = true);

            //LOCALIZATION
            services.AddLocalization(options => options.ResourcesPath = "");

            //CACHING
            services.AddResponseCaching();

            //BLAZOR
            services.AddServerSideBlazor().AddCircuitOptions(options =>
            {
                //if (HostEnvironment.IsDevelopment())
                options.DetailedErrors = true;
            });

            services.AddBlazoredToast();
            services.AddBlazoredModal();
            services.AddScoped<IModalService, ModalService>();
            services.AddScoped<IToastService, ToastService>();
            services.AddScoped<IAuthorizationStateProvider, AuthorizationStateProvider>();

            //MVC
            var mvcBuilder = services.AddMvc(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
            mvcBuilder.AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix);
            mvcBuilder.AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    var assemblyName = new AssemblyName(typeof(SharedResources).GetTypeInfo().Assembly.FullName);
                    return factory.Create("SharedResources", assemblyName.Name);
                };
            });
#if (DEBUG)
            mvcBuilder.AddRazorRuntimeCompilation();
#endif

            services.AddInfrastructure(Configuration);

            services.AddScoped(typeof(RolesToMultiSelectResolver<>));
            services.AddScoped<ILocalizationKeyProvider, LocalizationKeyProvider>();
            services.AddScoped<UserHelper>();
            services.AddApplicationLayer();

            //Configure Hangfire dashboard authorization
            services.AddApplicationAuthorization((options) => options.AddPolicy("HangfireDashboardPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(RoleEnum.SuperAdmin.ToString());
            }));

            services.AddHangfireDashboardAuthorizationFilter();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            var basePath = configuration.GetValue<string>("BasePath");
            if (!string.IsNullOrWhiteSpace(basePath))
                app.UsePathBase(basePath);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseMustChangePassword();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { app.ApplicationServices.GetService<IDashboardAuthorizationFilter>() }
            });

            var supportedCultures = new[] { "el", "en" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            localizationOptions.RequestCultureProviders.Insert(0, new UserProfileRequestCultureProvider());
            app.UseRequestLocalization(localizationOptions);

            app.UseResponseCaching();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapBlazorHub();
            });
        }
    }
}