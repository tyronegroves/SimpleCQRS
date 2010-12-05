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
            var handlerType = typeof (T);

            var eventType = handlerType.GetInterfaces().First().GetGenericArguments().First();

            var events = eventStore.GetEventsOfTheseTypes(new[] {eventType});

            var handler = serviceLocator.Resolve(handlerType);

            foreach (var @event in events)
                handlerType.InvokeMember("Handle", BindingFlags.Default | BindingFlags.InvokeMethod, null, handler, new object[] {@event}, null);
        }
    }
}