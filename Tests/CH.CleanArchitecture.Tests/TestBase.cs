using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Application.Extensions;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Infrastructure.DbContexts;
using CH.CleanArchitecture.Infrastructure.Extensions;
using CH.CleanArchitecture.Infrastructure.Models;
using CH.CleanArchitecture.Tests.Mocks;
using CH.EventStore.EntityFramework;
using CH.Messaging.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using IdentityDbContext = CH.CleanArchitecture.Infrastructure.DbContexts.IdentityDbContext;

namespace CH.CleanArchitecture.Tests
{
    public class TestBase
    {
        protected readonly ApplicationDbContext ApplicationContext;
        protected readonly EventStoreDbContext EventStoreContext;
        protected readonly IdentityDbContext IdentityContext;
        protected readonly IServiceBus ServiceBus;
        protected readonly IServiceProvider ServiceProvider;

        public TestBase() {
            var appDbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var identityDbContextOptions = new DbContextOptionsBuilder<IdentityDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var eventStoreDbContextOptions = new DbContextOptionsBuilder<EventStoreDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            // Create a new instance of your DbContext using the in-memory options
            ApplicationContext = new ApplicationDbContext(appDbContextOptions, new MockAuthenticatedUserService());
            IdentityContext = new IdentityDbContext(identityDbContextOptions);
            EventStoreContext = new EventStoreDbContext(eventStoreDbContextOptions);

            var configuration = BuildConfiguration();
            var serviceProvider = BuildServiceProvider(configuration, ApplicationContext, IdentityContext, EventStoreContext);
            ServiceProvider = serviceProvider;
            ServiceBus = serviceProvider.GetService<IServiceBus>();
            AddApplicationData(ApplicationContext);
            AddIdentityData(IdentityContext);
        }

        /// <summary>
        /// Build services for testing
        /// </summary>
        /// <returns></returns>
        private IServiceProvider BuildServiceProvider(IConfiguration configuration, ApplicationDbContext dbContext, IdentityDbContext identityDbContext, EventStoreDbContext eventStoreDbContext) {
            IServiceCollection services = new ServiceCollection();
            services.AddInfrastructureLayer(configuration);
            services.AddApplicationLayer();
            services.AddTransient<IAuthenticatedUserService, MockAuthenticatedUserService>();

            services.AddScoped<SignInManager<ApplicationUser>>(_ => InitializeSignInManager(identityDbContext));
            services.AddScoped<UserManager<ApplicationUser>>(_ => InitializeUserManager(identityDbContext));

            services.AddScoped<ApplicationDbContext>(_ => dbContext);
            services.AddScoped<IdentityDbContext>(_ => identityDbContext);
            services.AddScoped<EventStoreDbContext>(_ => eventStoreDbContext);

            services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            services.AddLogging();

            var localizationService = new Mock<ILocalizationService>();
            services.AddScoped(_ => localizationService.Object);

            return services.BuildServiceProvider();
        }

        private IConfiguration BuildConfiguration() {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            configuration["UseInMemoryDatabase"] = "true";
            return configuration;
        }

        /// <summary>
        /// Seeds dummy data for the application
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public void AddApplicationData(ApplicationDbContext context) {
            context.Database.EnsureCreated();
            DbInitializer.Initialize(context);
        }

        /// <summary>
        /// Seeds dummy identity data
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public void AddIdentityData(IdentityDbContext context) {
            context.Database.EnsureCreated();
            DbInitializer.Initialize(context);
        }

        /// <summary>
        /// Initializes a <see cref="FakeUserManager"/>.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static FakeUserManager InitializeUserManager(IdentityDbContext context) {
            var fakeUserManager = new Mock<FakeUserManager>(context);
            var availableRoles = Enum.GetValues(typeof(RoleEnum)).Cast<RoleEnum>();

            fakeUserManager.Setup(x => x.Users)
                .Returns(context.Users);

            fakeUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser user, string role) =>
                    user.Roles.Any(r => r.Role.Name == role));

            fakeUserManager.Setup(x => x.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()))
                .Callback<ApplicationUser, IEnumerable<string>>((au, roles) =>
                {
                    var userRoles = au.Roles.ToList();
                    userRoles.RemoveAll(ur => roles.Any(r => r == ur.Role.Name));
                })
                .ReturnsAsync(IdentityResult.Success);

            fakeUserManager.Setup(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()))
                .Callback<ApplicationUser, IEnumerable<string>>((au, roles) =>
                {
                    foreach (var role in roles)
                        au.Roles.Add(new ApplicationUserRole { Role = new ApplicationRole { Name = role } });
                })
                .ReturnsAsync(IdentityResult.Success);

            fakeUserManager.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
                .Callback<ApplicationUser>(u => context.Users.Remove(u))
                .ReturnsAsync(IdentityResult.Success);

            fakeUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((u, p) =>
                {
                    if (string.IsNullOrWhiteSpace(p))
                        throw new ArgumentNullException();
                    if (u == null || string.IsNullOrWhiteSpace(u.UserName))
                        throw new ArgumentNullException();
                    context.Users.Add(u);
                }).ReturnsAsync(IdentityResult.Success);

            fakeUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .Callback<ApplicationUser>(u =>
                {
                    context.Users.Remove(u);
                    context.Users.Add(u);
                }).ReturnsAsync(IdentityResult.Success);

            fakeUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((string username) => context.Users.FirstOrDefault(u => u.UserName == username));

            fakeUserManager.Setup(x => x.FindByNameAsync("throwsException")).Throws<Exception>();

            return fakeUserManager.Object;
        }

        /// <summary>
        /// Initializes a <see cref="FakeSignInManager"/>
        /// </summary>
        /// <returns></returns>
        private static FakeSignInManager InitializeSignInManager(IdentityDbContext context) {
            var signInManager = new Mock<FakeSignInManager>(context);
            signInManager.Setup(
                    x => x.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);
            return signInManager.Object;
        }
    }
}
