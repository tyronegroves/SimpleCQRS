namespace EventSourcingCQRS.Querying
{
    public interface IQueryHandler<in TQuery, TQueryResult>
    {
        Task<TQueryResult> Handle(TQuery query, CancellationToken cancellation);
    }
}