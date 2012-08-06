using Microsoft.VisualStudio.TestTools.UnitTesting;
using NerdDinner.Helpers;

namespace NerdDinner.Tests.Helpers
{
    [TestClass]
    public class PhoneValidatorTest
    {
        [TestMethod]
        public void Validator_Should_Return_True_For_ValidNumber_In_USA()
        {
            Assert.IsTrue(PhoneValidator.IsValidNumber("425-555-1212", "USA"));    
        }

        [TestMethod]
        public void Validator_Should_Return_False_For_InValidNumber_In_USA()
        {
            Assert.IsFalse(PhoneValidator.IsValidNumber("555-123456", "USA"));
        }

        [TestMethod]
        public void Validator_Should_Return_True_For_ValidNumber_In_Canada()
        {
            Assert.IsTrue(PhoneValidator.IsValidNumber("403-555-1212", "Canada"));
        }

        [TestMethod]
        public void Validator_Should_Return_False_For_InValidNumber_In_Canada()
        {
            Assert.IsFalse(PhoneValidator.IsValidNumber("555-123456", "Canada"));
        }

        [TestMethod]
        public void Validator_Should_Return_True_For_ValidNumber_In_Mexico()
        {
            Assert.IsTrue(PhoneValidator.IsValidNumber("875-489-1568", "Mexico"));
        }

        [TestMethod]
        public void Validator_Should_Return_False_For_InValidNumber_In_Mexico()
        {
            Assert.IsFalse(PhoneValidator.IsValidNumber("875 (489 1568)", "Mexico"));
        }

        [TestMethod]
        public void Validator_Should_Return_True_For_ValidNumber_In_Peru()
        {
            Assert.IsTrue(PhoneValidator.IsValidNumber("5660578 1235", "Peru"));
        }

        [TestMethod]
        public void Validator_Should_Return_False_For_InValidNumber_In_Peru()
        {
            Assert.IsFalse(PhoneValidator.IsValidNumber("4224586 50124", "Peru"));
        }

        [TestMethod]
        public void Validator_Should_Return_True_For_ValidNumber_In_Australia()
        {
            Assert.IsTrue(PhoneValidator.IsValidNumber("02-9323-1234", "Australia"));
        }

        [TestMethod]
        public void Validator_Should_Return_False_For_InValidNumber_In_Australia()
        {
            Assert.IsFalse(PhoneValidator.IsValidNumber("02 932 123", "Australia"));
        }

        [TestMethod]
        public void Validator_Should_Return_True_For_ValidNumber_In_Brazil()
        {
            Assert.IsTrue(PhoneValidator.IsValidNumber("2323-6699", "Brazil"));
        }

        [TestMethod]
        public void Validator_Should_Return_False_For_InValidNumber_In_Brazil()
        {
            Assert.IsFalse(PhoneValidator.IsValidNumber("55(2)232-232", "Brazil"));
        }

        [TestMethod]
        public void Validator_Should_Return_True_For_ValidNumber_In_Netherlands()
        {
            Assert.IsTrue(PhoneValidator.IsValidNumber("0111-101234", "Netherlands"));
        }

        [TestMethod]
        public void Validator_Should_Return_False_For_InValidNumber_In_Netherlands()
        {
            Assert.IsFalse(PhoneValidator.IsValidNumber("123-4567890", "Netherlands"));
        }

        [TestMethod]
        public void Validator_Should_Return_True_For_ValidNumber_In_Italy()
        {
            Assert.IsTrue(PhoneValidator.IsValidNumber("3381234567", "Italy"));
        }

        [TestMethod]
        public void Validator_Should_Return_False_For_InValidNumber_In_Italy()
        {
            Assert.IsFalse(PhoneValidator.IsValidNumber("338-1234567", "Italy"));
        }

        [TestMethod]
        public void Validator_Should_Return_True_For_ValidNumber_In_SouthAfrica()
        {
            Assert.IsTrue(PhoneValidator.IsValidNumber("0832762842", "South Africa"));
        }

        [TestMethod]
        public void Validator_Should_Return_False_For_InValidNumber_In_SouthAfrica()
        {
            Assert.IsFalse(PhoneValidator.IsValidNumber("083 276 2842", "South Africa"));
        }

        [TestMethod]
        public void Validator_Should_Return_True_For_ValidNumber_In_India()
        {
            Assert.IsTrue(PhoneValidator.IsValidNumber("0493 3227341", "India"));
        }

        [TestMethod]
        public void Validator_Should_Return_False_For_InValidNumber_In_India()
        {
            Assert.IsFalse(PhoneValidator.IsValidNumber("493 322734111", "India"));
        }

        [TestMethod]
        public void Validator_Should_Return_True_For_ValidNumber_In_Spain()
        {
            Assert.IsTrue(PhoneValidator.IsValidNumber("972-377086", "Spain"));
        }

        [TestMethod]
        public void Validator_Should_Return_False_For_InValidNumber_In_Spain()
        {
            Assert.IsFalse(PhoneValidator.IsValidNumber("9988-989898", "Spain"));
        }

        [TestMethod]
        public void Validator_Should_Return_True_For_ValidNumber_In_UK()
        {
            Assert.IsTrue(PhoneValidator.IsValidNumber("01222 555 555", "UK"));
        }

        [TestMethod]
        public void Validator_Should_Return_False_For_InValidNumber_In_UK()
        {
            Assert.IsFalse(PhoneValidator.IsValidNumber("01222 555 5555", "UK"));
        }

        [TestMethod]
        public void Validator_Should_Return_True_For_ValidNumber_In_Sweden()
        {
            Assert.IsTrue(PhoneValidator.IsValidNumber("+46 8 123 456 78", "Sweden"));
        }

        [TestMethod]
        public void Validator_Should_Return_False_For_InValidNumber_In_Sweden()
        {
            Assert.IsFalse(PhoneValidator.IsValidNumber("+46 08-123 456 78", "Sweden"));
        }

    }
}
