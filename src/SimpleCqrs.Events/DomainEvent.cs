namespace SimpleCqrs.Events
{
    public class DomainEvent : IDomainEvent
    {
        public int EventId { get; set; }
    }
}