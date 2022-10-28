using System.Diagnostics;
using System.Reflection;

namespace EventSourcingCQRS.Domain
{
    public class AssemblyTypeCatalog : ITypeCatalog
    {
        private readonly IEnumerable<Type> _loadedTypes;

        public AssemblyTypeCatalog(IEnumerable<Assembly> assemblies)
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains("MD.Vaccination") || a.FullName.Contains("MD.MyBsa.EventHandlers"));
            _loadedTypes = LoadTypes(assemblies);
        }

        public Type[] LoadedTypes => _loadedTypes.ToArray();

        public Type[] GetDerivedTypes(Type type)
        {
            return (
                from derivedType in _loadedTypes
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
            return (from derivedType in _loadedTypes
                    from interfaceType in derivedType.GetInterfaces()
                    where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == type
                    select derivedType).Distinct().ToArray();
        }

        public Type[] GetInterfaceImplementations(Type type)
        {
            return (from derivedType in _loadedTypes
                    where !derivedType.IsInterface
                    from interfaceType in derivedType.GetInterfaces()
                    where interfaceType == type
                    select derivedType).Distinct().ToArray();
        }

        public Type[] GetInterfaceImplementations<T>()
        {
            return GetInterfaceImplementations(typeof(T));
        }

        public Type GetTypeFromUnqualifiedTypeName(string unqualifiedTypeName)
        {
            var assembly =
                LoadedTypes.FirstOrDefault(c => c.FullName != null && c.FullName.EndsWith(unqualifiedTypeName));
            if (assembly == null || string.IsNullOrWhiteSpace(assembly.AssemblyQualifiedName))
            {
                return null;
            }

            return Type.GetType(assembly.AssemblyQualifiedName);
        }

        public Type GetTypeFromQualifiedTypeName(string qualifiedTypeName)
        {
            var assembly = LoadedTypes.FirstOrDefault(c => c.AssemblyQualifiedName == qualifiedTypeName);
            if (assembly == null || string.IsNullOrWhiteSpace(assembly.AssemblyQualifiedName))
            {
                return null;
            }

            return Type.GetType(assembly.AssemblyQualifiedName);
        }

        private static IEnumerable<Type> LoadTypes(IEnumerable<Assembly> assemblies)
        {
            var loadedTypes = new List<Type>();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    loadedTypes.AddRange(types);
                }
                catch (ReflectionTypeLoadException exception)
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