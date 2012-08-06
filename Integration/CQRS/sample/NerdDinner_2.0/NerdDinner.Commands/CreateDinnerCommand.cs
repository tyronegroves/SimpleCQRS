using System;
using SimpleCqrs.Commanding;

namespace NerdDinner.Commands
{
    public class CreateDinnerCommand : ICommand
    {
        public Guid DinnerId { get; set; }
        public DinnerHost Host { get; set; }
        public string ContactPhone { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Title { get; set; }
        public Location Location { get; set; }
    }
}