using NerdDinner.Commands;
using NerdDinner.CommandService.Domain;
using NerdDinner.CommandService.Services;
using SimpleCqrs.Commanding;

namespace NerdDinner.CommandService.CommandHandlers
{
    public enum CancelDinnerStatus
    {
        DinnerDoesNotExists,
        Successful,
    }

    public class CancelDinnerCommandHandler : AggregateRootCommandHandler<CancelDinnerCommand, Dinner>
    {
        private readonly IDinnerService dinnerReadModel;

        public CancelDinnerCommandHandler(IDinnerService dinnerReadModel)
        {
            this.dinnerReadModel = dinnerReadModel;
        }

        public override int ValidateCommand(CancelDinnerCommand command)
        {
            if(!dinnerReadModel.DinnerExists(command.DinnerId))
                return (int)CancelDinnerStatus.DinnerDoesNotExists;

            return (int)CancelDinnerStatus.Successful;
        }

        public override void Handle(CancelDinnerCommand command, Dinner dinner)
        {
            dinner.Cancel();
        }
    }
}