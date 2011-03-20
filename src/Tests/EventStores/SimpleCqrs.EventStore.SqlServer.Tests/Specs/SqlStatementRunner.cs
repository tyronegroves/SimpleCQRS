using Moq;
using TechTalk.SpecFlow;

namespace SimpleCqrs.EventStore.SqlServer.Tests.Specs
{
    [Binding]
    public class SqlStatementRunner
    {
        private Mock<ISqlStatementRunner> fakeSqlStatementRunner;

        [BeforeScenario]
        public void Setup()
        {
            fakeSqlStatementRunner = new Mock<ISqlStatementRunner>();
            RuntimeSteps.Runtime.ServiceLocator.Register(fakeSqlStatementRunner.Object);
        }

        [Then(@"the following SQL statement should be run")]
        public void x(string expectedSqlStatement)
        {
            fakeSqlStatementRunner.Verify(x => x.RunThisSql(expectedSqlStatement), Times.Once());
        }
    }
}