namespace EventSourcingCQRS
{
    public interface IRegisterComponents
    {
        void Register(IServiceLocator serviceLocator);
    }
}