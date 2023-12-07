namespace EventSourcingCQRS.EventStore.CosmosDb.UnitTests
{
    [TestClass]
    public class CosmosDbConnectionStringBuilderTests
    {
        [TestMethod]
        public void ParseConnectionString_When_DatabaseName_Is_Present_Then_Returns_Value()
        {
            //Arrange
            var connectionString = "DatabaseName=test";

            //Act
            var uut = CosmosDbConnectionStringBuilder.ParseConnectionString(connectionString);

            //Assert
            Assert.AreEqual("test", uut.DatabaseName);
        }

        [TestMethod]
        public void ParseConnectionString_When_AccountKey_Is_Present_Then_Returns_Value()
        {
            //Arrange
            var connectionString = "AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

            //Act
            var uut = CosmosDbConnectionStringBuilder.ParseConnectionString(connectionString);

            //Assert
            Assert.AreEqual("C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", uut.AccountKey);
        }
    }
}