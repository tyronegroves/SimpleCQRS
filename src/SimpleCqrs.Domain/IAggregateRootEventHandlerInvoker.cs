using SimpleCqrs.Events;

namespace SimpleCqrs.Domain
{
    public interface IAggregateRootEventHandlerInvoker
    {
        void Invoke(AggregateRoot aggregateRoot, IEvent @event);
    }
}