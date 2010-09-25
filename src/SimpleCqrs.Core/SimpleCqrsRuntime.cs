using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace SimpleCqrs
{
    public abstract class SimpleCqrsRuntime<TServiceLocator> where TServiceLocator : class, IServiceLocator
    {
        public void Start()
        {
            var serviceLocator = GetServiceLocator();
            serviceLocator.Register<IServiceLocator>(serviceLocator);
            var assembliesToScan = GetAssembliesToScan(serviceLocator);
            var typeCatalog = GetTypeCatalog(assembliesToScan);
            serviceLocator.Register(typeCatalog);
            serviceLocator.Register(GetCommandBus(serviceLocator));
            serviceLocator.Register(GetEventBus(serviceLocator));
            serviceLocator.Register(GetSnapshotStore(serviceLocator));
            serviceLocator.Register(GetEventStore(serviceLocator));
            serviceLocator.Register<IDomainRepository, DomainRepository>();
            ServiceLocator.SetCurrent(serviceLocator);
        }

        public void Shutdown()
        {
            var serviceLocator = ServiceLocator.Current;
            serviceLocator.Dispose();
        }

        protected virtual ICommandBus GetCommandBus(IServiceLocator serviceLocator)
        {
            return serviceLocator.Resolve<LocalCommandBus>();
        }

        protected virtual IEventBus GetEventBus(IServiceLocator serviceLocator)
        {
            return serviceLocator.Resolve<LocalEventBus>();
        }

        protected virtual ISnapshotStore GetSnapshotStore(IServiceLocator serviceLocator)
        {
            return new NullSnapshotStore();
        }

        protected abstract IEventStore GetEventStore(IServiceLocator serviceLocator);

        protected virtual ITypeCatalog GetTypeCatalog(IEnumerable<Assembly> assembliesToScan)
        {
            return new TypeCatalog(assembliesToScan);
        }

        protected virtual TServiceLocator GetServiceLocator()
        {
            return Activator.CreateInstance<TServiceLocator>();
        }

        protected virtual IEnumerable<Assembly> GetAssembliesToScan(IServiceLocator serviceLocator)
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}