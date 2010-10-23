using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using NServiceBus.ObjectBuilder;
using NServiceBus.Unicast;
using NServiceBus.Unicast.Config;
using SimpleCqrs.Eventing;
using SimpleCqrs.NServiceBus.Eventing.Config;

namespace SimpleCqrs.NServiceBus.Eventing
{
    public class ConfigEventBus : Configure
    {
        public void Configure(ConfigUnicastBus config, IServiceLocator serviceLocator)
        {
            Configurer = config.Configurer;
            Builder = config.Builder;

            var eventHandlerTypes = GetDomainEventHandlerTypes();
            eventHandlerTypes.ForEach(t => Configurer.ConfigureComponent(t, ComponentCallModelEnum.Singlecall));
            Configurer.RegisterSingleton<IEventBus>(new LocalEventBus(eventHandlerTypes, new DomainEventHandlerFactory()));

            config.LoadMessageHandlers();
            var domainEventBusConfig = GetConfigSection<DomainEventBusConfig>();
            var domainEventTypes = GetDomainEventTypes();
            var domainEventMessageType = typeof(DomainEventMessage<>);
            var domainEventMessageTypes = new List<Type>();
            var bus = Builder.Build<UnicastBus>();

            foreach(DomainEventEndpointMapping mapping in domainEventBusConfig.DomainEventEndpointMappings)
            {
                foreach(var domainEventType in domainEventTypes)
                {
                    if(DomainEventsIsTypeNameOrAssemblieNameForDomainEventType(domainEventType, mapping.DomainEvents))
                    {
                        var messageType = domainEventMessageType.MakeGenericType(domainEventType);
                        domainEventMessageTypes.Add(messageType);
                        bus.RegisterMessageType(messageType, mapping.Endpoint, false);
                    }
                }
            }

            bus.Started += (s, e) => domainEventMessageTypes.ForEach(bus.Subscribe);
        }

        private static bool DomainEventsIsTypeNameOrAssemblieNameForDomainEventType(Type domainEventType, string domainEvents)
        {
            return domainEventType.AssemblyQualifiedName.ToLower() == domainEvents.ToLower() || domainEventType.Assembly.GetName().Name.ToLower() == domainEvents.ToLower();
        }

        private static IEnumerable<Type> GetDomainEventTypes()
        {
            return TypesToScan.Where(type => typeof(DomainEvent).IsAssignableFrom(type)).ToList();
        }

        private static IEnumerable<Type> GetDomainEventHandlerTypes()
        {
            return (from derivedType in TypesToScan
                    from interfaceType in derivedType.GetInterfaces()
                    where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandleDomainEvents<>)
                    select derivedType).Distinct().ToArray();
        }
    }
}