using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleCqrs.Commands;
using SimpleCqrs.Events;

namespace SimpleCqrs
{
    public class Bootstrapper
    {
        private IServiceLocator serviceLocator;

        public IServiceLocator Run()
        {
            serviceLocator = GetServiceLocator();
            var assembliesToScan = GetAssembliesToScan();
            var typeCatalog = GetTypeCatalog(assembliesToScan);
            serviceLocator.Register(typeCatalog);
            serviceLocator.Register(GetCommandBus(typeCatalog));
            serviceLocator.Register(GetEventBus(typeCatalog));
            serviceLocator.Register(GetSnapshotStore());
            serviceLocator.Register(GetEventStore());
            return serviceLocator;
        }

        public void Shutdown()
        {
            serviceLocator.Dispose();
        }

        protected virtual ICommandBus GetCommandBus(ITypeCatalog typeCatalog)
        {
            return new DirectCommandBus(typeCatalog, serviceLocator);
        }

        protected virtual IEventBus GetEventBus(ITypeCatalog typeCatalog)
        {
            return new DirectEventBus(typeCatalog);
        }

        protected virtual ISnapshotStore GetSnapshotStore()
        {
            return new NullSnapshotStore();
        }

        protected virtual IEventStore GetEventStore()
        {
            return new NullEventStore();
        }

        protected virtual ITypeCatalog GetTypeCatalog(IEnumerable<Assembly> assembliesToScan)
        {
            return new DefaultTypeCatalog(assembliesToScan);
        }

        protected virtual IServiceLocator GetServiceLocator()
        {
            return new DefaultServiceLocator();
        }

        protected virtual IEnumerable<Assembly> GetAssembliesToScan()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}