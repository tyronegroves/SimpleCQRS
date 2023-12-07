namespace EventSourcingCQRS.Querying
{
    public interface IQueryDispatcher
    {
        Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellation);
    }
}