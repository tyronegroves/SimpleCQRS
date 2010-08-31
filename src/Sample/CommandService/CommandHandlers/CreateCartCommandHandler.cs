using System;
using Commands.Domain;
using SimpleCqrs.Commanding;
using SimpleCqrs.Eventing;

namespace Commands.CommandHandlers
{
    public class CreateCartCommandHandler : CommandHandler<CreateCartCommand>
    {
        private readonly IDomainRepository repository;

        public CreateCartCommandHandler(IDomainRepository repository)
        {
            this.repository = repository;
        }

        protected override void Handle(CreateCartCommand command)
        {
            if (command.Id == Guid.Empty)
            {
                Return(1);
                return;
            }

            Return(0);

            var cart = Cart.Create(command.Id);
            repository.Save(cart);
        }
    }
}