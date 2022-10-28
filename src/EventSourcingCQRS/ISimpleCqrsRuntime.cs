namespace EventSourcingCQRS
{
    /// <summary>
    /// Defines a runtime environment for a <b>SimpleCqrs</b> applications.
    /// </summary>
    public interface ISimpleCqrsRuntime : IDisposable
    {
        /// <summary>
        /// Gets the service locator associated with the runtime.
        /// </summary>
        IServiceLocator ServiceLocator { get; }

        /// <summary>
        /// Starts the runtime environment.
        /// </summary>
        void Start();

        /// <summary>
        /// Shutdowns the runtime environment and release all the resouces held by the runtime environment.
        /// </summary>
        void Shutdown();
    }
}