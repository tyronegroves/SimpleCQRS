namespace SimpleCqrs.EventStore.SqlServer
{
    public interface ISqlStatementRunner
    {
        void RunThisSql(string sqlStatement);
    }
}