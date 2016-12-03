﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SimpleCqrs
{
    public class AssemblyTypeCatalog : ITypeCatalog
    {
        private readonly IEnumerable<Type> loadedTypes;

        public AssemblyTypeCatalog(IEnumerable<Assembly> assemblies)
        {
            loadedTypes = LoadTypes(assemblies);
        }

        public Type[] LoadedTypes
        {
            get { return loadedTypes.ToArray(); }
        }

        public Type[] GetDerivedTypes(Type type)
        {
            return (
                       from derivedType in loadedTypes
                       where type != derivedType
                       where type.IsAssignableFrom(derivedType)
                       select derivedType).ToArray();
        }

        public Type[] GetDerivedTypes<T>()
        {
            return GetDerivedTypes(typeof(T));
        }

        public Type[] GetGenericInterfaceImplementations(Type type)
        {
#if NETSTANDARD
            return (from derivedType in loadedTypes
                    from interfaceType in derivedType.GetInterfaces()
                    where interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == type
                    select derivedType).Distinct().ToArray();
#else
            return (from derivedType in loadedTypes
                    from interfaceType in derivedType.GetInterfaces()
                    where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == type
                    select derivedType).Distinct().ToArray();
#endif
        }

        public Type[] GetInterfaceImplementations(Type type)
        {
#if NETSTANDARD
            return (from derivedType in loadedTypes
                    where !derivedType.GetTypeInfo().IsInterface
                    from interfaceType in derivedType.GetInterfaces()
                    where interfaceType == type
                    select derivedType).Distinct().ToArray();
#else
            return (from derivedType in loadedTypes
                    where !derivedType.IsInterface
                    from interfaceType in derivedType.GetInterfaces()
                    where interfaceType == type
                    select derivedType).Distinct().ToArray();
#endif
        }

        public Type[] GetInterfaceImplementations<T>()
        {
            return GetInterfaceImplementations(typeof(T));
        }

        private static IEnumerable<Type> LoadTypes(IEnumerable<Assembly> assemblies)
        {
            var loadedTypes = new List<Type>();
            foreach(var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    loadedTypes.AddRange(types);
                }
                catch(ReflectionTypeLoadException exception)
                {
                    exception.LoaderExceptions
                        .Select(e => e.Message)
                        .Distinct().ToList()
                        .ForEach(message => Debug.WriteLine(message));
                }
            }

            return loadedTypes;
        }
    }
}