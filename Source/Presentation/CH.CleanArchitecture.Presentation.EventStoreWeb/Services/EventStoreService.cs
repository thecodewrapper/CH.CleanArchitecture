using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Infrastructure.DbContexts;
using CH.CleanArchitecture.Infrastructure.Models;
using CH.CleanArchitecture.Presentation.EventStoreWeb.Enumerations;
using CH.CleanArchitecture.Presentation.EventStoreWeb.Models;

namespace CH.CleanArchitecture.Presentation.EventStoreWeb.Services
{
    public class EventStoreService
    {
        private readonly EventStoreDbContext _eventStoreDbContext;

        public EventStoreService(EventStoreDbContext eventStoreDbContext)
        {
            _eventStoreDbContext = eventStoreDbContext;
        }

        public List<EventModel> GetEvents()
        {
            return _eventStoreDbContext.Events.Select(ToModel).ToList();
        }

        private EventModel ToModel(EventEntity eventEntity)
        {
            var eventModel = new EventModel()
            {
                AggregateId = eventEntity.AggregateId,
                AggregateName = eventEntity.AggregateName,
                AssemblyTypeName = eventEntity.AssemblyTypeName,
                Data = eventEntity.Data,
                CreatedAt = eventEntity.CreatedAt,
                Id = eventEntity.Id,
                Name = eventEntity.Name,
                Version = eventEntity.Version
            };
            if (eventEntity.BranchPoints?.Any() ?? false)
            {
                eventModel.BranchPoints = eventEntity.BranchPoints.Select(ToModel).ToList();
            }

            return eventModel;
        }

        private BranchPointModel ToModel(BranchPointEntity branchPointEntity)
        {
            var branchPointModel = new BranchPointModel()
            {
                Name = branchPointEntity.Name,
                Type = ToBranchPointType(branchPointEntity.Type)
            };

            if (branchPointEntity.RetroactiveEvents?.Any() ?? false)
            {
                branchPointModel.RetroactiveEvents = branchPointEntity.RetroactiveEvents.Select(ToModel).ToList();
            }

            return branchPointModel;
        }

        private RetroactiveEventModel ToModel(RetroactiveEventEntity retroactiveEventEntity)
        {
            return new RetroactiveEventModel()
            {
                AssemblyTypeName = retroactiveEventEntity.AssemblyTypeName,
                Data = retroactiveEventEntity.Data,
                IsEnabled = retroactiveEventEntity.IsEnabled,
                Sequence = retroactiveEventEntity.Sequence
            };
        }

        private static BranchPointType ToBranchPointType(BranchPointTypeEnum branchPointEnum)
        {
            switch (branchPointEnum)
            {
                case BranchPointTypeEnum.Incorrect: return BranchPointType.Incorrect;
                case BranchPointTypeEnum.OutOfOrder: return BranchPointType.OutOfOrder;
                case BranchPointTypeEnum.Rejected: return BranchPointType.Rejected;
                default: throw new Exception("Unable to convert branch point type");
            }
        }
    }
}