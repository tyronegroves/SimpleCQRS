namespace EventSourcingCQRS.EventStore.CosmosDb
{
    public class CosmosDbConfiguration
    {
        private readonly string _connectionString;

        public CosmosDbConfiguration(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public string ConnectionString => _connectionString;
    }
}