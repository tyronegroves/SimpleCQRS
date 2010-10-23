using System;
using NServiceBus;
using SimpleCqrs.Commanding;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.NServiceBus
{
    public class ConfigSimpleCqrs<TServiceLocator> : Configure where TServiceLocator : class, IServiceLocator
    {
        private readonly NServiceBusSimpleCqrsRuntime<TServiceLocator> runtime;

        public ConfigSimpleCqrs(NServiceBusSimpleCqrsRuntime<TServiceLocator> runtime)
        {
            this.runtime = runtime;
        }

        public void Configure(Configure config)
        {
            Configurer = config.Configurer;
            Builder = config.Builder;

            Configurer.RegisterSingleton<SimpleCqrsRuntime<TServiceLocator>>(runtime);
        }

        public ConfigSimpleCqrs<TServiceLocator> EventStore(Func<IServiceLocator, IEventStore> eventStoreFactoryMethod)
        {
            runtime.EventStoreFactoryMethod = eventStoreFactoryMethod;
            return this;
        }

        public ConfigSimpleCqrs<TServiceLocator> UseNsbPublishing()
        {
            var eventBus = new NsbEventBus();
            runtime.EventBusFactoryMethod = serviceLocator => eventBus;
            Configurer.RegisterSingleton<IEventBus>(eventBus);
            return this;
        }

        public ConfigSimpleCqrs<TServiceLocator> EventBus(Func<IServiceLocator, IEventBus> eventBusFactoryMethod)
        {
            runtime.EventBusFactoryMethod = eventBusFactoryMethod;
            return this;
        }

        public ConfigSimpleCqrs<TServiceLocator> SnapshotStore(ISnapshotStore snapshotStore)
        {
            runtime.SnapshotStore = snapshotStore;
            Configurer.RegisterSingleton<ISnapshotStore>(snapshotStore);
            return this;
        }

        public ConfigSimpleCqrs<TServiceLocator> StartSimpleCqrs()
        {
            runtime.Start();

            var commandBus = ServiceLocator.Current.Resolve<ICommandBus>();
            var eventBus = ServiceLocator.Current.Resolve<IEventBus>();

            Configurer.RegisterSingleton<ICommandBus>(commandBus);
            Configurer.RegisterSingleton<IEventStore>(eventBus);

            return this;
        }
    }
}