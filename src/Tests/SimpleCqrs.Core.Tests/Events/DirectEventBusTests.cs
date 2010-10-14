using System;
using System.Collections.Generic;
using AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Core.Tests.Events
{
    [TestClass]
    public class DirectEventBusTests
    {
        private AutoMoqer mocker;

        [TestInitialize]
        public void SetupMocksForAllTests()
        {
            mocker = new AutoMoqer();
        }

        [TestMethod]
        public void DomainEventHandlerForMyTestEventIsCalledWhenHandlerTypeIsInTypeCatalog()
        {
            mocker.GetMock<IDomainEventHandlerFactory>()
                .Setup(factory => factory.Create(It.IsAny<Type>()))
                .Returns((Type type) => Activator.CreateInstance(type));

            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleDomainEvents<>)))
                .Returns(new[] {typeof(MyTestEventHandler)});

            var eventBus = CreateLocalEventBus();
            var myTestEvent = new MyTestEvent();

            eventBus.PublishEvent(myTestEvent);

            Assert.AreEqual(101, myTestEvent.Result);
        }

        [TestMethod]
        public void DomainEventHandlerThatImplementsTwoHandlersAreCalledWhenHandlerTypeIsInTypeCatalog()
        {
            mocker.GetMock<IDomainEventHandlerFactory>()
                .Setup(factory => factory.Create(It.IsAny<Type>()))
                .Returns((Type type) => Activator.CreateInstance(type));

            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleDomainEvents<>)))
                .Returns(new[] {typeof(MyTest2EventHandler)});

            var eventBus = CreateLocalEventBus();
            var myTestEvent = new MyTestEvent();
            var myTest2Event = new MyTest2Event();

            eventBus.PublishEvents(new DomainEvent[] {myTestEvent, myTest2Event});

            Assert.AreEqual(102, myTestEvent.Result);
            Assert.AreEqual(45, myTest2Event.Result);
        }

        [TestMethod]
        public void AllEventHandlersAreCalledWhenHandlerTypesAreInTheTypeCatalog()
        {
            mocker.GetMock<IDomainEventHandlerFactory>()
                .Setup(factory => factory.Create(It.IsAny<Type>()))
                .Returns((Type type) => Activator.CreateInstance(type));

            mocker.GetMock<ITypeCatalog>()
                .Setup(typeCatalog => typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleDomainEvents<>)))
                .Returns(new[] {typeof(MyTestEventHandler), typeof(MyTest2EventHandler)});

            var eventBus = CreateLocalEventBus();
            var myTestEvent = new MyTestEvent();

            eventBus.PublishEvent(myTestEvent);

            Assert.IsTrue(myTestEvent.MyTestEventHandlerWasCalled);
            Assert.IsTrue(myTestEvent.MyTest2EventHandlerWasCalled);
        }

        private LocalEventBus CreateLocalEventBus()
        {
            var typeCatalog = mocker.Resolve<ITypeCatalog>();
            var factory = mocker.Resolve<IDomainEventHandlerFactory>();
            var eventHandlerTypes = typeCatalog.GetGenericInterfaceImplementations(typeof(IHandleDomainEvents<>));

            return new LocalEventBus(eventHandlerTypes, factory);
        }
    }

    public class MyTestEventHandler : IHandleDomainEvents<MyTestEvent>
    {
        public void Handle(MyTestEvent domainEvent)
        {
            domainEvent.Result = 101;
            domainEvent.MyTestEventHandlerWasCalled = true;
        }
    }

    public class MyTest2EventHandler : IHandleDomainEvents<MyTestEvent>, IHandleDomainEvents<MyTest2Event>
    {
        public void Handle(MyTest2Event domainEvent)
        {
            domainEvent.Result = 45;
        }

        public void Handle(MyTestEvent domainEvent)
        {
            domainEvent.Result = 102;
            domainEvent.MyTest2EventHandlerWasCalled = true;
        }
    }

    public class MyTestEvent : DomainEvent
    {
        public virtual int Result { get; set; }
        public bool MyTestEventHandlerWasCalled { get; set; }
        public bool MyTest2EventHandlerWasCalled { get; set; }
    }

    public class MyTest2Event : DomainEvent
    {
        public int Result { get; set; }
    }

    public class MockServiceLocator : IServiceLocator
    {
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

        public Func<Type, object> ResolveFunc { get; set; }

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