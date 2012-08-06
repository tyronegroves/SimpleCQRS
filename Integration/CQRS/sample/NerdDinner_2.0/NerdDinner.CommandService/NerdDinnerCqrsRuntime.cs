using System;
using System.IO;
using NerdDinner.CommandService.Models;
using NerdDinner.CommandService.Services;
using SimpleCqrs;
using SimpleCqrs.Eventing;
using SimpleCqrs.EventStore.File;
using SimpleCqrs.Unity;

namespace NerdDinner.CommandService
{
    public class NerdDinnerCqrsRuntime : SimpleCqrsRuntime<UnityServiceLocator>
    {
        protected override UnityServiceLocator GetServiceLocator()
        {
            var serviceLocator = base.GetServiceLocator();
            serviceLocator.Register<IDinnerService, DinnerService>();
            serviceLocator.Register<IUserService, UserService>();
            return serviceLocator;
        }

        protected override IEventStore GetEventStore(IServiceLocator serviceLocator)
        {
            var baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EventStoreData");
            return new FileEventStore(baseDirectory, serviceLocator.Resolve<ITypeCatalog>());
        }
    }
}