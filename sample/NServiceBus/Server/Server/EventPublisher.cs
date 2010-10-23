using System.Timers;
using Events;
using NServiceBus;
using NServiceBus.Unicast;
using SimpleCqrs.NServiceBus.Eventing;

namespace Server
{
    public class EventPublisher : IWantToRunAtStartup
    {
        private readonly IBus bus;
        private readonly Timer timer;

        public EventPublisher(UnicastBus bus)
        {
            this.bus = bus;
            timer = new Timer(5000) {AutoReset = true};
            timer.Elapsed += TimerElapse;
            timer.Start();
        }

        private void TimerElapse(object sender, ElapsedEventArgs e)
        {
            bus.Publish<IDomainEventMessage>(new DomainEventMessage<MyTestEvent> { DomainEvent = new MyTestEvent() });
            bus.Publish<IDomainEventMessage>(new DomainEventMessage<MyTestEvent2> { DomainEvent = new MyTestEvent2() });
        }

        public void Run()
        {
        }

        public void Stop()
        {
        }
    }
}