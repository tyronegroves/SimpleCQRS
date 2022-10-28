using EventSourcingCQRS.Eventing;

namespace EventSourcingCQRS.Domain
{
    public interface IEventModification
    {
        void Apply(DomainEvent e);
    }
}