using System;
using NServiceBus;
using SimpleCqrs.NServiceBus.Eventing;

namespace Client
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            Console.WriteLine("Press enter to start listening for messages");
            Console.ReadLine();

            Configure.With()
                .DefaultBuilder()
                .BinarySerializer()
                .Log4Net()
                .MsmqTransport()
                .UnicastBus()
                    .SubscribeForDomainEvents()
                    .CreateBus()
                    .Start();

            Console.ReadLine();
        }
    }
}