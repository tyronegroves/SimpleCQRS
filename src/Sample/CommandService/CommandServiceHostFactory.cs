using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using SimpleCqrs;

namespace Commands
{
    public class CommandServiceHostFactory : ServiceHostFactory
    {
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            var serviceLocator = ServiceLocator.Current;
            return new ServiceHost(serviceLocator.Resolve<CommandService>(), baseAddresses);
        }
    }
}