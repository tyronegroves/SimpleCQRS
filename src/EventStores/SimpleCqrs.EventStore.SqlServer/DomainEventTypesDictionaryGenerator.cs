using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.EventStore.SqlServer
{
    internal class DomainEventTypesDictionaryGenerator
    {
        public IDictionary<string, Type> GenerateDictionaryOfDomainTypes()
        {
            var list = GetAListOfAllDomainEventTypes();
            return list.ToDictionary(x => x.Name, x => x);
        }

        private IEnumerable<Type> GetAListOfAllDomainEventTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Select(GetTheDomainEventTypesFromThisAssembly)
                .SelectMany(x => x);
        }

        private static IEnumerable<Type> GetTheDomainEventTypesFromThisAssembly(Assembly assembly)
        {
            try
            {
                return MakeACallToGetTheTypesFromTheAssemblyInAWayThatCouldPotentiallyThrow(assembly);
            }
            catch
            {
                return new Type[] {};
            }
        }

        private static IEnumerable<Type> MakeACallToGetTheTypesFromTheAssemblyInAWayThatCouldPotentiallyThrow(Assembly assembly)
        {
            return assembly.GetTypes().Where(x => x.BaseType == typeof (DomainEvent));
        }
    }
}