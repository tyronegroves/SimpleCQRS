using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NerdDinner.Controllers;
using System.Web.Mvc;
using NerdDinner.Models;
using NerdDinner.Tests.Fakes;
using Moq;
using NerdDinner.Helpers;
using System.Web.Routing;

namespace NerdDinner.Tests.Controllers {
 
    [TestClass]
    public class RSVPControllerTest {

        RSVPController CreateRSVPController() {
            var testData = FakeDinnerData.CreateTestDinners();
            var repository = new FakeDinnerRepository(testData);

            return new RSVPController(repository);
        }

        RSVPController CreateRSVPControllerAs(string userName)
        {

            var mock = new Mock<ControllerContext>();
            var nerdIdentity = FakeIdentity.CreateIdentity("SomeUser");
            mock.SetupGet(p => p.HttpContext.User.Identity).Returns(nerdIdentity);

            var controller = CreateRSVPController();
            controller.ControllerContext = mock.Object;

            return controller;
        }

        [TestMethod]
        public void RegisterAction_Should_Return_Content()
        {
            // Arrange
            var controller = CreateRSVPControllerAs("scottha");

            // Act
            var result = controller.Register(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ContentResult));
        }
    }
}
