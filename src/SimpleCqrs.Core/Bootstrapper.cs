using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleCqrs.Commands;
using SimpleCqrs.Events;

namespace SimpleCqrs
{
    public abstract class Bootstrapper<TServiceLocator> where TServiceLocator : IServiceLocator
    {
        private IServiceLocator applicationServiceLocator;

        public IServiceLocator Run()
        {
            applicationServiceLocator = GetServiceLocator();
            var assembliesToScan = GetAssembliesToScan(applicationServiceLocator);
            var typeCatalog = GetTypeCatalog(assembliesToScan);
            applicationServiceLocator.Register(applicationServiceLocator);
            applicationServiceLocator.Register(typeCatalog);
            applicationServiceLocator.Register(GetCommandBus(applicationServiceLocator));
            applicationServiceLocator.Register(GetEventBus(applicationServiceLocator));
            applicationServiceLocator.Register(GetSnapshotStore(applicationServiceLocator));
            applicationServiceLocator.Register(GetEventStore(applicationServiceLocator));
            return applicationServiceLocator;
        }

        public void Shutdown()
        {
            applicationServiceLocator.Dispose();
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
            var typeCatalog = serviceLocator.Resolve<ITypeCatalog>();
            var snapshotStoreTypes = typeCatalog.GetInterfaceImplementations<ISnapshotStore>();

            if (snapshotStoreTypes.Length > 1)
                throw new Exception("Two snapshot stores");

            return snapshotStoreTypes.Length == 0 ? new NullSnapshotStore() : (ISnapshotStore)serviceLocator.Resolve(snapshotStoreTypes[0]);
        }

        protected abstract IEventStore GetEventStore(IServiceLocator serviceLocator);

        protected virtual ITypeCatalog GetTypeCatalog(IEnumerable<Assembly> assembliesToScan)
        {
            return new DefaultTypeCatalog(assembliesToScan);
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