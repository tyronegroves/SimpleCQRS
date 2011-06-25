using System;

namespace SimpleCqrs.Eventing
{
    public class SimpleCqrsInvalidEventNameException : Exception
    {
        public SimpleCqrsInvalidEventNameException()
            : base("Your events must end with the word Event.")
        {
        }
    }
}