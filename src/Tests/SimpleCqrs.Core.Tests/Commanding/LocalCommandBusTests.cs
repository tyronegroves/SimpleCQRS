using System;
using System.Collections.Generic;
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
        public void CommandHandlerForMyCommandIsCalledWhenHandlerTypeIsInTypeCatalog()
        {
            ReturnTheseHandlersForThisType(typeof (IHandleCommands<>), new[] {typeof (MyTestCommandHandler)});

            var serviceLocator = CreateServiceLocatorThatWillReturnThis(t => new MyTestCommandHandler());

            var commandBus = CreateCommandBus(serviceLocator);
            var result = commandBus.Execute(new MyTestCommand {ReturnValue = 321});

            Assert.AreEqual(321, result);
        }

        [TestMethod]
        [ExpectedException(typeof (DuplicateCommandHandlersException))]
        public void DuplicateCommandHandlersExceptionIsThrownWhenTwoCommandHandlersForSameCommandExist()
        {
            ReturnTheseHandlersForThisType(typeof (IHandleCommands<>),
                                           new[]
                                               {
                                                   typeof (MyTestCommandHandler),
                                                   typeof (MyTestCommandHandler)
                                               });

            var serviceLocator = CreateServiceLocatorThatWillReturnThis(t => new MyTestCommandHandler());

            var commandBus = CreateCommandBus(serviceLocator);
            commandBus.Execute(new MyTestCommand());
        }

        [TestMethod]
        public void CommandHandlerThatImplementsTwoHandlersAreCalledWhenHandlerTypeIsInTypeCatalog()
        {
            ReturnTheseHandlersForThisType(typeof (IHandleCommands<>), new[] {typeof (HandlerForTwoCommands)});

            var serviceLocator = CreateServiceLocatorThatWillReturnThis(t => new HandlerForTwoCommands());

            var commandBus = CreateCommandBus(serviceLocator);
            var result = commandBus.Execute(new MyTestCommand {ReturnValue = 102});
            var result2 = commandBus.Execute(new MyTest2Command {ReturnValue = 45});

            Assert.AreEqual(102, result);
            Assert.AreEqual(45, result2);
        }

        private MockServiceLocator CreateServiceLocatorThatWillReturnThis(Func<Type, object> resolveFunc)
        {
            return new MockServiceLocator {ResolveFunc = resolveFunc};
        }

        private void ReturnTheseHandlersForThisType(Type genericHandlerType, Type[] handlers)
        {
            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(genericHandlerType))
                .Returns(handlers);
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