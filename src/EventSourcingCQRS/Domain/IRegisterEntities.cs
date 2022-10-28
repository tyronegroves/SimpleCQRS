namespace EventSourcingCQRS.Domain
{
    public interface IRegisterEntities
    {
        void RegisterEntity(Entity entity);
    }
}