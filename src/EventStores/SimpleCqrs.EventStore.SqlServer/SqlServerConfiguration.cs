namespace SimpleCqrs.EventStore.SqlServer
{
    public class SqlServerConfiguration
    {
        private readonly string connectionString;

        public SqlServerConfiguration(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public string ConnectionString
        {
            get { return connectionString; }
        }
    }
}