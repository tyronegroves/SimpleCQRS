using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleCqrs.Eventing;

namespace SimpleCqrs.Core.Tests.Events
{
    [TestClass]
    public class SimpleCqrsInvalidEventNameExceptionTests
    {
        [TestMethod]
        public void The_default_message_on_the_exception_is_a_tip_about_naming()
        {
            var exception = new SimpleCqrsInvalidEventNameException();
            Assert.AreEqual("Your events must end with the word Event.", exception.Message);
        }
    }
}