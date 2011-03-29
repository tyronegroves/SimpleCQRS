using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using SimpleCqrs.Eventing;
using System.Text;

namespace SimpleCqrs.NServiceBus.Eventing
{
    public class NsbLocalEventBus : IEventBus
    {
        private readonly IDomainEventHandlerFactory eventHandlerBuilder;
        private IDictionary<Type, EventHandlerInvoker> eventHandlerInvokers;

        public NsbLocalEventBus(IEnumerable<Type> eventHandlerTypes, IDomainEventHandlerFactory eventHandlerBuilder)
        {
            this.eventHandlerBuilder = eventHandlerBuilder;
            BuildEventInvokers(eventHandlerTypes);
        }

        public void PublishEvent(DomainEvent domainEvent)
        {
            if(!eventHandlerInvokers.ContainsKey(domainEvent.GetType())) return;

            var eventHandlerInvoker = eventHandlerInvokers[domainEvent.GetType()];
            eventHandlerInvoker.Publish(domainEvent);
        }

        public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
        {
            foreach(var domainEvent in domainEvents)
                PublishEvent(domainEvent);
        }

        private void BuildEventInvokers(IEnumerable<Type> eventHandlerTypes)
        {
            eventHandlerInvokers = new Dictionary<Type, EventHandlerInvoker>();
            foreach(var eventHandlerType in eventHandlerTypes)
            {
                foreach(var domainEventType in GetDomainEventTypes(eventHandlerType))
                {
                    EventHandlerInvoker eventInvoker;
                    if(!eventHandlerInvokers.TryGetValue(domainEventType, out eventInvoker))
                        eventInvoker = new EventHandlerInvoker(eventHandlerBuilder, domainEventType);

                    eventInvoker.AddEventHandlerType(eventHandlerType);
                    eventHandlerInvokers[domainEventType] = eventInvoker;
                }
            }
        }

        private static IEnumerable<Type> GetDomainEventTypes(Type eventHandlerType)
        {
            return from interfaceType in eventHandlerType.GetInterfaces()
                   where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandleDomainEvents<>)
                   select interfaceType.GetGenericArguments()[0];
        }

        private class EventHandlerInvoker
        {
            private readonly IDomainEventHandlerFactory eventHandlerFactory;
            private readonly Type domainEventType;
            private readonly List<Type> eventHandlerTypes;
            private readonly ILog logger = LogManager.GetLogger(typeof(NsbLocalEventBus));

            public EventHandlerInvoker(IDomainEventHandlerFactory eventHandlerFactory, Type domainEventType)
            {
                this.eventHandlerFactory = eventHandlerFactory;
                this.domainEventType = domainEventType;
                eventHandlerTypes = new List<Type>();
            }

            public void AddEventHandlerType(Type eventHandlerType)
            {
                eventHandlerTypes.Add(eventHandlerType);
            }

            public void Publish(DomainEvent domainEvent)
            {
                var exceptionList = new List<Exception>();

                var handleMethod = typeof(IHandleDomainEvents<>).MakeGenericType(domainEventType).GetMethod("Handle");
                foreach(var eventHandlerType in eventHandlerTypes)
                {
                    try
                    {
                        var eventHandler = eventHandlerFactory.Create(eventHandlerType);
                        handleMethod.Invoke(eventHandler, new object[] {domainEvent});
                    }
                    catch(Exception exception)
                    {
                        logger.Error(string.Format("An exception occured while handling event of type '{0}'\nMessage: {1}", domainEvent.GetType(), exception.Message), exception);
                        exceptionList.Add(exception);
                    }
                }

                if(exceptionList.Count > 0)
                    throw new AggregateException(exceptionList);
            }
        }
    }

#if (NET35 || NET20)
    public class AggregateException : Exception
    {
        public IList<Exception> InnerExceptions { get; set; }

        public AggregateException(List<Exception> exceptionList)
        {
            InnerExceptions = exceptionList;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("An exception occurred!");
            if(InnerExceptions != null)
            {
                int count = 1;
                foreach (var exc in InnerExceptions)
                {
                    sb.AppendLine(count + "\t:\t" + exc);
                    count++;
                }
            }
            return sb.ToString();
        }
    }
#endif
}