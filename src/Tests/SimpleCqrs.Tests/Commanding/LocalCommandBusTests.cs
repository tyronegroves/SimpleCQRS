﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.Core.Tests.Commanding
{
    [TestClass]
    public class LocalCommandBusTests
    {
        private AutoMoqer mocker;

        [TestInitialize]
        public void SetupMocksForAllTest()
        {
            mocker = new AutoMoqer();
        }

        [TestMethod]
        public void CommandHandlerNotFoundExceptionIsNotThrownWhenNoCommandHandlerIsFoundForASentCommandAndCommandBusIsInTestMode()
        {
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] { typeof(MyTestCommandHandler) });

            var serviceLocator = new MockServiceLocator { ResolveFunc = t => new MyTestCommandHandler() };

            var commandBus = CreateCommandBus(serviceLocator);
            ((IHaveATestMode)commandBus).IsInTestMode = true;

            commandBus.Send(new MyTest3Command());
        }

        [TestMethod]
        public void CommandHandlerNotFoundExceptionIsNotThrownWhenNoCommandHandlerIsFoundForAnExecutedCommandAndCommandBusIsInTestMode()
        {
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] { typeof(MyTestCommandHandler) });

            var serviceLocator = new MockServiceLocator { ResolveFunc = t => new MyTestCommandHandler() };

            var commandBus = CreateCommandBus(serviceLocator);
            ((IHaveATestMode)commandBus).IsInTestMode = true;

            commandBus.Execute(new MyTest3Command { ReturnValue = 321 });
        }

        [TestMethod]
        [ExpectedException(typeof(CommandHandlerNotFoundException))]
        public void CommandHandlerNotFoundExceptionIsThrownWhenNoCommandHandlerIsFoundForCommand()
        {
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] { typeof(MyTestCommandHandler) });

            var serviceLocator = new MockServiceLocator { ResolveFunc = t => new MyTestCommandHandler() };

            var commandBus = CreateCommandBus(serviceLocator);
            commandBus.Execute(new MyTest3Command { ReturnValue = 321 });
        }

        [TestMethod]
        public void CommandHandlerForMyCommandIsCalledWhenHandlerTypeIsInTypeCatalog()
        {
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] { typeof(MyTestCommandHandler) });

            var serviceLocator = new MockServiceLocator { ResolveFunc = t => new MyTestCommandHandler() };

            var commandBus = CreateCommandBus(serviceLocator);
            var result = commandBus.Execute(new MyTestCommand { ReturnValue = 321 });

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
                                 typeof (MyTestCommandHandler),
                                 typeof (MyTestCommandHandler)
                             });

            var serviceLocator = new MockServiceLocator { ResolveFunc = t => new MyTestCommandHandler() };

            var commandBus = CreateCommandBus(serviceLocator);
            commandBus.Execute(new MyTestCommand());
        }

        [TestMethod]
        public void CommandHandlerThatImplementsTwoHandlersAreCalledWhenHandlerTypeIsInTypeCatalog()
        {
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] { typeof(HandlerForTwoCommands) });

            var serviceLocator = new MockServiceLocator { ResolveFunc = t => new HandlerForTwoCommands() };

            var commandBus = CreateCommandBus(serviceLocator);
            var result = commandBus.Execute(new MyTestCommand { ReturnValue = 102 });
            var result2 = commandBus.Execute(new MyTest2Command { ReturnValue = 45 });

            Assert.AreEqual(102, result);
            Assert.AreEqual(45, result2);
        }

        [TestMethod]
        public void Handle_WhenExceptionIsThrown_InvokerThrowsInitialException()
        {
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] { typeof(MyTestCommandHandler) });

            var myExceptionThrowingHandler = new MyTestCommandHandler
            {
                OnHandle = (ctx) => { throw new Exception("THE SANTA ROCKS!"); }
            };

            var serviceLocator = new MockServiceLocator { ResolveFunc = t => myExceptionThrowingHandler };

            var commandBus = CreateCommandBus(serviceLocator);

            var ex = CustomAsserts.Throws<Exception>(() => commandBus.Execute(new MyTestCommand()));
            Assert.AreEqual("THE SANTA ROCKS!", ex.InnerException.Message);
        }

        private ICommandBus CreateCommandBus(IServiceLocator serviceLocator)
        {
            var typeCatalog = mocker.Resolve<ITypeCatalog>();
            return new LocalCommandBus(typeCatalog, serviceLocator);
        }
    }

    public class MyTestCommandHandler : IHandleCommands<MyTestCommand>
    {
        public Action<ICommandHandlingContext<MyTestCommand>> OnHandle;

        public void Handle(ICommandHandlingContext<MyTestCommand> handlingContext)
        {
            if (OnHandle == null)
                handlingContext.Return(handlingContext.Command.ReturnValue);
            else
                OnHandle(handlingContext);
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

    public class MockServiceLocator : IServiceLocator
    {
        public Func<Type, object> ResolveFunc { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type)
        {
            return ResolveFunc(type);
        }

        public IList<T> ResolveServices<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public void Register<TInterface>(Type implType) where TInterface : class
        {
            throw new NotImplementedException();
        }

        public void Register<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            throw new NotImplementedException();
        }

        public void Register<TInterface, TImplementation>(string key) where TImplementation : class, TInterface
        {
            throw new NotImplementedException();
        }

        public void Register(string key, Type type)
        {
            throw new NotImplementedException();
        }

        public void Register(Type serviceType, Type implType)
        {
            throw new NotImplementedException();
        }

        public void Register<TInterface>(TInterface instance) where TInterface : class
        {
            throw new NotImplementedException();
        }

        public void Release(object instance)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public TService Inject<TService>(TService instance) where TService : class
        {
            throw new NotImplementedException();
        }

        public void TearDown<TService>(TService instance) where TService : class
        {
            throw new NotImplementedException();
        }

        public void Register<Interface>(Func<Interface> factoryMethod) where Interface : class
        {
            throw new NotImplementedException();
        }
    }
}