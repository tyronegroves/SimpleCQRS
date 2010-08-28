using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleCqrs
{
    internal class TypeCatalog : ITypeCatalog
    {
        private readonly IEnumerable<Assembly> assemblies;

        public TypeCatalog(IEnumerable<Assembly> assemblies)
        {
            this.assemblies = assemblies;
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