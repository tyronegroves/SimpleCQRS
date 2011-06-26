using Sample.JesseHouse.Commands;
using Sample.JesseHouse.Domain;
using SimpleCqrs.Commanding;
using SimpleCqrs.Domain;

namespace Sample.JesseHouse.Processing
{
    public class CreateCustomerCommandHandler : CommandHandler<CreateCustomerCommand>
    {
        private readonly IDomainRepository domainRepository;

        public CreateCustomerCommandHandler(IDomainRepository domainRepository)
        {
            this.domainRepository = domainRepository;
        }

        public override void Handle(CreateCustomerCommand command)
        {
            var customer = new Customer(command.CustomerId);
            customer.SetName(command.FirstName, command.LastName);

            domainRepository.Save(customer);
        }
    }
}