using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using SimpleCqrs.Ninject;

namespace SimpleCqrs.Core.Tests.ServiceLocators
{
    [TestClass]
    public class NinjectServiceLocatorTests
    {
        [TestMethod]
        public void can_create_new_default_instance()
        {
            var sut = new NinjectServiceLocator();

            sut.Should().NotBeNull();
        }

        [TestMethod]
        public void can_initialize_a_new_instance_with_a_existing_kernel()
        {
            IKernel kernel = new StandardKernel();
            var sut = new NinjectServiceLocator(kernel);

            sut.Should().NotBeNull();
        }

        [TestMethod]
        public void when_initializing_a_new_instance_with_a_null_kernel_it_should_throw_an_argument_null_exception()
        {
            NinjectServiceLocator sut = null;
            Action initializing = () => sut = new NinjectServiceLocator(null);
            initializing.ShouldThrow<ArgumentNullException>()
                .WithMessage("The specified Ninject kernel cannot be null", ComparisonMode.Substring)
                .WithMessage("kernel", ComparisonMode.Substring);
        }

        [TestClass]
        public class TheResolveMethod
        {
            [TestMethod]
            public void it_should_resolve_the_type_registered_with_the_generic_contract_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.NinjectServiceLocator.Register<IMyTestingContract, MyImplementation>();
                    helper.NinjectServiceLocator.Resolve<IMyTestingContract>().Should().BeOfType<MyImplementation>();
                }
            }

