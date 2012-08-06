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
    public class SearchControllerTest {

        SearchController CreateSearchController() {
            var testData = FakeDinnerData.CreateTestDinners();
            var repository = new FakeDinnerRepository(testData);

            return new SearchController(repository);
        }

        [TestMethod]
        public void SearchByLocationAction_Should_Return_Json()
        {

            // Arrange
            var controller = CreateSearchController();

            // Act
            var result = controller.SearchByLocation(99, -99);

            // Assert
            Assert.IsInstanceOfType(result, typeof(JsonResult));
        }

        [TestMethod]
        public void SearchByLocationAction_Should_Return_JsonDinners()
        {

            // Arrange
            var controller = CreateSearchController();

            // Act
            var result = (JsonResult)controller.SearchByLocation(99, -99);

            // Assert
            Assert.IsInstanceOfType(result.Data, typeof(List<JsonDinner>));
            var dinners = (List<JsonDinner>)result.Data;
            Assert.AreEqual(101, dinners.Count);
        }

        [TestMethod]
        public void GetMostPopularDinnersAction_WithLimit_Returns_Expected_Dinners()
        {

            // Arrange
            var controller = CreateSearchController();

            // Act
            var result = (JsonResult)controller.GetMostPopularDinners(5);

            // Assert
            Assert.IsInstanceOfType(result.Data, typeof(List<JsonDinner>));
            var dinners = (List<JsonDinner>)result.Data;
            Assert.AreEqual(5, dinners.Count);
        }

        [TestMethod]
        public void GetMostPopularDinnersAction_WithNoLimit_Returns_Expected_Dinners()
        {

            // Arrange
            var controller = CreateSearchController();

            // Act
            var result = (JsonResult)controller.GetMostPopularDinners(null);

            // Assert
            Assert.IsInstanceOfType(result.Data, typeof(List<JsonDinner>));
            var dinners = (List<JsonDinner>)result.Data;
            Assert.AreEqual(40, dinners.Count);
        }


    }
}
