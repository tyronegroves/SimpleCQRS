using System;
using System.Collections.Generic;

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

namespace SimpleCqrs
{
    public interface IServiceLocator : IDisposable
    {
        T Resolve<T>() where T : class;
        T Resolve<T>(string key) where T : class;
        object Resolve(Type type);
        IList<T> ResolveServices<T>() where T : class;
        void Register<TInterface>(Type implType) where TInterface : class;
        void Register<TInterface, TImplementation>() where TImplementation : class, TInterface;
        void Register<TInterface, TImplementation>(string key) where TImplementation : class, TInterface;
        void Register(string key, Type type);
        void Register(Type serviceType, Type implType);
        void Register<TInterface>(TInterface instance) where TInterface : class;
        void Release(object instance);
        void Reset();
        TService Inject<TService>(TService instance) where TService : class;
        void TearDown<TService>(TService instance) where TService : class;
        void Register<Interface>(Func<Interface> factoryMethod) where Interface : class;
    }
}