            [TestMethod]
            public void it_should_resolve_the_type_registered_with_the_generic_contract_specified_and_with_the_key_specifdied()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.NinjectServiceLocator.Register<IMyTestingContract, MyImplementation>("my key");
                    helper.NinjectServiceLocator.Resolve<IMyTestingContract>("my key").Should().NotBeNull().And.BeOfType<MyImplementation>();
                }
            }

            [TestMethod]
            public void it_should_resolve_the_type_registered_for_the_type_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.NinjectServiceLocator.Register<IMyTestingContract, MyImplementation>();
                    helper.NinjectServiceLocator.Resolve(typeof(MyImplementation)).Should().NotBeNull().And.BeOfType<MyImplementation>();
                }
            }
        }

        [TestClass]
        public class TheResolveServicesMethod
        {
            [TestMethod]
            public void it_should_return_an_empty_enumerable_when_there_are_not_types_registered_for_the_generic_type_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.NinjectServiceLocator.ResolveServices<IMyTestingContract>().Should().NotBeNull().And.HaveCount(0);
                }
            }

            [TestMethod]
            public void it_should_resolve_all_the_types_registered_for_the_generic_contract_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.NinjectServiceLocator.Register<IMyTestingContract, MyImplementation>();
                    helper.NinjectServiceLocator.Register<IMyTestingContract, MyImplementation3>();
                    helper.NinjectServiceLocator.Register<IMyTestingContract, MyImplementation2>();
                    helper.NinjectServiceLocator.ResolveServices<IMyTestingContract>().Should()
                        .NotBeNull().And.HaveCount(3).And.ContainItemsAssignableTo<IMyTestingContract>()
                        .And.OnlyHaveUniqueItems()
                        .And.Match(x => x.OfType<MyImplementation>().Count() == 1)
                        .And.Match(x => x.OfType<MyImplementation2>().Count() == 1)
                        .And.Match(x => x.OfType<MyImplementation3>().Count() == 1);
                }
            }
        }

        [TestClass]
        public class TheRegisteMethod
        {
            [TestMethod]
            public void it_should_throw_an_ArgumentNullException_when_the_register_type_specified_is_null_for_the_generic_contract_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.ValidateType(helper.NinjectServiceLocator.Invoking(x => x.Register<IMyTestingContract>((Type)null)), "implType");
                }
            }

            [TestMethod]
            public void it_should_register_the_type_specified_with_the_generic_contract_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.NinjectServiceLocator.Register<IMyTestingContract>(typeof(MyImplementation));
                    helper.Kernel.Get<IMyTestingContract>().Should().BeOfType<MyImplementation>();
                }
            }

            [TestMethod]
            public void it_should_register_the_generic_type_specified_with_the_generic_contract_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.NinjectServiceLocator.Register<IMyTestingContract, MyImplementation>();
                    helper.Kernel.Get<IMyTestingContract>().Should().BeOfType<MyImplementation>();
                }
            }

            [TestMethod]
            public void it_should_throw_an_ArgumentNullException_when_the_key_specified_is_null_for_the_generic_contract_and_the_generic_type_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.ValidateKey(helper.NinjectServiceLocator.Invoking(x => x.Register<IMyTestingContract, MyImplementation>(null)));
                    helper.ValidateKey(helper.NinjectServiceLocator.Invoking(x => x.Register<IMyTestingContract, MyImplementation>(string.Empty)));
                    helper.ValidateKey(helper.NinjectServiceLocator.Invoking(x => x.Register<IMyTestingContract, MyImplementation>("             ")));
                }
            }

            [TestMethod]
            public void it_should_rgeister_the_generic_type_with_the_generic_contract_and_the_key_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.NinjectServiceLocator.Register<IMyTestingContract, MyImplementation>("my key");
                    helper.Kernel.Get<IMyTestingContract>("my key").Should().BeOfType<MyImplementation>();
                }
            }

            [TestMethod]
            public void it_should_throw_an_ArgumentNullException_when_the_key_is_null_for_the_type_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.ValidateKey(helper.NinjectServiceLocator.Invoking(x => x.Register((string)null, typeof(MyImplementation))));
                    helper.ValidateKey(helper.NinjectServiceLocator.Invoking(x => x.Register(string.Empty, typeof(MyImplementation))));
                    helper.ValidateKey(helper.NinjectServiceLocator.Invoking(x => x.Register("   ", typeof(MyImplementation))));
                }
            }

            [TestMethod]
            public void it_should_throw_an_ArgumentNullException_when_the_type_is_null_for_the_key_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.ValidateType(helper.NinjectServiceLocator.Invoking(x => x.Register("my key", (Type)null)), "implType");
                }
            }

            [TestMethod]
            public void it_should_register_the_specified_type_with_the_key_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.NinjectServiceLocator.Register("my key", typeof(MyImplementation));
                    helper.Kernel.Get<MyImplementation>("my key").Should().NotBeNull().And.BeOfType<MyImplementation>();
                }
            }

            [TestMethod]
            public void it_should_throw_an_ArgumentNullException_when_the_contract_type_is_null_for_the_type_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.ValidateType(helper.NinjectServiceLocator.Invoking(x => x.Register((Type)null, typeof(MyImplementation))), "serviceType");
                }
            }

            [TestMethod]
            public void it_should_throw_an_ArgumentNullException_when_the_type_is_null_for_the_contract_type_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.ValidateType(helper.NinjectServiceLocator.Invoking(x => x.Register(typeof(IMyTestingContract), (Type)null)), "implType");
                }
            }

            [TestMethod]
            public void it_should_register_the_type_specified_with_the_contract_type_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.NinjectServiceLocator.Register(typeof(IMyTestingContract), typeof(MyImplementation));
                    helper.Kernel.Get<IMyTestingContract>().Should().BeOfType<MyImplementation>();
                }
            }

            [TestMethod]
            public void it_should_throw_an_ArgumentNullException_when_the_instance_is_null_for_the_generic_type_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.ValidateInstance(helper.NinjectServiceLocator.Invoking(x => x.Register<IMyTestingContract>((IMyTestingContract)null)), "instance");
                }
            }

            [TestMethod]
            public void it_should_register_the_instance_specified_with_the_generic_contract_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    var myType = new MyImplementation();
                    helper.NinjectServiceLocator.Register<IMyTestingContract>(myType);
                    helper.Kernel.Get<IMyTestingContract>().Should().BeOfType<MyImplementation>().And.Be(myType);
                }
            }

            [TestMethod]
            public void it_should_throw_an_ArgumentNullException_when_the_delegate_is_null_for_the_generic_contract_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.ValidaeFuncDelegate(helper.NinjectServiceLocator.Invoking(x => x.Register<IMyTestingContract>((Func<IMyTestingContract>)null)), "factoryMethod");
                }
            }

            [TestMethod]
            public void it_should_throw_an_ArgumentNullException_when_the_returned_object_from_the_Func_Delegate_is_null_for_the_generic_contract_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.ValidateType(helper.NinjectServiceLocator.Invoking(x => x.Register<IMyTestingContract>(() => (IMyTestingContract)null)), "factoryMethod");
                }
            }

            [TestMethod]
            public void it_should_register_the_instance_returned_from_the_Func_delegate_with_the_generic_contract_specified()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    var myType = new MyImplementation();
                    helper.NinjectServiceLocator.Register<IMyTestingContract>(() => myType);
                    helper.Kernel.Get<IMyTestingContract>().Should().BeOfType<MyImplementation>().And.Be(myType);
                }
            }
        }

        [TestClass]
        public class TheReleaseMethod
        {
            [TestMethod]
            public void when_the_instance_is_null_it_should_not_throw_any_exceptions()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.NinjectServiceLocator.Invoking(x => x.Release(null)).ShouldNotThrow();
                }
            }

            [TestMethod]
            public void it_should_release_the_specified_object()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    var myObject = new MyImplementation();

                    helper.NinjectServiceLocator.Invoking(x => x.Release(myObject));

                    helper.NinjectServiceLocator.Register<IMyTestingContract>(myObject);
                    helper.NinjectServiceLocator.Invoking(x => x.Release(myObject))
                        .ShouldNotThrow();
                }
            }
        }

        [TestClass]
        public class TheTearDownMethod
        {
            [TestMethod]
            public void when_the_instance_is_null_it_should_not_throw_any_exceptions()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.NinjectServiceLocator.Invoking(x => x.TearDown<IMyTestingContract>(null)).ShouldNotThrow();
                }
            }

            [TestMethod]
            public void it_should_tear_down_the_specified_instance()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    var myObject = new MyImplementation();

                    helper.NinjectServiceLocator.Invoking(x => x.Release(myObject));

                    helper.NinjectServiceLocator.Register<IMyTestingContract>(myObject);
                    helper.NinjectServiceLocator.Invoking(x => x.TearDown(myObject))
                        .ShouldNotThrow();
                }
            }
        }

        [TestClass]
        public class TheResetMethod
        {
            [TestMethod]
            public void it_should_throw_a_NotImplementedException_because_Ninject_does_not_support_reseting_the_container()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.NinjectServiceLocator.Invoking(x => x.Reset()).ShouldThrow<NotImplementedException>();
                }
            }
        }

        [TestClass]
        public class TheDisposeMethod
        {
            [TestMethod]
            public void it_should_dispose_the_Ninject_kernel_and_the_service_locator_container()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.Kernel.Should().NotBeNull();
                    helper.NinjectServiceLocator.Register<IMyTestingContract, MyImplementation>();
                    helper.NinjectServiceLocator.IsDisposed.Should().BeFalse();
                    helper.Kernel.IsDisposed.Should().BeFalse();
                    helper.NinjectServiceLocator.Dispose();
                    GC.Collect();
                    helper.NinjectServiceLocator.IsDisposed.Should().BeTrue();
                    helper.Kernel.IsDisposed.Should().BeTrue();
                }
            }
        }

        [TestClass]
        public class TheInjectMethod
        {
            [TestMethod]
            public void it_should_throw_an_ArgumentNullException_when_the_instance_to_be_injected_is_null()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.ValidateInstance(helper.NinjectServiceLocator.Invoking(x => x.Inject<IMyTestingContract>(null)), "instance");
                }
            }

            [TestMethod]
            public void it_should_inject_an_existing_instance()
            {
                using (var helper = new NinjectServiceLocatorHelper())
                {
                    helper.NinjectServiceLocator.Register<Additionaltype>(new Additionaltype());
                    var myObject = new MyImplementation();
                    myObject = helper.NinjectServiceLocator.Inject(myObject);
                    myObject.Should().NotBeNull().And.BeOfType<MyImplementation>();
                    myObject.AdditionalType.Should().NotBeNull().And.BeOfType<Additionaltype>();
                }
            }
        }
    }

    class NinjectServiceLocatorHelper : IDisposable
    {
        public IKernel Kernel { get; private set; }
        public NinjectServiceLocator NinjectServiceLocator { get; private set; }

        public NinjectServiceLocatorHelper()
        {
            this.Kernel = new StandardKernel();
            this.Kernel.Settings.InjectNonPublic = true;
            this.NinjectServiceLocator = new NinjectServiceLocator(this.Kernel);
        }

        public void Dispose()
        {
            this.NinjectServiceLocator.Dispose();
        }

        public void ValidateKey(Action action)
        {
            action.ShouldThrow<ArgumentNullException>()
                .WithMessage("The key cannot be null, empty or a string with white spaces only", ComparisonMode.Substring)
                .WithMessage("key", ComparisonMode.Substring);
        }

        public void ValidateType(Action action, string argumentName)
        {
            action.ShouldThrow<ArgumentNullException>()
                .WithMessage("The container does not accept null types to be registered", ComparisonMode.Substring)
                .WithMessage(argumentName, ComparisonMode.Substring);
        }

        public void ValidateInstance(Action action, string argumentName)
        {
            action.ShouldThrow<ArgumentNullException>()
                .WithMessage("Null objects cannot be registered in the container", ComparisonMode.Substring)
                .WithMessage(argumentName, ComparisonMode.Substring);
        }

        public void ValidaeFuncDelegate(Action action, string argumentName)
        {
            action.ShouldThrow<ArgumentNullException>()
                .WithMessage("The calling delegate connot be null", ComparisonMode.Substring)
                .WithMessage(argumentName, ComparisonMode.Substring);
        }
    }

    interface IMyTestingContract
    {
        int Add(int num1, int num2);
    }

    class Additionaltype
    {
    }

    class MyImplementation : IMyTestingContract
    {
        [Inject]
        public Additionaltype AdditionalType { get; private set; }

        public int Add(int num1, int num2)
        {
            throw new NotImplementedException();
        }
    }

    class MyImplementation2 : IMyTestingContract
    {
        public int Add(int num1, int num2)
        {
            throw new NotImplementedException();
        }
    }

    class MyImplementation3 : IMyTestingContract
    {
        public int Add(int num1, int num2)
        {
            throw new NotImplementedException();
        }
    }
}
