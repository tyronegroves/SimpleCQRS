// -----------------------------------------------------------------------
// https://github.com/jupaol
// jupaol@hotmail.com
// http://jupaol.blogspot.com/
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;

namespace SimpleCqrs.Ninject
{
    /// <summary>
    /// Ninject service locator
    /// </summary>
    public class NinjectServiceLocator : IServiceLocator
    {
        /// <summary>
        /// Ninject kernel used as the current IoC container
        /// </summary>
        private IKernel kernel;

        /// <summary>
        /// Error message used when the type specified in the register methods is null
        /// </summary>
        private const string TypeNullErrorMessage = "The container does not accept null types to be registered";

        /// <summary>
        /// Error message used when the key specified in the register methods is null, empty or a string with white spaces only
        /// </summary>
        private const string KeyNullErrorMessage = "The key cannot be null, empty or a string with white spaces only";

        /// <summary>
        /// Error message used when the kernel is null
        /// </summary>
        private const string KernelNullErrorMessage = "The specified Ninject kernel cannot be null";

        /// <summary>
        /// Error message used when the reset method is called, actually it is not supported because Ninject does not suppor reseting the container
        /// </summary>
        private const string ResetingNinjectContainerErrorMessage = "Ninject does not support reseting the container";

        /// <summary>
        /// Error message used when the instance passed to the register methods is null
        /// </summary>
        private const string InstanceNullErrorMessage = "Null objects cannot be registered in the container";

        /// <summary>
        /// Error message used when the calling delegate passed to the register methods is null
        /// </summary>
        private const string CallingDelegateNullErrorMessage = "The calling delegate connot be null";

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectServiceLocator"/> class.
        /// </summary>
        /// <remarks>
        /// It creates a new instance of the <see cref="StandardKernel"/> to initialize this instance
        /// </remarks>
        public NinjectServiceLocator()
            : this(new StandardKernel())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectServiceLocator"/> class.
        /// </summary>
        /// <param name="kernel">The Ninject kernel.</param>
        public NinjectServiceLocator(IKernel kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel", KernelNullErrorMessage);
            }

            this.IsDisposed = false;
            this.kernel = kernel;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get; protected set; }

        public T Resolve<T>() where T : class
        {
            try
            {
                return this.kernel.Get<T>();
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
                return this.kernel.Get<T>(key);
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
                return this.kernel.Get(type);
            }
            catch (Exception ex)
            {
                throw new ServiceResolutionException(typeof(Type), ex);
            }
        }

        public IList<T> ResolveServices<T>() where T : class
        {
            try
            {
                return this.kernel.GetAll<T>().ToList();
            }
            catch (Exception ex)
            {
                throw new ServiceResolutionException(typeof(T), ex);
            }
        }

        public void Register<TInterface>(Type implType) where TInterface : class
        {
            if (implType == null)
            {
                throw new ArgumentNullException("implType", TypeNullErrorMessage);
            }

            this.kernel.Bind<TInterface>().To(implType);
        }

        public void Register<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            this.kernel.Bind<TInterface>().To<TImplementation>();
        }

        public void Register<TInterface, TImplementation>(string key) where TImplementation : class, TInterface
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key", KeyNullErrorMessage);
            }

            this.kernel.Bind<TInterface>().To<TImplementation>().Named(key);
        }

        public void Register(string key, Type type)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key", KeyNullErrorMessage);
            }

            if (type == null)
            {
                throw new ArgumentNullException("implType", TypeNullErrorMessage);
            }

            this.kernel.Bind(type).ToSelf().Named(key);
        }

        public void Register(Type serviceType, Type implType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType", TypeNullErrorMessage);
            }

            if (implType == null)
            {
                throw new ArgumentNullException("implType", TypeNullErrorMessage);
            }

            this.kernel.Bind(serviceType).To(implType);
        }

        public void Register<TInterface>(TInterface instance) where TInterface : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance", InstanceNullErrorMessage);
            }

            this.kernel.Bind<TInterface>().ToConstant(instance);
        }

        public void Release(object instance)
        {
            if (instance == null)
            {
                return;
            }

            // TODO: verify call
            this.kernel.Release(instance);
        }

        public void Reset()
        {
            throw new NotImplementedException(ResetingNinjectContainerErrorMessage);
        }

        public TService Inject<TService>(TService instance) where TService : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance", InstanceNullErrorMessage);
            }

            this.kernel.Inject(instance);
            return instance;
        }

        public void TearDown<TService>(TService instance) where TService : class
        {
            if (instance == null)
            {
                return;
            }

            // TODO: verify call
            this.kernel.Release(instance);
        }

        public void Register<TInterface>(Func<TInterface> factoryMethod) where TInterface : class
        {
            if (factoryMethod == null)
            {
                throw new ArgumentNullException("factoryMethod", CallingDelegateNullErrorMessage);
            }

            TInterface theInstance = factoryMethod();

            if (theInstance == null)
            {
                throw new ArgumentNullException("factoryMethod", TypeNullErrorMessage);
            }

            this.kernel.Bind<TInterface>().ToMethod(x => theInstance);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (disposing)
                {
                    if (this.kernel != null)
                    {
                        if (!this.kernel.IsDisposed)
                        {
                            this.kernel.Dispose();
                        }
                    }
                }

                this.IsDisposed = true;
            }
        }
    }
}
