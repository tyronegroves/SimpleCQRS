using System;
using System.Collections.Generic;
using StructureMap;

namespace SimpleCqrs.StructureMap
{
    public class StructureMapServiceLocator : IServiceLocator
    {
        private static bool _isDisposing;

        public StructureMapServiceLocator() : this(ObjectFactory.Container) { }

        public StructureMapServiceLocator(IContainer container)
        {
            Container = container;
        }

        public IContainer Container { private set; get; }

        public T Resolve<T>() where T : class
        {
            try
            {
                return Container.GetInstance<T>();
            }
            catch (Exception ex)
            {
                throw new ServiceResolutionException(typeof(T), ex);
            }
        }

        public T Resolve<T>(string key) where T : class
        {
            try
            {
                return Container.GetInstance<T>(key);
            }
            catch (Exception ex)
            {
                throw new ServiceResolutionException(typeof(T), ex);
            }
        }

        public object Resolve(Type type)
        {
            try
            {
                return Container.GetInstance(type);
            }
            catch (Exception ex)
            {
                throw new ServiceResolutionException(type, ex);
            }
        }

        public IList<T> ResolveServices<T>() where T : class
        {
            return Container.GetAllInstances<T>();
        }

        public void Register<TInterface>(Type implType) where TInterface : class
        {
            var key = string.Format("{0}-{1}", typeof(TInterface).Name, implType.FullName);
            Container.Configure(x => x.For(implType).Use(implType).Named(key));

            // Work-around, also register this implementation to service mapping
            // without the generated key above.
            Container.Configure(x => x.For(implType).Use(implType));
        }

        public void Register<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            Container.Configure(x => x.For<TInterface>().Use<TImplementation>());
        }

        public void Register<TInterface, TImplementation>(string key) where TImplementation : class, TInterface
        {
            Container.Configure(x => x.For<TInterface>().Use<TImplementation>().Named(key));
        }

        public void Register(string key, Type type)
        {
            Container.Configure(x => x.For(type).Use(type).Named(key));
        }

        public void Register(Type serviceType, Type implType)
        {
            Container.Configure(x => x.For(serviceType).Use(implType));
        }

        public void Register<TInterface>(TInterface instance) where TInterface : class
        {
            Container.Configure(x => x.For<TInterface>().Use(instance));
        }

        public void Register<TInterface>(Func<TInterface> factoryMethod) where TInterface : class
        {
            Container.Configure(x => x.For<TInterface>().Use(factoryMethod));
        }

        public void Release(object instance)
        {
            //Not needed for StructureMap it doesn't keep references beyond the life cycle that was configured.
        }

        public void Reset()
        {
            throw new NotSupportedException("StructureMap does not support reset");
        }

        public TService Inject<TService>(TService instance) where TService : class
        {
            if (instance == null)
                return null;

            Container.BuildUp(instance);

            return instance;
        }

        public void TearDown<TService>(TService instance) where TService : class
        {
            //Not needed for StructureMap it doesn't keep references beyond the life cycle that was configured.
        }

        public void Dispose()
        {
            if (_isDisposing) return;
            if (Container == null) return;

            _isDisposing = true;
            Container.Dispose();
            Container = null;
        }
    }
}