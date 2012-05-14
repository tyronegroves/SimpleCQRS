using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Ninject.Tests.Integration
{
    [TestClass]
    public class NinjectServiceLocatorIntegrationTests
    {
        [TestMethod]
        public void can_create_a_new_instance_of_the_Simple_CQRS_Runtime()
        {
            var sut = new MySimpleCqrsRuntime();
            sut.Should().NotBeNull();
        }

        [TestMethod]
        public void can_start_and_stop_a_new_instance_of_MySimpleRuntime()
        {
            var sut = new MySimpleCqrsRuntime();

            sut.Invoking(x => sut.Start()).ShouldNotThrow();
            sut.Invoking(x => x.Shutdown()).ShouldNotThrow();
        }

        [TestMethod]
        public void can_send_a_simple_command_and_firing_domain_events_happy_path()
        {
            using (var runtime = new MySimpleCqrsRuntimeHelper().Build())
            {
                var command = new CreateAccountCommand { FirstName = "Juan", LastName = "Olmos" };
                var commandBus = runtime.ServiceLocator.Resolve<ICommandBus>();

                commandBus.Send(command);

                CreateAccountCommandHandlerWasCalled.Should().BeTrue();
                CreateAccountInternalEventHandlerWasCalled.Should().BeTrue();
                NameSetInternalEventHandlerWasCalled.Should().BeTrue();
                CreateAccountEventHandlerWasCalled.Should().BeTrue();
                NameSetEventHandlerWasCalled.Should().BeTrue();
            }
        }

        internal static bool CreateAccountCommandHandlerWasCalled = false;
        internal static bool CreateAccountInternalEventHandlerWasCalled = false;
        internal static bool NameSetInternalEventHandlerWasCalled = false;
        internal static bool CreateAccountEventHandlerWasCalled = false;
        internal static bool NameSetEventHandlerWasCalled = false;
    }

    class MySimpleCqrsRuntimeHelper : IDisposable
    {
        public MySimpleCqrsRuntime Runtime { get; private set; }

        public MySimpleCqrsRuntimeHelper()
        {
            this.Runtime = new MySimpleCqrsRuntime();
            this.Runtime.Start();
        }

        public void Dispose()
        {
            this.Runtime.Shutdown();
            this.Runtime.Dispose();
            this.Dispose();
        }

        public MySimpleCqrsRuntime Build()
        {
            return this.Runtime;
        }
    }

    class MySimpleCqrsRuntime : SimpleCqrsRuntime<NinjectServiceLocator>
    {
    }

    class CreateAccountCommand : ICommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    class CreateAccountCommandHandler : CommandHandler<CreateAccountCommand>
    {
        private IDomainRepository repository;

        public CreateAccountCommandHandler(IDomainRepository repository)
        {
            this.repository = repository;
        }

        public override void Handle(CreateAccountCommand command)
        {
            NinjectServiceLocatorIntegrationTests.CreateAccountCommandHandlerWasCalled = true;

            command.Should().NotBeNull();
            command.FirstName.Should().Be("Juan");
            command.LastName.Should().Be("Olmos");

            var account = new Account(Guid.NewGuid());

            account.SetName(command.FirstName, command.LastName);

            this.repository.Save(account);
        }
    }

    class Account : AggregateRoot
    {
        public Account(Guid id)
        {
            this.Apply(new AccountCreatedEvent(id));
        }

        public void SetName(string firstName, string lastName)
        {
            this.Apply(new AccountNameSetEvent(firstName, lastName));
        }

        public void OnAccountCreated(AccountCreatedEvent e)
        {
            NinjectServiceLocatorIntegrationTests.CreateAccountInternalEventHandlerWasCalled = true;

            e.Should().NotBeNull();
            e.Id.Should().NotBeEmpty();
        }

        public void OnAccountNameSet(AccountNameSetEvent e)
        {
            NinjectServiceLocatorIntegrationTests.NameSetInternalEventHandlerWasCalled = true;

            e.Should().NotBeNull();
            e.FirstName.Should().Be("Juan");
            e.LastName.Should().Be("Olmos");
        }
    }

    class AccountNameSetEvent : DomainEvent
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public AccountNameSetEvent(string firstName, string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }
    }

    class AccountCreatedEvent : DomainEvent
    {
        public Guid Id { get; private set; }

        public AccountCreatedEvent(Guid id)
        {
            this.Id = id;
        }
    }

    class AccountReportDenormalized : IHandleDomainEvents<AccountCreatedEvent>, IHandleDomainEvents<AccountNameSetEvent>
    {

        public void Handle(AccountNameSetEvent domainEvent)
        {
            NinjectServiceLocatorIntegrationTests.NameSetEventHandlerWasCalled = true;
        }

        public void Handle(AccountCreatedEvent domainEvent)
        {
            NinjectServiceLocatorIntegrationTests.CreateAccountEventHandlerWasCalled = true;
        }
    }
}
