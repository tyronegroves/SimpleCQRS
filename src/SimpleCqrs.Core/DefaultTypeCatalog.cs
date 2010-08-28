using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleCqrs
{
    internal class DefaultTypeCatalog : ITypeCatalog
    {
        private readonly IEnumerable<Assembly> assemblies;

        public DefaultTypeCatalog(IEnumerable<Assembly> assemblies)
        {
            this.assemblies = assemblies;
        }

        public IEnumerable<Type> GetDerivedTypes(Type type)
        {
            return (from assembly in assemblies
                   from derivedType in assembly.GetTypes()
                   where type != derivedType
                   where type.IsAssignableFrom(derivedType)
                   select derivedType).ToList();
        }

        public IEnumerable<Type> GetGenericInterfaceImplementations(Type type)
        {
            return (from assembly in assemblies
                    from derivedType in assembly.GetTypes()
                    from interfaceType in derivedType.GetInterfaces()
                    where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == type
                    select derivedType).Distinct().ToList();
        }
    }
}