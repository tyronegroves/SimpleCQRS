namespace EventSourcingCQRS.EventStore.SqlServer
{
    public class SqlServerConfiguration
    {
        private readonly string _connectionString;

        public SqlServerConfiguration(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public string ConnectionString => _connectionString;
    }
}