using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleCqrs.Commanding;
using SimpleCqrs.Eventing;

namespace SimpleCqrs
{
    public abstract class Bootstrapper<TServiceLocator> where TServiceLocator : class, IServiceLocator
    {
        public IServiceLocator Run()
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
            return serviceLocator;
        }

        public void Shutdown(IServiceLocator serviceLocator)
        {
            serviceLocator.Dispose();
        }

        protected virtual ICommandBus GetCommandBus(IServiceLocator serviceLocator)
        {
            return serviceLocator.Resolve<DirectCommandBus>();
        }

        protected virtual IEventBus GetEventBus(IServiceLocator serviceLocator)
        {
            return serviceLocator.Resolve<DirectEventBus>();
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