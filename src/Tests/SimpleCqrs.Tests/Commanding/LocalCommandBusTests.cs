using System;
using AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.Core.Tests.Commanding
{
    [TestClass]
    public class LocalCommandBusTests
    {
        private AutoMoqer mocker;
        private MockServiceLocator serviceLocator;

        [TestInitialize]
        public void SetupMocksForAllTest()
        {
            mocker = new AutoMoqer();
            serviceLocator = new MockServiceLocator(mocker);
        }

        [TestMethod]
        public void CommandHandlerNotFoundExceptionIsNotThrownWhenNoCommandHandlerIsFoundForASentCommandAndCommandBusIsInTestMode()
        {
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] {typeof(MyTestCommandHandler)});

            var commandBus = CreateCommandBus(serviceLocator);
            ((IHaveATestMode)commandBus).IsInTestMode = true;

            commandBus.Send(new MyTest3Command());
        }

        [TestMethod]
        public void CommandHandlerNotFoundExceptionIsNotThrownWhenNoCommandHandlerIsFoundForAnExecutedCommandAndCommandBusIsInTestMode()
        {
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] {typeof(MyTestCommandHandler)});

            var commandBus = CreateCommandBus(serviceLocator);
            ((IHaveATestMode)commandBus).IsInTestMode = true;

            commandBus.Execute(new MyTest3Command {ReturnValue = 321});
        }

        [TestMethod]
        [ExpectedException(typeof(CommandHandlerNotFoundException))]
        public void CommandHandlerNotFoundExceptionIsThrownWhenNoCommandHandlerIsFoundForCommand()
        {
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] {typeof(MyTestCommandHandler)});

            var commandBus = CreateCommandBus(serviceLocator);
            commandBus.Execute(new MyTest3Command {ReturnValue = 321});
        }

        [TestMethod]
        public void CommandHandlerForMyCommandIsCalledWhenHandlerTypeIsInTypeCatalog()
        {
            serviceLocator.ResolveFunc = Activator.CreateInstance;

            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] {typeof(MyTestCommandHandler)});

            var commandBus = CreateCommandBus(serviceLocator);
            var result = commandBus.Execute(new MyTestCommand {ReturnValue = 321});

            Assert.AreEqual(321, result);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateCommandHandlersException))]
        public void DuplicateCommandHandlersExceptionIsThrownWhenTwoCommandHandlersForSameCommandExist()
        {
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[]
                             {
                                 typeof(MyTestCommandHandler),
                                 typeof(MyTestCommandHandler)
                             });

            var commandBus = CreateCommandBus(serviceLocator);
            commandBus.Execute(new MyTestCommand());
        }

        [TestMethod]
        public void CommandHandlerThatImplementsTwoHandlersAreCalledWhenHandlerTypeIsInTypeCatalog()
        {
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] {typeof(HandlerForTwoCommands)});

            var commandBus = CreateCommandBus(serviceLocator);
            var result = commandBus.Execute(new MyTestCommand {ReturnValue = 102});
            var result2 = commandBus.Execute(new MyTest2Command {ReturnValue = 45});

            Assert.AreEqual(102, result);
            Assert.AreEqual(45, result2);
        }

        [TestMethod]
        public void Handle_WhenExceptionIsThrown_InvokerThrowsInitialException()
        {
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] {typeof(MyTestCommandHandler)});

            mocker.GetMock<MyTestCommandHandler>()
                .Setup(handler => handler.Handle(It.IsAny<ICommandHandlingContext<MyTestCommand>>()))
                .Throws(new Exception("THE SANTA ROCKS!"));

            var commandBus = CreateCommandBus(serviceLocator);

            var ex = CustomAsserts.Throws<Exception>(() => commandBus.Execute(new MyTestCommand()));
            Assert.AreEqual("THE SANTA ROCKS!", ex.Message);
        }

        private ICommandBus CreateCommandBus(IServiceLocator serviceLocator)
        {
            var typeCatalog = mocker.Resolve<ITypeCatalog>();
            return new LocalCommandBus(typeCatalog, serviceLocator);
        }
    }

    public class MyTestCommandHandler : IHandleCommands<MyTestCommand>
    {
        public virtual void Handle(ICommandHandlingContext<MyTestCommand> handlingContext)
        {
            handlingContext.Return(handlingContext.Command.ReturnValue);
        }
    }

    public class HandlerForTwoCommands : IHandleCommands<MyTestCommand>, IHandleCommands<MyTest2Command>
    {
        public void Handle(ICommandHandlingContext<MyTestCommand> handlingContext)
        {
            handlingContext.Return(handlingContext.Command.ReturnValue);
        }

        public void Handle(ICommandHandlingContext<MyTest2Command> handlingContext)
        {
            handlingContext.Return(handlingContext.Command.ReturnValue);
        }
    }

    public class MyTestCommand : ICommand
    {
        public int ReturnValue { get; set; }
    }

    public class MyTest2Command : ICommand
    {
        public int ReturnValue { get; set; }
    }

    public class MyTest3Command : ICommand
    {
        public int ReturnValue { get; set; }
    }
}