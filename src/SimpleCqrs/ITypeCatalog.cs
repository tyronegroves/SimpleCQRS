using System;

namespace SimpleCqrs
{
    public interface ITypeCatalog
    {
        Type[] GetDerivedTypes(Type type);
        Type[] GetDerivedTypes<T>();
        Type[] GetGenericInterfaceImplementations(Type type);
        Type[] GetInterfaceImplementations(Type type);
        Type[] GetInterfaceImplementations<T>();
    }
}