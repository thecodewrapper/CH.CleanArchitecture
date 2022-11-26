using System;
using Newtonsoft.Json;
using CH.Domain.Abstractions;

namespace CH.EventStore.Abstractions
{
    public static class DomainEventHelper
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new PrivateSetterContractResolver() };

        public static IDomainEvent<TAggregateId> ConstructDomainEvent<TAggregateId>(string data, string assemblyTypeName) {
            Type type = Type.GetType(assemblyTypeName);
            var domainEvent = (IDomainEvent<TAggregateId>)JsonConvert.DeserializeObject(data, type, _jsonSerializerSettings);
            return domainEvent;
        }
    }
}