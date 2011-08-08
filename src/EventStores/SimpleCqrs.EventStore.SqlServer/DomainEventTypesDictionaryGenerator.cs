using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.SqlServer
{
    internal class DomainEventTypesDictionaryGenerator
    {
        public IDictionary<string, Type> GenerateDictionaryOfDomainTypes()
        {
            var list = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var domainEventTypes = assembly.GetTypes().Where(x => x.BaseType == typeof (DomainEvent));
                list.AddRange(domainEventTypes);
            }
            return list.ToDictionary(x => x.Name, x => x);
        }
    }
}