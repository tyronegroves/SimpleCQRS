namespace SimpleCqrs
{
    public static class ServiceLocator
    {
        public static IServiceLocator Current { get; private set; }
        
        public static void SetCurrent(IServiceLocator serviceLocator)
        {
            Current = serviceLocator;
        }
    }
}