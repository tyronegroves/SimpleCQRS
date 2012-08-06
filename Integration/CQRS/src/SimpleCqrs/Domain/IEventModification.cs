using SimpleCqrs.Eventing;

namespace SimpleCqrs.Domain
{
    public interface IEventModification
    {
        void Apply(DomainEvent e);
    }
}