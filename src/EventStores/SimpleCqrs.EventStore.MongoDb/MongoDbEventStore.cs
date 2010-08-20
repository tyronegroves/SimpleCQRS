using System;
using System.Collections.Generic;
using SimpleCqrs.Events;

namespace SimpleCqrs.EventStore.MongoDb
{
    public class MongoDbEventStore : IEventStore
    {
        public IEnumerable<IDomainEvent> GetAggregateEvents(Guid aggregateRootId)
        {
            throw new NotImplementedException();
        }

        public void Insert(IEnumerable<IDomainEvent> domainEvents)
        {
            throw new NotImplementedException();
        }
    }
}