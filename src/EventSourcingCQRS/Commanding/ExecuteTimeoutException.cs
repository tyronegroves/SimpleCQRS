using System.Runtime.Serialization;

namespace EventSourcingCQRS.Commanding
{
    [Serializable]
    public class ExecuteTimeoutException : Exception
    {
        public ExecuteTimeoutException()
            : base("The allotted time for the ICommandBus.Execute method has expired.")
        {
        }

        public ExecuteTimeoutException(string message) : base(message)
        {
        }

        public ExecuteTimeoutException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ExecuteTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}