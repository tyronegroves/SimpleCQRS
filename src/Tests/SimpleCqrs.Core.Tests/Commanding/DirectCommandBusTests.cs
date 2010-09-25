using System;
using System.Collections.Generic;
using AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.Core.Tests.Commanding
{
    [TestClass]
    public class DirectCommandBusTests
    {
        private AutoMoqer mocker;

        [TestInitialize]
        public void SetupMocksForAllTest()
        {
            mocker = new AutoMoqer();
        }

        [TestMethod]
        public void CommandHandlerForMyCommandIsCalledWhenHandlerTypeIsInTypeCatalog()
        {
            var serviceLocator = new MockServiceLocator {ResolveFunc = t => new MyTestCommandHandler()};
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] {typeof(MyTestCommandHandler)});

            var commandBus = CreateCommandBus(serviceLocator);
            var result = commandBus.Execute(new MyTestCommand());

            Assert.AreEqual(101, result);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateCommandHandlersException))]
        public void DuplicateCommandHandlersExceptionIsThrowWhenTwoCommand()
        {
            var serviceLocator = new MockServiceLocator {ResolveFunc = t => new MyTestCommandHandler()};
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] {typeof(MyTestCommandHandler), typeof(MyTestCommandHandler)});

            var commandBus = CreateCommandBus(serviceLocator);
            commandBus.Execute(new MyTestCommand());
        }

        [TestMethod]
        public void CommandHandlerThatImplementsTwoHandlersAreCalledWhenHandlerTypeIsInTypeCatalog()
        {
            var serviceLocator = new MockServiceLocator {ResolveFunc = t => new MyTest2CommandHandler()};
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleCommands<>)))
                .Returns(new[] {typeof(MyTest2CommandHandler)});

            var commandBus = CreateCommandBus(serviceLocator);
            var result = commandBus.Execute(new MyTestCommand());
            var result2 = commandBus.Execute(new MyTest2Command());

            Assert.AreEqual(102, result);
            Assert.AreEqual(45, result2);
        }

        private ICommandBus CreateCommandBus(IServiceLocator serviceLocator)
        {
            var typeCatalog = mocker.Resolve<ITypeCatalog>();
            return new LocalCommandBus(typeCatalog, serviceLocator);
        }
    }

    public class MyTestCommandHandler : IHandleCommands<MyTestCommand>
    {
        public void Handle(ICommandHandlingContext<MyTestCommand> handlingContext)
        {
            handlingContext.Return(101);
        }
    }

    public class MyTest2CommandHandler : IHandleCommands<MyTestCommand>, IHandleCommands<MyTest2Command>
    {
        public void Handle(ICommandHandlingContext<MyTestCommand> handlingContext)
        {
            handlingContext.Return(102);
        }

        public void Handle(ICommandHandlingContext<MyTest2Command> handlingContext)
        {
            handlingContext.Return(45);
        }
    }

    public class MyTestCommand : ICommand
    {
    }

    public class MyTest2Command : ICommand
    {
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
    }
}