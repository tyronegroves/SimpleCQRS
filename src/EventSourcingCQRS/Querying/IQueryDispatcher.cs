namespace EventSourcingCQRS.Querying
{
    internal interface IQueryDispatcher
    {
        Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellation);
    }
}