using System;
using SimpleCqrs.Commanding;

namespace NerdDinner.Commands
{
    public class EditDinnerCommand : CommandWithAggregateRootId
    {
        public Guid DinnerId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }
        public DinnerHost Host { get; set; }
        public string ContactPhone { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Title { get; set; }
        public Location Location { get; set; }
    }
}