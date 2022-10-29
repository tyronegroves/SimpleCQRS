namespace EventSourcingCQRS.Domain
{
    public class AggregateRootNotFoundException : Exception
    {
        public AggregateRootNotFoundException()
        {

        }
        public AggregateRootNotFoundException(Guid aggregateRootId, Type type)
        {
            AggregateRootId = aggregateRootId;
            Type = type;
        }

        public Guid AggregateRootId { get; }
        public Type Type { get; }
    }
}