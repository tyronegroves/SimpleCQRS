using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NerdDinner.Helpers;

namespace NerdDinner.Tests.Helpers
{
    [TestClass]
    public class FileNotFoundResultTests
    {
        [TestMethod]
        public void ExecuteResult_Throws_HttpException_With_404_Status() {
            // Arrange
            ControllerContext context = new ControllerContext(); 
            FileNotFoundResult result = new FileNotFoundResult();

            // Act
            var thrownException = UnitTestHelper.AssertThrows<HttpException>(() => result.ExecuteResult(context));
            
            // Assert
            Assert.AreEqual(404, thrownException.GetHttpCode());
        }
    }
}
