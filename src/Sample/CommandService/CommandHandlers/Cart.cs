using System.Threading;
using SimpleCqrs.Commanding;
using SimpleCqrs.Eventing;

namespace Commands.CommandHandlers
{
    public class Cart : CommandHandler<CreateCartCommand>
    {
        private readonly IDomainRepository repository;

        public Cart(IDomainRepository repository)
        {
            this.repository = repository;
        }

        protected override void Handle(CreateCartCommand command)
        {
            Return(15);

            Thread.Sleep(12000);
        }
    }
}