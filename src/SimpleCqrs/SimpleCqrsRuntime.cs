using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;
#if NETSTANDARD
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyModel;
using System.Diagnostics;
#endif

namespace SimpleCqrs
{
    /// <summary>
    /// Represents the runtime environment for a <b>SimpleCqrs</b> applications.
    /// </summary>
    /// <typeparam name="TServiceLocator">The type of the service locator.</typeparam>
    public class SimpleCqrsRuntime<TServiceLocator> : ISimpleCqrsRuntime
        where TServiceLocator : class, IServiceLocator
    {
        /// <summary>
        /// Gets the service locator associated with the runtime.
        /// </summary>
        public TServiceLocator ServiceLocator
        {
            get { return (TServiceLocator)SimpleCqrs.ServiceLocator.Current; }
        }

        /// <summary>
        /// Gets the service locator associated with the runtime.
        /// </summary>
        IServiceLocator ISimpleCqrsRuntime.ServiceLocator
        {
            get { return ServiceLocator; }
        }

        /// <summary>
        /// Starts the runtime environment.
        /// </summary>
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

            OnStarted(serviceLocator);
        }

        /// <summary>
        /// Called after the runtime has be started.
        /// </summary>
        /// <param name="serviceLocator">The current <see cref="IServiceLocator"/>.</param>
        protected virtual void OnStarted(TServiceLocator serviceLocator)
        {
        }

        /// <summary>
        /// Called when the runtime is to shutdown.
        /// </summary>
        /// <param name="serviceLocator">The current <see cref="IServiceLocator"/>.</param>
        protected virtual void OnShutdown(TServiceLocator serviceLocator)
        {
        }

        /// <summary>
        /// Shutdowns the runtime environment and release all the resouces held by the runtime environment.
        /// </summary>
        public void Shutdown()
        {
            OnShutdown(ServiceLocator);
            ServiceLocator.Dispose();
        }

        /// <summary>
        /// When overriden this method returns the <see cref="ICommandBus"/> that will be used by the runtime.  
        /// The default implementation returns an instance of <see cref="LocalCommandBus"/>.
        /// </summary>
        /// <param name="serviceLocator">The current <see cref="IServiceLocator"/>.</param>
        /// <returns>An instance of <see cref="ICommandBus"/>.</returns>
        protected virtual ICommandBus GetCommandBus(IServiceLocator serviceLocator)
        {
            return new LocalCommandBus(serviceLocator.Resolve<ITypeCatalog>(), serviceLocator);
        }

