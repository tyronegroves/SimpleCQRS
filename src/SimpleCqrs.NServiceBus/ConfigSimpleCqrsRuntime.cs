using NServiceBus;
using NServiceBus.ObjectBuilder;
using SimpleCqrs.Commanding;
using SimpleCqrs.Eventing;
using SimpleCqrs.NServiceBus.Commanding;

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
            runtime.CommandBus = new CommandBus();
            Configurer.RegisterSingleton<SimpleCqrsRuntime<TServiceLocator>>(runtime);
            Configurer.RegisterSingleton<ICommandBus>(runtime.CommandBus);
            Configurer.ConfigureComponent(typeof(CommandMessageHandler), ComponentCallModelEnum.Singleton);
        }

        public ConfigSimpleCqrs<TServiceLocator> EventStore(IEventStore eventStore)
        {
            runtime.EventStore = eventStore;
            Configurer.RegisterSingleton<IEventStore>(eventStore);
            return this;
        }

        public ConfigSimpleCqrs<TServiceLocator> EventBus()
        {
            runtime.EventBus = new EventBus();
            Configurer.RegisterSingleton<IEventBus>(runtime.EventBus);
            return this;
        }

        public ConfigSimpleCqrs<TServiceLocator> EventBus(IEventBus eventBus)
        {
            runtime.EventBus = eventBus;
            Configurer.RegisterSingleton<IEventBus>(eventBus);
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
            return this;
        }
    }
}