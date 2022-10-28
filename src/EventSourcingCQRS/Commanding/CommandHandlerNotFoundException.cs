namespace EventSourcingCQRS.Commanding
{
    public class CommandHandlerNotFoundException : Exception
    {
        public CommandHandlerNotFoundException(Type commandType)
            : base(string.Format("No command handlers were found for '{0}'", commandType))
        {
        }
    }
}