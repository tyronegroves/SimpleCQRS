using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleCqrs.Commanding;
using SimpleCqrs.Eventing;
using SimpleCqrs.NServiceBus.Commanding;

namespace SimpleCqrs.NServiceBus
{
    public class NServiceBusSimpleCqrsRuntime<TServiceLocator> : SimpleCqrsRuntime<TServiceLocator> where TServiceLocator : class, IServiceLocator
    {
        public ISnapshotStore SnapshotStore { get; set; }
        public TServiceLocator ServiceLocator { get; set; }
        public NsbCommandBus CommandBus { get; set; }
        public Func<IServiceLocator, IEventBus> EventBusFactoryMethod { get; set; }
        public Func<IServiceLocator, IEventStore> EventStoreFactoryMethod { get; set; }

        protected override IEventStore GetEventStore(IServiceLocator serviceLocator)
        {
            return EventStoreFactoryMethod != null ? EventStoreFactoryMethod(serviceLocator) : base.GetEventStore(serviceLocator);
        }

        protected override IEventBus GetEventBus(IServiceLocator serviceLocator)
        {
            return EventBusFactoryMethod != null ? EventBusFactoryMethod(serviceLocator) : BuildLocalEventBus(serviceLocator);
        }

        protected override ISnapshotStore GetSnapshotStore(IServiceLocator serviceLocator)
        {
            return SnapshotStore ?? base.GetSnapshotStore(serviceLocator);
        }

        protected override ITypeCatalog GetTypeCatalog(IEnumerable<Assembly> assembliesToScan)
        {
            return new TypeCatalog();
        }

        protected override TServiceLocator GetServiceLocator()
        {
            return ServiceLocator ?? base.GetServiceLocator();
        }

        protected override ICommandBus GetCommandBus(IServiceLocator serviceLocator)
        {
            return CommandBus ?? base.GetCommandBus(serviceLocator);
        }

        private static IEventBus BuildLocalEventBus(IServiceLocator serviceLocator)
        {
            var typeCatalog = serviceLocator.Resolve<ITypeCatalog>();
            var eventHandlerTypes = typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleDomainEvents<>));
            var domainEventHandlerFactory = serviceLocator.Resolve<DomainEventHandlerFactory>();

            return new LocalEventBus(eventHandlerTypes, domainEventHandlerFactory);
        }

    }
}