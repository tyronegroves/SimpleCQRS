using System;
using AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;

namespace SimpleCqrs.Core.Tests.Commanding
{
    [TestClass]
    public class AggregateRootCommandHandlerTests
    {
        private AutoMoqer mocker;
        private MockServiceLocator mockServiceLocator;

        [TestInitialize]
        public void SetupMocksForAllTest()
        {
            mocker = new AutoMoqer();
            mockServiceLocator = new MockServiceLocator(mocker);
           
            ServiceLocator.SetCurrent(mockServiceLocator);
        }

        [TestMethod]
        public void When_the_command_is_handled_the_aggregateroot_with_the_aggregaterootid_is_retrieved_out_of_the_domainrepository()
        {
            var aggregateRootId = new Guid("438DBA36-8253-4AC3-BF37-0CBB86F167BB");
            var handlingContext = new MyHandlingContext();
            handlingContext.Command = new MyCommand{AggregateRootId = aggregateRootId};

            var commandHandler = CreateAggregateRootCommandHandler();
            commandHandler.Handle(handlingContext);

            mocker.GetMock<IDomainRepository>()
                .Verify(repository => repository.GetById<MyAggregateRoot>(aggregateRootId));
        }

        private IHandleCommands<MyCommand> CreateAggregateRootCommandHandler()
        {
            return mocker.Resolve<MyAggregateRootCommandHandler>();
        }
    }

    public class MyAggregateRootCommandHandler : AggregateRootCommandHandler<MyCommand, MyAggregateRoot>
    {
        public override void Handle(MyCommand command, MyAggregateRoot aggregateRoot)
        {
        }
    }

    public class MyCommand : CommandWithAggregateRootId
    {
    }

    public class MyAggregateRoot : AggregateRoot
    {
    }

    public class MyHandlingContext : ICommandHandlingContext<MyCommand>
    {
        public MyCommand Command { get; set; }

        public void Return(int value)
        {
            throw new NotImplementedException();
        }
    }
}