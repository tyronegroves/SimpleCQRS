using SimpleCqrs.Commanding;

namespace NerdDinner.Commands
{
    public class ChangePasswordCommand : CommandWithAggregateRootId
    {
        public string Name { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}