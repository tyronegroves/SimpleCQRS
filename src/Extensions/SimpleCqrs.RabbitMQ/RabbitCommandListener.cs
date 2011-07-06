using System.Threading;
using SimpleCqrs.Commanding;

namespace SimpleCqrs.RabbitMQ
{
    public class RabbitCommandListener
    {
        private readonly ICommandBus commandBus;
        private Thread listenerThread;

        public RabbitCommandListener(ICommandBus commandBus)
        {
            this.commandBus = commandBus;
        }

        public void StartListening()
        {
            listenerThread = new Thread(OnStartListening);
            listenerThread.Start();
        }

        public void StopListening()
        {
        }

        private void OnStartListening()
        {
            
        }
    }
}