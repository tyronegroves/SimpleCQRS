using System;
using SimpleCqrs.Commanding;

namespace NerdDinner.Commands
{
    public class RsvpForDinnerCommand : CommandWithAggregateRootId
    {
        public Guid DinnerId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }

        public string AttendeeName { get; set; }
        public Guid AttendeeId { get; set; }
    }
}