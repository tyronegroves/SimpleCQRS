namespace SimpleCqrs
{
    public static class ServiceLocator
    {
        public static IServiceLocator Current { get; private set; }
        
        internal static void SetCurrent(IServiceLocator serviceLocator)
        {
            Current = serviceLocator;
        }
    }
}