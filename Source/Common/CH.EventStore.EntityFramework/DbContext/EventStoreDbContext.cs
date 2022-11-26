using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CH.Data.Abstractions;
using CH.EventStore.EntityFramework.Entities;

namespace CH.EventStore.EntityFramework
{
    public class EventStoreDbContext : DbContext, IUnitOfWork
    {
        public DbSet<EventEntity> Events { get; set; }
        public DbSet<AggregateSnapshotEntity> Snapshots { get; set; }
        public DbSet<BranchPointEntity> BranchPoints { get; set; }

        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options) {
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
    }
}