using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleCqrs;
using SimpleCqrs.Commands;
using SimpleCqrs.Events;
using SimpleCqrs.EventStore.MongoDb;

namespace SimpleCrqs.EventStore.MongoDb.Tests
{
    [TestClass]
    public class MongoEventStoreTests
    {
        [TestMethod]
        public void Test()
        {
            var bootstrapper = new MyBootstrapper();
            var serviceLocator = bootstrapper.Run();
            
            var commandBus = serviceLocator.Resolve<ICommandBus>();

            commandBus.Execute(new MyCommand());
            commandBus.Execute(new MyCommand2());
        }
    }

    public class MyCommandHandler2 : MyCommandHandler
    {
        public override void Handle(MyCommand command)
        {
            base.Handle(command);
        }
    }

    public class MyCommandHandler : IHandleCommands<MyCommand>, IHandleCommands<MyCommand2>
    {
        public virtual void Handle(MyCommand command)
        {
        }

        public virtual void Handle(MyCommand2 command)
        {
        }
    }

    public class MyCommand2 : Command
    {
    }

    public class MyCommand : Command
    {
    }

    public class MyEventHandler : IHandleDomainEvents<MyEvent>
    {
        public void Handle(MyEvent domainEvent)
        {  
        }
    }

    public class MyEvent : DomainEvent
    {
    }

    public class MyBootstrapper : Bootstrapper
    {
        protected override ICommandBus GetCommandBus(ITypeCatalog typeCatalog)
        {
            var domainEvent = typeCatalog.GetDerivedTypes(typeof(DomainEvent));


            return base.GetCommandBus(typeCatalog);
        }
    }
}