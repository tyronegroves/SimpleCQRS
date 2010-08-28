using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

#region License

//
// Author: Javier Lozano <javier@lozanotek.com>
// Copyright (c) 2009-2010, lozanotek, inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#endregion

namespace SimpleCqrs.Unity
{
    public class UnityServiceLocator : IServiceLocator
    {
        public UnityServiceLocator()
            : this(new UnityContainer())
        {
        }

        public UnityServiceLocator(IUnityContainer container)
        {
            if(container == null)
                throw new ArgumentNullException("container", "The specified Unity container cannot be null.");

            Container = container;
        }

        public IUnityContainer Container { private set; get; }

        public T Resolve<T>() where T : class
        {
            try
            {
                return Container.Resolve<T>();
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
                return Container.Resolve<T>(key);
            }
            catch (Exception ex)
            {
                throw new ServiceResolutionException(typeof(T), ex);
            }
        }

        public T Resolve<T>(Type type) where T : class
        {
            try
            {
                return Container.Resolve(type) as T;
            }
            catch (Exception ex)
            {
                throw new ServiceResolutionException(type, ex);
            }
        }

        public object Resolve(Type type)
        {
            try
            {
                return Container.Resolve(type);
            }
            catch (Exception ex)
            {
                throw new ServiceResolutionException(type, ex);
            }
        }

        public IList<T> ResolveServices<T>() where T : class
        {
            var services = Container.ResolveAll<T>();
            return new List<T>(services);
        }

        public void Register<TInterface>(Type implType) where TInterface : class
        {
            var key = string.Format("{0}-{1}", typeof(TInterface).Name, implType.FullName);
            Container.RegisterType(typeof(TInterface), implType, key);

            // Work-around, also register this implementation to service mapping
            // without the generated key above.
            Container.RegisterType(typeof(TInterface), implType);
        }

        public void Register<TInterface, TImplementation>()
            where TImplementation : class, TInterface
        {
            Container.RegisterType<TInterface, TImplementation>();
        }

        public void Register<TInterface, TImplementation>(string key)
            where TImplementation : class, TInterface
        {
            Container.RegisterType<TInterface, TImplementation>(key);
        }

        public void Register(string key, Type type)
        {
            Container.RegisterType(type, key);
        }

        public void Register(Type serviceType, Type implType)
        {
            Container.RegisterType(serviceType, implType);
        }

        public void Register<TInterface>(TInterface instance) where TInterface : class
        {
            Container.RegisterInstance(instance);
        }

        public void Release(object instance)
        {
            if (instance == null) return;

            Container.Teardown(instance);
        }

        public void Reset()
        {
            Dispose();
        }

        public TService Inject<TService>(TService instance) where TService : class
        {
            return instance == null ? instance : (TService)Container.BuildUp(instance.GetType(), instance);
        }

        public void TearDown<TService>(TService instance) where TService : class
        {
            if (instance == null) return;
            Container.Teardown(instance);
        }

        public void Dispose()
        {
            // Cannot call Dispose on the Unity container.
            // If Unity is registered with itself (which includes registering an instance of the IServiceLocator),
            // it will get caught in an endless loop trying to dispose of itself.
        }
    }
}