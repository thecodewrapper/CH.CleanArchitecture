using System;
using CH.CleanArchitecture.Core.Domain;
using Newtonsoft.Json;

namespace CH.CleanArchitecture.Infrastructure
{
    internal static class DomainEventHelper
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new PrivateSetterContractResolver() };

        public static IDomainEvent<TAggregateId> ConstructDomainEvent<TAggregateId>(string data, string assemblyTypeName) {
            Type type = Type.GetType(assemblyTypeName);
            var domainEvent = (IDomainEvent<TAggregateId>)JsonConvert.DeserializeObject(data, type, _jsonSerializerSettings);
            return domainEvent;
        }
    }
}