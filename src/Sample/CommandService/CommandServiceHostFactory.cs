using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Commands
{
    public class CommandServiceHostFactory : ServiceHostFactory
    {
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            var serviceLocator = Global.ServiceLocator;
            return new ServiceHost(serviceLocator.Resolve<CommandService>(), baseAddresses);
        }
    }
}