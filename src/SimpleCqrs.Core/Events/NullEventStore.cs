using System;
using System.Collections.Generic;

namespace SimpleCqrs.Events
{
    public class NullEventStore : IEventStore
    {
        public IEnumerable<DomainEvent> GetEvents(Guid aggregateRootId, int startSequence)
        {
            throw new NotImplementedException();
        }

        public void Insert(IEnumerable<DomainEvent> domainEvents)
        {
            throw new NotImplementedException();
        }
    }
}