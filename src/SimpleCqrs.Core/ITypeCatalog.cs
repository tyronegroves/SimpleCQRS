using System;
using System.Collections.Generic;

namespace SimpleCqrs
{
    public interface ITypeCatalog
    {
        IEnumerable<Type> GetDerivedTypes(Type type);
        IEnumerable<Type> GetGenericInterfaceImplementations(Type type);
    }
}