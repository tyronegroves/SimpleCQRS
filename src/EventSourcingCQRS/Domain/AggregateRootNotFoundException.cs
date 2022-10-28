namespace EventSourcingCQRS.Domain
{
	public class AggregateRootNotFoundException : Exception
	{
		public Guid AggregateRootId { get; private set; }
		public Type Type { get; private set; }

		public AggregateRootNotFoundException(Guid aggregateRootId, Type type)
		{
			AggregateRootId = aggregateRootId;
			Type = type;
		}
	}
}