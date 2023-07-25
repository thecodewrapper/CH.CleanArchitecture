using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Infrastructure.Auditing;
using CH.CleanArchitecture.Infrastructure.EntityTypeConfigurations;
using CH.CleanArchitecture.Infrastructure.Models;
using CH.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("CH.CleanArchitecture.Tests")]

namespace CH.CleanArchitecture.Infrastructure.DbContexts
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IAuthenticatedUserService _authenticatedUser;
        public const string CONFIG_SCHEMA = "Config";
        public const string DOMAIN_SCHEMA = "Domain";
        public DbSet<ApplicationConfigurationEntity> ApplicationConfigurations { get; set; }

        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderItemEntity> OrderItems { get; set; }
        public DbSet<AuditHistory> AuditHistory { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IAuthenticatedUserService authenticatedUser) : base(options) {
            _authenticatedUser = authenticatedUser;
        }

        /// <summary>
        /// Entity model definitions
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            builder.EnableAuditHistory();

            //Application Configurations
            builder.ApplyConfiguration(new ApplicationConfigurationEntityConfiguration());

            //Orders
            builder.ApplyConfiguration(new OrderEntityConfiguration());

            //Order items
            builder.ApplyConfiguration(new OrderItemEntityConfiguration());

            //Addresses
            builder.ApplyConfiguration(new AddressEntityConfiguration("Addresses", DOMAIN_SCHEMA));
        }

        /// <summary>
        /// Overrides the <see cref="SaveChanges(bool)"/>
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <returns></returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess) {
            DbContextUpdateOperations.UpdateDates(ChangeTracker.Entries<AuditableEntity>(), _authenticatedUser.Username);
            this.EnsureAuditHistory(_authenticatedUser.Username);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// Overrides the <see cref="DbContext.SaveChanges()"/>.
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges() {
            DbContextUpdateOperations.UpdateDates(ChangeTracker.Entries<AuditableEntity>(), _authenticatedUser.Username);
            this.EnsureAuditHistory(_authenticatedUser.Username);
            return base.SaveChanges(true);
        }

        /// <summary>
        /// Overrides the <see cref="DbContext.SaveChangesAsync(bool, CancellationToken)"/>
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) {
            DbContextUpdateOperations.UpdateDates(ChangeTracker.Entries<AuditableEntity>(), _authenticatedUser.Username);
            this.EnsureAuditHistory(_authenticatedUser.Username);
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Overrides the <see cref="SaveChangesAsync(CancellationToken)"/>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            DbContextUpdateOperations.UpdateDates(ChangeTracker.Entries<AuditableEntity>(), _authenticatedUser.Username);
            this.EnsureAuditHistory(_authenticatedUser.Username);
            return base.SaveChangesAsync(true, cancellationToken);
        }
    }
}