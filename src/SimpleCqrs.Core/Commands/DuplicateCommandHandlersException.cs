using System;

namespace SimpleCqrs.Commands
{
    public class DuplicateCommandHandlersException : Exception
    {
        public DuplicateCommandHandlersException(Type commandType)
            : base(string.Format("Duplicate handlers were found for type {0}", commandType))
        {
        }
    }
}