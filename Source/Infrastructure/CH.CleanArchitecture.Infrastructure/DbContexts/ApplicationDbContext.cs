using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Infrastructure.Auditing;
using CH.CleanArchitecture.Infrastructure.Models;
using CH.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("CH.CleanArchitecture.Tests")]

namespace CH.CleanArchitecture.Infrastructure.DbContexts
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IAuthenticatedUserService _authenticatedUser;
        private const string CONFIG_SCHEMA = "Config";
        private const string DOMAIN_SCHEMA = "Domain";
        public DbSet<ApplicationConfigurationEntity> ApplicationConfigurations { get; set; }

        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderItemEntity> OrderItems { get; set; }
        public DbSet<AuditHistory> AuditHistory { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IAuthenticatedUserService authenticatedUser) : base(options)
        {
            _authenticatedUser = authenticatedUser;
        }

        /// <summary>
        /// Entity model definitions
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.EnableAuditHistory();

            //Application Configurations
            builder.Entity<ApplicationConfigurationEntity>().ToTable("ApplicationConfigurations", CONFIG_SCHEMA);
            builder.Entity<ApplicationConfigurationEntity>(ac =>
            {
                ac.Property(e => e.Id).HasMaxLength(256);
                ac.Property(e => e.Value).IsRequired().HasMaxLength(512);
                ac.Property(e => e.Description).HasMaxLength(1024);
            });

            //Orders
            builder.Entity<OrderEntity>().ToTable("Orders", DOMAIN_SCHEMA);
            builder.Entity<OrderEntity>(o =>
            {
                o.Property(o => o.TotalAmount).IsRequired();
                o.HasMany(o => o.OrderItems).WithOne(oi => oi.Order).HasForeignKey(o => o.OrderId).OnDelete(DeleteBehavior.Cascade);
            });

            //Order items
            builder.Entity<OrderItemEntity>().ToTable("OrderItems", DOMAIN_SCHEMA);
            builder.Entity<OrderItemEntity>(oi =>
            {
                oi.Property(oi => oi.Quantity).IsRequired();
                oi.Property(oi => oi.ProductName).IsRequired();
                oi.Property(oi => oi.ProductPrice).IsRequired();
            });
        }

        /// <summary>
        /// Overrides the <see cref="SaveChanges(bool)"/>
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <returns></returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DbContextUpdateOperations.UpdateDates(ChangeTracker.Entries<AuditableEntity>(), _authenticatedUser.Username);
            this.EnsureAuditHistory(_authenticatedUser.Username);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// Overrides the <see cref="DbContext.SaveChanges()"/>.
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
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
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            DbContextUpdateOperations.UpdateDates(ChangeTracker.Entries<AuditableEntity>(), _authenticatedUser.Username);
            this.EnsureAuditHistory(_authenticatedUser.Username);
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Overrides the <see cref="SaveChangesAsync(CancellationToken)"/>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            DbContextUpdateOperations.UpdateDates(ChangeTracker.Entries<AuditableEntity>(), _authenticatedUser.Username);
            this.EnsureAuditHistory(_authenticatedUser.Username);
            return base.SaveChangesAsync(true, cancellationToken);
        }
    }
}