using SimpleCqrs.Eventing;

namespace SimpleCqrs.Domain
{
    public abstract class Entity
    {
        public AggregateRoot Parent { get; set; }

        public void Apply(DomainEvent domainEvent)
        {
        }
    }
}