using TechTalk.SpecFlow;

namespace SimpleCqrs.EventStore.SqlServer.Tests.Steps
{
    [Binding]
    public class SqlServerConfigurationSteps
    {
        [Given(@"the connection string to my database is")]
        public void x(string connectionString)
        {
            var sqlServerConfiguration = new SqlServerConfiguration(connectionString);
            ScenarioContext.Current.Set(sqlServerConfiguration);

            DatabaseSteps.ClearTheEventStore();
        }
    }
}