using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SimpleCqrs
{
    public class AssemblyTypeCatalog : ITypeCatalog
    {
        private readonly IEnumerable<Assembly> assemblies;

        public AssemblyTypeCatalog(IEnumerable<Assembly> assemblies)
        {
            this.assemblies = ValidateAssemblies(assemblies);
        }

        private static IEnumerable<Assembly> ValidateAssemblies(IEnumerable<Assembly> enumerable)
        {
            var validAssemblies = new List<Assembly>();
            foreach(var assembly in enumerable)
            {
                try
                {

                    assembly.GetTypes();
                    validAssemblies.Add(assembly);
                }
                catch(Exception exception)
                {
                    Debug.WriteLine(exception);
                }
            }

            return validAssemblies;
        }

        public Type[] GetDerivedTypes(Type type)
        {
            return (from assembly in assemblies
                   from derivedType in assembly.GetTypes()
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
            return (from assembly in assemblies
                    from derivedType in assembly.GetTypes()
                    from interfaceType in derivedType.GetInterfaces()
                    where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == type
                    select derivedType).Distinct().ToArray();
        }

        public Type[] GetInterfaceImplementations(Type type)
        {
            return (from assembly in assemblies
                    from derivedType in assembly.GetTypes()
                    where !derivedType.IsInterface
                    from interfaceType in derivedType.GetInterfaces()
                    where interfaceType == type
                    select derivedType).Distinct().ToArray();
        }

        public Type[] GetInterfaceImplementations<T>()
        {
            return GetInterfaceImplementations(typeof(T));
        }
    }
}