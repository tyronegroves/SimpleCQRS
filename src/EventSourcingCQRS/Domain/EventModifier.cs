using EventSourcingCQRS.Eventing;

namespace EventSourcingCQRS.Domain
{
    public static class EventModifier
    {
        public static IEventModification Modification;
 
        public static void Modify(DomainEvent e)
        {
            if (Modification != null)
                Modification.Apply(e);
        }
    }
}