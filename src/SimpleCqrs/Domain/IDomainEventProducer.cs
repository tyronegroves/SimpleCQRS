using SimpleCqrs.Eventing;

namespace SimpleCqrs.Domain
{
    public interface IDomainEventProducer 
    {
        void Apply(EntityDomainEvent domainEvent);
    }
}