namespace SimpleCqrs.Events
{
    public interface IDomainEvent
    {
        int EventId { get; set; }
    }
}