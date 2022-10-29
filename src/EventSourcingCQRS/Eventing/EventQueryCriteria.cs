namespace EventSourcingCQRS.Eventing
{
    public class EventQueryCriteria
    {
        public string Name { get; set; }
        public Type Type { get; protected set; }
    }

    public class EventQueryCriteria<T> : EventQueryCriteria
    {
        public EventQueryCriteria()
        {
            Type = typeof(T);
        }
        public T Value { get; set; }

    }
}