using System.Configuration;

namespace SimpleCqrs.NServiceBus.Commanding.Config
{
    public class CommandEndpointMapping : ConfigurationElement
    {
        private const string CommandsPropertyName = "Commands";
        private const string EndpointPropertyName = "Endpoint";

        [ConfigurationProperty(EndpointPropertyName, IsRequired = true, IsKey = true)]
        public string Endpoint
        {
            get { return (string)base[EndpointPropertyName]; }
            set { base[EndpointPropertyName] = value; }
        }

        [ConfigurationProperty(CommandsPropertyName, IsRequired = true, IsKey = true)]
        public string Commands
        {
            get { return (string)base[CommandsPropertyName]; }
            set { base[CommandsPropertyName] = value; }
        }
    }
}