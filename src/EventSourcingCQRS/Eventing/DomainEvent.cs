namespace EventSourcingCQRS.Eventing
{
    [Serializable]
    public class DomainEvent
    {
        public Guid AggregateRootId { get; set; }
        public int Sequence { get; set; }
        public DateTime EventDate { get; set; }
    }
}