namespace EventSourcingCQRS.Domain
{
    public interface IDomainRepository
    {
        Task<TAggregateRoot> GetById<TAggregateRoot>(Guid aggregateRootId, CancellationToken cancellationToken)
            where TAggregateRoot : AggregateRoot, new();

        Task<TAggregateRoot> GetExistingById<TAggregateRoot>(Guid aggregateRootId, CancellationToken cancellationToken)
            where TAggregateRoot : AggregateRoot, new();

        Task<TAggregateRoot> GetExistingByIdUpToSequence<TAggregateRoot>(Guid aggregateRootId, int sequence,
            CancellationToken cancellationToken) where TAggregateRoot : AggregateRoot, new();

        Task Save(AggregateRoot aggregateRoot, CancellationToken cancellationToken);

        Task<Snapshot> GetSnapshotFromSnapshotStore(Guid aggregateRootId, CancellationToken cancellationToken);

        Task SaveSnapshotToSnapshotStore(Snapshot snapshot, CancellationToken cancellationToken);
    }
}