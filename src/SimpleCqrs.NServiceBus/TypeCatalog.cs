using System;
using System.Linq;
using NServiceBus;

namespace SimpleCqrs.NServiceBus
{
    public class TypeCatalog : ITypeCatalog
    {
        public Type[] GetDerivedTypes(Type type)
        {
            return (from derivedType in Configure.TypesToScan
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
            return (from derivedType in Configure.TypesToScan
                    from interfaceType in derivedType.GetInterfaces()
                    where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == type
                    select derivedType).Distinct().ToArray();
        }

        public Type[] GetInterfaceImplementations(Type type)
        {
            return (from derivedType in Configure.TypesToScan
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