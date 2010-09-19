using System;
using SimpleCqrs.Commanding;

namespace NerdDinner.Commands
{
    public class CancelDinnerCommand : CommandWithAggregateRootId
    {
        public Guid DinnerId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }
    }
}