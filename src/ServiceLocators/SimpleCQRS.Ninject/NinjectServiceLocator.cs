using Ninject;
using SimpleCqrs;
using System;
using System.Collections.Generic;

namespace SimpleCQRS.Ninject
{
    /// <summary>
    /// Ninject IoC service locator.
    /// </summary>
    public class NinjectServiceLocator : IServiceLocator
    {
        /// <summary>
        /// Gets or sets the kernel.
        /// </summary>
        /// <value>
        /// The kernel.
        /// </value>
        public IKernel Kernel { private set; get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectServiceLocator"/> class.
        /// </summary>
        public NinjectServiceLocator() : this(new StandardKernel()) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectServiceLocator"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public NinjectServiceLocator(IKernel kernel)
		{
            Kernel = kernel;
		}

        /// <summary>
        /// Injects the specified instance.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public TService Inject<TService>(TService instance) where TService : class
        {
            if (instance == null)
            {
                return null;
            }
            Kernel.Inject(instance);
            return instance;
        }

        /// <summary>
        /// Registers the specified factory method.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="factoryMethod">The factory method.</param>
        public void Register<TInterface>(Func<TInterface> factoryMethod) where TInterface : class
        {
            Kernel.Bind<TInterface>().ToMethod(c => factoryMethod.Invoke());
        }

        /// <summary>
        /// Registers the specified instance.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="instance">The instance.</param>
        public void Register<TInterface>(TInterface instance) where TInterface : class
        {
            Kernel.Bind<TInterface>().To(instance.GetType());
        }

        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="implType">Type of the impl.</param>
        public void Register(Type serviceType, Type implType)
        {
            Kernel.Bind(serviceType).To(implType);
        }

        /// <summary>
        /// Registers the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="type">The type.</param>
        public void Register(string key, Type type)
        {
            Kernel.Bind(type).To(type).Named(key);
        }

        /// <summary>
        /// Registers the specified key.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="key">The key.</param>
        public void Register<TInterface, TImplementation>(string key) where TImplementation : class, TInterface
        {
            Kernel.Bind<TInterface>().To<TImplementation>().Named(key);
        }

        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void Register<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            Kernel.Bind<TInterface>().To<TImplementation>();
        }

        /// <summary>
        /// Registers the specified impl type.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="implType">Type of the impl.</param>
        public void Register<TInterface>(Type implType) where TInterface : class
        {
            Kernel.Bind<TInterface>().To(implType);
        }

        /// <summary>
        /// Releases the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void Release(object instance)
        {
            Kernel.Release(instance);
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            Dispose();
        }

        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="SimpleCqrs.ServiceResolutionException"></exception>
        public object Resolve(Type type)
        {
            try
			{
				return Kernel.Get(type);
			}
			catch (Exception ex)
			{
				throw new ServiceResolutionException(type, ex);
			}
        }

        /// <summary>
        /// Resolves the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="SimpleCqrs.ServiceResolutionException"></exception>
        public T Resolve<T>(string key) where T : class
        {
            try
            {
                return Kernel.Get<T>(key);
            }
			catch (Exception ex)
			{
                throw new ServiceResolutionException(typeof(T), ex);
			}
        }

        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="SimpleCqrs.ServiceResolutionException"></exception>
        public T Resolve<T>() where T : class
        {
            try
            {
                return Kernel.Get<T>();
            }
            catch (Exception ex)
            {
                throw new ServiceResolutionException(typeof(T), ex);
            }
        }

        /// <summary>
        /// Resolves the services.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> ResolveServices<T>() where T : class
        {
            var services = Kernel.GetAll<T>();
            return new List<T>(services);
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        public void TearDown<TService>(TService instance) where TService : class
        {
            // Not needed for Ninject it doesn't keep references beyond the life cycle that was configured.
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Kernel != null && !Kernel.IsDisposed)
            {
                Kernel.Dispose();
            }
        }
    }
}