        /// <summary>
        /// When overriden this method returns the <see cref="IEventBus"/> that will be used by the runtime.  
        /// The default implementation returns an instance of <see cref="LocalEventBus"/>.
        /// </summary>
        /// <param name="serviceLocator">The current <see cref="IServiceLocator"/>.</param>
        /// <returns>An instance of <see cref="IEventBus"/>.</returns>
        protected virtual IEventBus GetEventBus(IServiceLocator serviceLocator)
        {
            var typeCatalog = serviceLocator.Resolve<ITypeCatalog>();
            var domainEventHandlerFactory = serviceLocator.Resolve<DomainEventHandlerFactory>();
            var domainEventTypes = typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleDomainEvents<>));

            return new LocalEventBus(domainEventTypes, domainEventHandlerFactory);
        }

        /// <summary>
        /// When overriden this method returns the assemblies that will be scanned for types used by the runtime.  
        /// The default implementation returns the assemblies returned from <see cref="AppDomain.GetAssemblies"/>.
        /// </summary>
        /// <param name="serviceLocator">The current <see cref="IServiceLocator"/>.</param>
        /// <returns>An <see cref="IEnumerable{Assembly}"/> containing the assemblies to scan.</returns>
        protected virtual IEnumerable<Assembly> GetAssembliesToScan(IServiceLocator serviceLocator)
        {
#if NETSTANDARD
            var programType = typeof(SimpleCqrsRuntime<>);
            var dependencyModel = DependencyContext.Load(programType.GetTypeInfo().Assembly);
            var assemblyNames = dependencyModel.GetRuntimeAssemblyNames(RuntimeEnvironment.GetRuntimeIdentifier());
            return assemblyNames.Select(name =>
            {
                try
                {
                    try
                    {
                        var assembly = Assembly.Load(name);
                        // just load all types and skip this assembly if one or more types cannot be resolved
                        assembly.DefinedTypes.ToArray();
                        return assembly;
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.Message);
                }

                return default(Assembly);
            }).Where(w => w != default(Assembly)).OrderBy(o => o.FullName).ToArray();
#else
            return AppDomain.CurrentDomain.GetAssemblies();
#endif
        }

        /// <summary>
        /// When overriden this method returns the <see cref="ITypeCatalog"/> that will be used by the runtime.  
        /// The default implementation returns an instance of <see cref="AssemblyTypeCatalog"/>.
        /// </summary>
        /// <param name="serviceLocator">The current <see cref="IServiceLocator"/>.</param>
        /// <returns>An instance of <see cref="ITypeCatalog"/>.</returns>
        protected virtual ITypeCatalog GetTypeCatalog(IServiceLocator serviceLocator)
        {
            return new AssemblyTypeCatalog(GetAssembliesToScan(serviceLocator));
        }

        /// <summary>
        /// When overriden this method returns the <see cref="ISnapshotStore"/> that will be used by the runtime.  
        /// The default implementation returns an instance of <see cref="NullSnapshotStore"/>.
        /// </summary>
        /// <param name="serviceLocator">The current <see cref="IServiceLocator"/>.</param>
        /// <returns>An instance of <see cref="ISnapshotStore"/>.</returns>
        protected virtual ISnapshotStore GetSnapshotStore(IServiceLocator serviceLocator)
        {
            return new NullSnapshotStore();
        }

        /// <summary>
        /// When overriden this method returns the <see cref="IEventStore"/> that will be used by the runtime.  
        /// The default implementation returns an instance of <see cref="MemoryEventStore"/>.
        /// </summary>
        /// <param name="serviceLocator">The current <see cref="IServiceLocator"/>.</param>
        /// <returns>An instance of <see cref="IEventStore"/>.</returns>
        protected virtual IEventStore GetEventStore(IServiceLocator serviceLocator)
        {
            return new MemoryEventStore();
        }

        /// <summary>
        /// When overriden this method returns the <see cref="TServiceLocator"/> that will be used by the runtime.  
        /// The default implementation returns an instance of <see cref="TServiceLocator"/>.
        /// </summary>
        /// <returns>An instance of <see cref="TServiceLocator"/>.</returns>
        protected virtual TServiceLocator GetServiceLocator()
        {
            return Activator.CreateInstance<TServiceLocator>();
        }

        protected virtual IEnumerable<IRegisterComponents> GetComponentRegistrars(Type componentRegistarType, IServiceLocator serviceLocator)
        {
            var typeCatalog = serviceLocator.Resolve<ITypeCatalog>();
#if NETSTANDARD
            var componentRegistrarTypes = componentRegistarType.GetTypeInfo().IsInterface ?
                typeCatalog.GetInterfaceImplementations(componentRegistarType) :
                typeCatalog.GetDerivedTypes(componentRegistarType);
#else
            var componentRegistrarTypes = componentRegistarType.IsInterface ?
                typeCatalog.GetInterfaceImplementations(componentRegistarType) :
                typeCatalog.GetDerivedTypes(componentRegistarType);
#endif

            return componentRegistrarTypes
                .Select(serviceLocator.Resolve)
                .Cast<IRegisterComponents>();
        }

        protected virtual void RegisterComponents(IServiceLocator serviceLocator)
        {
            var componentRegistrars = GetComponentRegistrars(typeof(IRegisterComponents), serviceLocator).ToList();
            componentRegistrars.ForEach(componentRegistrar => componentRegistrar.Register(serviceLocator));
        }

        /// <summary>
        /// Shutdowns the runtime and disposes of the resources in the service locator.
        /// </summary>
        /// <remarks>This calls <see cref="SimpleCqrsRuntime{TServiceLocator}.Shutdown">Shutdown</see>.</remarks>
        public void Dispose()
        {
            Shutdown();
        }
    }
}