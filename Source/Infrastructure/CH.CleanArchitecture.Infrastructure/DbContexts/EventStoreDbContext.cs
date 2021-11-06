using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("CH.CleanArchitecture.Tests")]
namespace CH.CleanArchitecture.Infrastructure.DbContexts
{
    public class EventStoreDbContext : DbContext, IUnitOfWork
    {
        private readonly IAuthenticatedUserService _authenticatedUserService;

        public DbSet<EventEntity> Events { get; set; }
        public DbSet<AggregateSnapshotEntity> Snapshots { get; set; }
        public DbSet<BranchPointEntity> BranchPoints { get; set; }

        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options, IAuthenticatedUserService authenticatedUserService) : base(options) {
            _authenticatedUserService = authenticatedUserService;
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("EventStore");

            //Events
            var eventEntity = builder.Entity<EventEntity>().ToTable("Events");
            eventEntity.HasKey(k => k.Id);
            eventEntity.HasIndex(a => new { a.AggregateId, a.Version, a.AggregateName }).IsUnique().IsClustered(false);

            //Snapshots
            var snapshotEntity = builder.Entity<AggregateSnapshotEntity>().ToTable("Snapshots");
            snapshotEntity.HasKey(k => k.Id);
            snapshotEntity.Property(k => k.Id).UseIdentityColumn();

            //Branch points
            var branchPointEntity = builder.Entity<BranchPointEntity>().ToTable("BranchPoints");
            branchPointEntity.HasKey(k => k.Id);
            branchPointEntity.HasOne(bp => bp.Event).WithMany(e => e.BranchPoints).HasForeignKey(bp => bp.EventId).IsRequired().OnDelete(DeleteBehavior.Restrict);
            branchPointEntity.HasMany(bp => bp.RetroactiveEvents).WithOne(ra => ra.BranchPoint).HasForeignKey(ra => ra.BranchPointId).OnDelete(DeleteBehavior.Restrict);
            branchPointEntity.HasIndex(bp => new { bp.Name, bp.EventId }).IsUnique();

            //Retroactive events
            var retroactiveEventEntity = builder.Entity<RetroactiveEventEntity>().ToTable("RetroactiveEvents");
            retroactiveEventEntity.HasKey(k => k.Id);
        }

        /// <summary>
        /// Overrides the <see cref="SaveChangesAsync(CancellationToken)"/>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            DbContextUpdateOperations.UpdateDates(ChangeTracker.Entries<AuditableEntity>(), _authenticatedUserService.Username);
            return base.SaveChangesAsync(true, cancellationToken);
        }
    }
}