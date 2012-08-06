using System;
using System.Collections.Generic;
using NServiceBus;
using NServiceBus.Unicast;
using SimpleCqrs.Eventing;
using SimpleCqrs.NServiceBus.Eventing.Config;

namespace SimpleCqrs.NServiceBus.Eventing
{
    public class ConfigEventBus : Configure
    {
        public void Configure(ConfigSimpleCqrs config, ISimpleCqrsRuntime runtime)
        {
            Configurer = config.Configurer;
            Builder = config.Builder;

            var serviceLocator = runtime.ServiceLocator;
            var typeCatalog = serviceLocator.Resolve<ITypeCatalog>();

            var domainEventBusConfig = GetConfigSection<DomainEventBusConfig>();
            var domainEventTypes = typeCatalog.GetDerivedTypes(typeof(DomainEvent));
            var domainEventMessageTypes = new List<Type>();
            var bus = (UnicastBus)config
                .MsmqTransport()
                .UnicastBus()
                    .LoadMessageHandlers(new First<DomainEventMessageHandler>())
                    .CreateBus();

            RegisterAssemblyDomainEventSubscriptionMappings(domainEventBusConfig, domainEventTypes, domainEventMessageTypes, bus);
            RegisterDomainEventSubscriptionMappings(domainEventBusConfig, domainEventTypes, domainEventMessageTypes, bus);

            bus.Started += (s, e) => domainEventMessageTypes.ForEach(bus.Subscribe);
        }

        private static void RegisterDomainEventSubscriptionMappings(DomainEventBusConfig domainEventBusConfig, IEnumerable<Type> domainEventTypes, ICollection<Type> domainEventMessageTypes, UnicastBus bus)
        {
            var domainEventMessageType = typeof(DomainEventMessage<>);
            foreach (DomainEventEndpointMapping mapping in domainEventBusConfig.DomainEventEndpointMappings)
            {
                foreach (var domainEventType in domainEventTypes)
                {
                    if (DomainEventsIsDomainEvent(domainEventType, mapping.DomainEvents))
                    {
                        var messageType = domainEventMessageType.MakeGenericType(domainEventType);
                        domainEventMessageTypes.Add(messageType);
                        bus.RegisterMessageType(messageType, mapping.Endpoint, false);
                    }
                }
            }
        }

        private static void RegisterAssemblyDomainEventSubscriptionMappings(DomainEventBusConfig domainEventBusConfig, IEnumerable<Type> domainEventTypes, ICollection<Type> domainEventMessageTypes, UnicastBus bus)
        {
            var domainEventMessageType = typeof(DomainEventMessage<>);
            foreach(DomainEventEndpointMapping mapping in domainEventBusConfig.DomainEventEndpointMappings)
            {
                foreach(var domainEventType in domainEventTypes)
                {
                    if (DomainEventsIsAssembly(domainEventType, mapping.DomainEvents))
                    {
                        var messageType = domainEventMessageType.MakeGenericType(domainEventType);
                        domainEventMessageTypes.Add(messageType);
                        bus.RegisterMessageType(messageType, mapping.Endpoint, false);
                    }
                }
            }
        }

        private static bool DomainEventsIsDomainEvent(Type domainEventType, string domainEvents)
        {
            return domainEventType.FullName.ToLower() == domainEvents.ToLower()
                   || domainEventType.AssemblyQualifiedName.ToLower() == domainEvents.ToLower();
        }

        private static bool DomainEventsIsAssembly(Type domainEventType, string domainEvents)
        {
            return domainEventType.Assembly.GetName().Name.ToLower() == domainEvents.ToLower();
        }
    }
}