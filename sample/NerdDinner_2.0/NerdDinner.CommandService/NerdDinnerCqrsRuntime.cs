using System;
using SimpleCqrs;
using SimpleCqrs.Eventing;
using SimpleCqrs.EventStore.File;
using SimpleCqrs.Unity;

namespace NerdDinner.CommandService
{
    public class NerdDinnerCqrsRuntime : SimpleCqrsRuntime<UnityServiceLocator>
    {
        protected override IEventStore GetEventStore(IServiceLocator serviceLocator)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return new FileEventStore(baseDirectory, serviceLocator.Resolve<ITypeCatalog>());
        }
    }
}