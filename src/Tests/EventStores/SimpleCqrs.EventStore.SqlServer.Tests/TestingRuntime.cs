using System;
using SimpleCqrs.Unity;

namespace SimpleCqrs.EventStore.SqlServer.Tests
{
    public class TestingRuntime : SimpleCqrsRuntime<UnityServiceLocator>
    {
        protected override ITypeCatalog GetTypeCatalog(IServiceLocator serviceLocator)
        {
            var assemblyTypeCatalog = new AssemblyTypeCatalog(new []{typeof(TestingRuntime).Assembly});
            return new TestingAssemblyTypeCatalog(assemblyTypeCatalog);
        }
    }

    public class TestingAssemblyTypeCatalog : ITypeCatalog
    {
        private ITypeCatalog typeCatalog;
        public TestingAssemblyTypeCatalog(ITypeCatalog typeCatalog)
        {
            this.typeCatalog = typeCatalog;
        }

        public Type[] LoadedTypes
        {
            get { return typeCatalog.LoadedTypes; }
        }

        public Type[] GetDerivedTypes(Type type)
        {
            return typeCatalog.GetDerivedTypes(type);
        }

        public Type[] GetDerivedTypes<T>()
        {
            return typeCatalog.GetDerivedTypes<T>();
        }

        public Type[] GetGenericInterfaceImplementations(Type type)
        {
            return typeCatalog.GetGenericInterfaceImplementations(type);
        }

        public Type[] GetInterfaceImplementations(Type type)
        {
            return typeCatalog.GetInterfaceImplementations(type);
        }

        public Type[] GetInterfaceImplementations<T>()
        {
            return typeCatalog.GetInterfaceImplementations<T>();
        }
    }
}