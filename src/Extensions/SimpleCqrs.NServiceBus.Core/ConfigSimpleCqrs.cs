using NServiceBus;
using SimpleCqrs.Commanding;
using SimpleCqrs.Eventing;
using SimpleCqrs.NServiceBus.Commanding;
using SimpleCqrs.NServiceBus.Eventing;

namespace SimpleCqrs.NServiceBus
{
    public class ConfigSimpleCqrs : Configure
    {
        private readonly ISimpleCqrsRuntime runtime;

        public ConfigSimpleCqrs(ISimpleCqrsRuntime runtime)
        {
            this.runtime = runtime;
        }

        public IServiceLocator ServiceLocator { get; private set; }

        public void Configure(Configure config)
        {
            Configurer = config.Configurer;
            Builder = config.Builder;

            runtime.Start();

            ServiceLocator = runtime.ServiceLocator;
            ServiceLocator.Register(() => Builder.Build<IBus>());
            Configurer.RegisterSingleton<ISimpleCqrsRuntime>(runtime);
        }

        public ConfigSimpleCqrs UseLocalCommandBus()
        {
            var commandBus = ServiceLocator.Resolve<LocalCommandBus>();
            ServiceLocator.Register<ICommandBus>(commandBus);
            Configurer.RegisterSingleton<ICommandBus>(commandBus);

            return this;
        }

        public ConfigSimpleCqrs SubscribeForDomainEvents()
        {
            var eventBus = ServiceLocator.Resolve<IEventBus>();
            Configurer.RegisterSingleton<IEventBus>(eventBus);

            var configEventBus = new ConfigEventBus();
            configEventBus.Configure(this, runtime);

            return this;
        }

        public ConfigSimpleCqrs UseNsbCommandBus()
        {
            var config = new ConfigCommandBus(runtime);
            var commandBus = config.Configure(this);

            ServiceLocator.Register<ICommandBus>(commandBus);
            Configurer.RegisterSingleton<ICommandBus>(commandBus);

            return this;
        }

        public ConfigSimpleCqrs UseNsbEventBus()
        {
            var eventBus = new NsbEventBus();

            ServiceLocator.Register<IEventBus>(eventBus);
            Configurer.RegisterSingleton<IEventBus>(eventBus);

            return this;
        }
    }
}