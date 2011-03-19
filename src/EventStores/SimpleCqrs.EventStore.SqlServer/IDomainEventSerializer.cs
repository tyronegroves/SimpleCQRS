using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.SqlServer
{
    public interface IDomainEventSerializer
    {
        string Serialize(DomainEvent domainEvent);
        DomainEvent Deserialize(Type targetType, string serializedDomainEvent);
    }
}
