using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventReplay
{
    public class DomainEventHandlerEventReplayer
    {
        private readonly IEventStore eventStore;
        private readonly IServiceLocator serviceLocator;

        public DomainEventHandlerEventReplayer(IEventStore eventStore,
                                               IServiceLocator serviceLocator)
        {
            this.eventStore = eventStore;
            this.serviceLocator = serviceLocator;
        }

        public void ReplayEventsForThisHandler<T>() where T : class
        {
            var events = GetTheEventsToReplay<T>();

            var handler = GetTheHandler<T>();

            foreach (var @event in events)
                HandleTheEvent<T>(handler, @event);
        }

        private static void HandleTheEvent<T>(object handler, DomainEvent @event)
        {
            typeof (T).InvokeMember("Handle", BindingFlags.Default | BindingFlags.InvokeMethod, null, handler, new object[] {@event}, null);
        }

        private IEnumerable<DomainEvent> GetTheEventsToReplay<T>()
        {
            var eventTypes = typeof (T).GetInterfaces()
                .Where(x => x.FullName.StartsWith("SimpleCqrs.Eventing.IHandleDomainEvents`1"))
                .Select(z => z.GetGenericArguments().First());

            return eventStore.GetEventsOfTheseTypes(eventTypes);
        }

        private object GetTheHandler<T>()
        {
            return serviceLocator.Resolve(typeof (T));
        }
    }
}