using System.Collections.Generic;
using System.Reflection;
using SimpleCqrs.Commanding;
using SimpleCqrs.Eventing;
using SimpleCqrs.NServiceBus.Commanding;

namespace SimpleCqrs.NServiceBus
{
    public class NServiceBusSimpleCqrsRuntime<TServiceLocator> : SimpleCqrsRuntime<TServiceLocator> where TServiceLocator : class, IServiceLocator
    {
        protected override IEventStore GetEventStore(IServiceLocator serviceLocator)
        {
            return EventStore;
        }

        protected override IEventBus GetEventBus(IServiceLocator serviceLocator)
        {
            return EventBus ?? serviceLocator.Resolve<LocalEventBus>();
        }

        protected override ISnapshotStore GetSnapshotStore(IServiceLocator serviceLocator)
        {
            return SnapshotStore;
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

        public ISnapshotStore SnapshotStore { get; set; }
        public IEventBus EventBus { get; set; }
        public IEventStore EventStore { get; set; }
        public TServiceLocator ServiceLocator { get; set; }
        public CommandBus CommandBus { get; set; }
    }
}