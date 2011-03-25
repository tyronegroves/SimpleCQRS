using System;
using System.Collections;
using SimpleCqrs.Commanding;
using SimpleCQRSDemo.Commands;
using SimpleCQRSDemo.FakeDb;
using SimpleCQRSDemo.ReadModel;

namespace SimpleCQRSDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            var runtime = new SampleRunTime();

            runtime.Start();

            // Infrastructure and fakes
            var fakeAccountTable = new FakeAccountTable();
            runtime.ServiceLocator.Register(fakeAccountTable); // Create Fake-db
            runtime.ServiceLocator.Register(new AccountReportReadService(fakeAccountTable));
            var commandBus = runtime.ServiceLocator.Resolve<ICommandBus>();


            // Create and send a couple of command
            var cmdMarcus = new CreateAccountCommand { FirstName = "Marcus", LastName = "Hammarberg" };
            var cmdDarren = new CreateAccountCommand { FirstName = "Darren", LastName = "Cauthon" };
            var cmdTyrone = new CreateAccountCommand { FirstName = "Tyrone", LastName = "Groves" };
            commandBus.Send(cmdMarcus);
            commandBus.Send(cmdDarren);
            commandBus.Send(cmdTyrone);

            // Get the denormalized version of the data back from the read model
            var accountReportReadModel = runtime.ServiceLocator.Resolve<AccountReportReadService>();
            Console.WriteLine("Accounts in database");
            Console.WriteLine("####################");
            foreach (var account in accountReportReadModel.GetAccounts())
            {
                Console.WriteLine(" Id: {0} Name: {1}", account.Id, account.Name);
            }



            runtime.Shutdown();

            Console.ReadLine();
        }
    }
}
