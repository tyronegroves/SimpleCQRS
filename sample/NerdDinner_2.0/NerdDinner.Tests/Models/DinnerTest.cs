using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NerdDinner.Models;

namespace NerdDinner.Tests.Models {

    [TestClass]
    public class DinnerTest {

        [TestMethod]
        public void Dinner_Should_Not_Be_Valid_When_Some_Properties_Incorrect() {

            //Arrange
            Dinner dinner = new Dinner() {
                Title = "Test title",
                Country = "USA",
                ContactPhone = "BOGUS"
            };

            // Act
            bool isValid = dinner.IsValid;

            //Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void Dinner_Should_Be_Valid_When_All_Properties_Correct() {
            
            //Arrange
            Dinner dinner = new Dinner {
                Title = "Test title",
                Description = "Some description",
                EventDate = DateTime.Now,
                HostedBy = "ScottGu",
                Address = "One Microsoft Way",
                Country = "USA",
                ContactPhone = "425-703-8072",
                Latitude = 93,
                Longitude = -92,
            };

            // Act
            bool isValid = dinner.IsValid;

            //Assert
            Assert.IsTrue(isValid);
        }
    }
}
