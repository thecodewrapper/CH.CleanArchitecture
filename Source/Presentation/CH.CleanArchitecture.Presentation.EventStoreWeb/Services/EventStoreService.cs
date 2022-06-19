using CH.CleanArchitecture.Infrastructure.Models;
using CH.CleanArchitecture.Infrastructure.Repositories;
using CH.CleanArchitecture.Presentation.EventStoreWeb.Models;

namespace CH.CleanArchitecture.Presentation.EventStoreWeb.Services
{
    public class EventStoreService
    {
        private readonly DataEntityRepository<EventEntity, Guid> _eventRepository;

        public EventStoreService(DataEntityRepository<EventEntity, Guid> eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public List<EventModel> GetEvents()
        {
            return _eventRepository.GetAll().Select(ToModel).ToList();
        }

        private EventModel ToModel(EventEntity eventEntity)
        {
            return new EventModel()
            {
                AggregateId = eventEntity.AggregateId,
                AggregateName = eventEntity.AggregateName,
                AssemblyTypeName = eventEntity.AssemblyTypeName,
                Data = eventEntity.Data,
                CreatedAt = eventEntity.CreatedAt,
                Id = eventEntity.Id,
                Name = eventEntity.Name,
                Version = eventEntity.Version,
                BranchPoints = eventEntity.BranchPoints.Select(ToModel).ToList()
            };
        }

        private BranchPointModel ToModel(BranchPointEntity branchPointEntity)
        {
            return new BranchPointModel()
            {
                Name = branchPointEntity.Name,
                //Type = branchPointEntity.Type,
                RetroactiveEvents = branchPointEntity.RetroactiveEvents.Select(ToModel).ToList()
            };
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
    }
}