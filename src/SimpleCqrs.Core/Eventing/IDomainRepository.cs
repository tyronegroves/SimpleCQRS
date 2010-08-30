using System;
using SimpleCqrs.Domain;

namespace SimpleCqrs.Eventing
{
    public interface IDomainRepository
    {
        TAggregateRoot GetById<TAggregateRoot>(Guid aggregateRootId) where TAggregateRoot : AggregateRoot, new();
        void Save(AggregateRoot aggregateRoot);
    }
}