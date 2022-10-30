namespace EventSourcingCQRS.Querying
{
    internal interface IQueryHandler<in TQuery, TQueryResult>
    {
        Task<TQueryResult> Handle(TQuery query, CancellationToken cancellation);
    }
}