using System;
using System.Collections.Generic;
using AutoMoq;

namespace SimpleCqrs.Core.Tests.Commanding
{
    public class MockServiceLocator : IServiceLocator
    {
        private readonly AutoMoqer mocker;
        private Func<Type, object> resolveFunc;

        public MockServiceLocator(AutoMoqer mocker)
        {
            this.mocker = mocker;
        }

        public Func<Type, object> ResolveFunc
        {
            set { resolveFunc = value; }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>() where T : class
        {
            if (resolveFunc != null)
                return (T)resolveFunc(typeof(T));

            return mocker.GetMock<T>().Object;
        }

        public T Resolve<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type)
        {
            if (resolveFunc != null)
                return resolveFunc(type);

            dynamic mock = typeof(AutoMoqer).GetMethod("GetMock").MakeGenericMethod(type).Invoke(mocker, null);
            return mock.Object;
        }

        public IList<T> ResolveServices<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public void Register<TInterface>(Type implType) where TInterface : class
        {
            throw new NotImplementedException();
        }

        public void Register<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            throw new NotImplementedException();
        }

        public void Register<TInterface, TImplementation>(string key) where TImplementation : class, TInterface
        {
            throw new NotImplementedException();
        }

        public void Register(string key, Type type)
        {
            throw new NotImplementedException();
        }

        public void Register(Type serviceType, Type implType)
        {
            throw new NotImplementedException();
        }

        public void Register<TInterface>(TInterface instance) where TInterface : class
        {
            throw new NotImplementedException();
        }

        public void Release(object instance)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public TService Inject<TService>(TService instance) where TService : class
        {
            throw new NotImplementedException();
        }

        public void TearDown<TService>(TService instance) where TService : class
        {
            throw new NotImplementedException();
        }

        public void Register<Interface>(Func<Interface> factoryMethod) where Interface : class
        {
            throw new NotImplementedException();
        }
    }
}