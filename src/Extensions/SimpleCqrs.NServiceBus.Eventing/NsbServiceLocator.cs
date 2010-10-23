using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus.ObjectBuilder;

namespace SimpleCqrs.NServiceBus.Eventing
{
    internal class NsbServiceLocator : IServiceLocator
    {
        private readonly IConfigureComponents configurer;
        private readonly IBuilder builder;

        public NsbServiceLocator(IConfigureComponents configurer, IBuilder builder)
        {
            this.configurer = configurer;
            this.builder = builder;
        }

        public void Dispose()
        { 
        }

        public T Resolve<T>() where T : class
        {
            return builder.Build<T>();
        }

        public T Resolve<T>(string key) where T : class
        {
            return builder.Build<T>();
        }

        public object Resolve(Type type)
        {
            return builder.Build(type);
        }

        public IList<T> ResolveServices<T>() where T : class
        {
            return builder.BuildAll<T>().ToList();
        }

        public void Register<TInterface>(Type implType) where TInterface : class
        {
            configurer.ConfigureComponent(implType, ComponentCallModelEnum.None);
        }

        public void Register<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            configurer.ConfigureComponent<TImplementation>(ComponentCallModelEnum.None);
        }

        public void Register<TInterface, TImplementation>(string key) where TImplementation : class, TInterface
        {
            configurer.ConfigureComponent<TImplementation>(ComponentCallModelEnum.None);
        }

        public void Register(string key, Type type)
        {
            configurer.ConfigureComponent(type, ComponentCallModelEnum.None);
        }

        public void Register(Type serviceType, Type implType)
        {
            configurer.ConfigureComponent(implType, ComponentCallModelEnum.None);
        }

        public void Register<TInterface>(TInterface instance) where TInterface : class
        {
            configurer.RegisterSingleton<TInterface>(instance);
        }

        public void Release(object instance)
        {
        }

        public void Reset()
        {
        }

        public TService Inject<TService>(TService instance) where TService : class
        {
            throw new NotSupportedException();
        }

        public void TearDown<TService>(TService instance) where TService : class
        {
            throw new NotSupportedException();
        }

        public void Register<TInterface>(Func<TInterface> factoryMethod) where TInterface : class
        {
            throw new NotSupportedException();
        }
    }
}