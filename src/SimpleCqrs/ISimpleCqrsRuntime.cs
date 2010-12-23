using System;

namespace SimpleCqrs
{
    public interface ISimpleCqrsRuntime : IDisposable
    {
        IServiceLocator ServiceLocator { get; }
        void Start();
        void Shutdown();
    }
}