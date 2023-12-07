namespace EventSourcingCQRS.Domain
{
    public interface ITypeCatalog
    {
        Type[] LoadedTypes { get; }

        //Type[] GetDerivedTypes(Type type);

        //Type[] GetDerivedTypes<T>();

        //Type[] GetGenericInterfaceImplementations(Type type);

        //Type[] GetInterfaceImplementations(Type type);

        //Type[] GetInterfaceImplementations<T>();

        //Type GetTypeFromUnqualifiedTypeName(string unqualifiedTypeName);

        //Type GetTypeFromQualifiedTypeName(string qualifiedTypeName);
    }
}