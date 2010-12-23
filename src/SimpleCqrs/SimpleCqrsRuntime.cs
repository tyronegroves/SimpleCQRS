using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace SimpleCqrs
{
    public class SimpleCqrsRuntime<TServiceLocator> : ISimpleCqrsRuntime
        where TServiceLocator : class, IServiceLocator
    {
        public TServiceLocator ServiceLocator
        {
            get { return (TServiceLocator)SimpleCqrs.ServiceLocator.Current; }
        }

        IServiceLocator ISimpleCqrsRuntime.ServiceLocator
        {
            get { return ServiceLocator; }
        }

        public void Start()
        {
            var serviceLocator = GetServiceLocator();

            serviceLocator.Register<IServiceLocator>(() => serviceLocator);
            serviceLocator.Register(GetTypeCatalog(serviceLocator));
            serviceLocator.Register(GetCommandBus(serviceLocator));
            serviceLocator.Register(GetEventBus(serviceLocator));
            serviceLocator.Register(GetSnapshotStore(serviceLocator));
            serviceLocator.Register(GetEventStore(serviceLocator));
            serviceLocator.Register<IDomainRepository, DomainRepository>();
            RegisterComponents(serviceLocator);

            SimpleCqrs.ServiceLocator.SetCurrent(serviceLocator);
        }

        public void Shutdown()
        {
            ServiceLocator.Dispose();
        }

        protected virtual ICommandBus GetCommandBus(IServiceLocator serviceLocator)
        {
            return serviceLocator.Resolve<LocalCommandBus>();
        }

        protected virtual IEventBus GetEventBus(IServiceLocator serviceLocator)
        {
            var typeCatalog = serviceLocator.Resolve<ITypeCatalog>();
            var domainEventHandlerFactory = serviceLocator.Resolve<DomainEventHandlerFactory>();
            var domainEventTypes = typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleDomainEvents<>));

            return new LocalEventBus(domainEventTypes, domainEventHandlerFactory);
        }

        protected virtual IEnumerable<Assembly> GetAssembliesToScan(IServiceLocator serviceLocator)
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        protected virtual ITypeCatalog GetTypeCatalog(IServiceLocator serviceLocator)
        {
            return new AssemblyTypeCatalog(GetAssembliesToScan(serviceLocator));
        }

        protected virtual ISnapshotStore GetSnapshotStore(IServiceLocator serviceLocator)
        {
            return new NullSnapshotStore();
        }

        protected virtual IEventStore GetEventStore(IServiceLocator serviceLocator)
        {
            return new MemoryEventStore();
        }

        protected virtual TServiceLocator GetServiceLocator()
        {
            return Activator.CreateInstance<TServiceLocator>();
        }

        protected virtual IEnumerable<IRegisterComponents> GetComponentRegistrars(Type componentRegistarType, IServiceLocator serviceLocator)
        {
            var typeCatalog = serviceLocator.Resolve<ITypeCatalog>();
            var componentRegistrarTypes = componentRegistarType.IsInterface ? 
                typeCatalog.GetInterfaceImplementations(componentRegistarType) : 
                typeCatalog.GetDerivedTypes(componentRegistarType);

            return componentRegistrarTypes
                .Select(serviceLocator.Resolve)
                .Cast<IRegisterComponents>();
        }

        protected virtual void RegisterComponents(IServiceLocator serviceLocator)
        {
            var componentRegistrars = GetComponentRegistrars(typeof(IRegisterComponents), serviceLocator).ToList();
            componentRegistrars.ForEach(componentRegistrar => componentRegistrar.Register(serviceLocator));
        }

        public void Dispose()
        {
            Shutdown();
        }
    }
}