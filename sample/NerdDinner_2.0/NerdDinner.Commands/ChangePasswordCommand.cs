using System;
using SimpleCqrs.Commanding;

namespace NerdDinner.Commands
{
    public class ChangePasswordCommand : CommandWithAggregateRootId
    {
        public Guid UserId
        {
            get { return AggregateRootId; }
            set { AggregateRootId = value; }
        }

        public string NewPassword { get; set; }
    }
}