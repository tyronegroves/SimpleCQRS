using System.Configuration;

namespace SimpleCqrs.NServiceBus.Commanding.Config
{
    public class CommandBusConfig : ConfigurationSection
    {
        private const string CommandEndpointMappingsElementName = "CommandEndpointMappings";

        [ConfigurationProperty(CommandEndpointMappingsElementName, IsRequired = false)]
        public CommandEndpointMappingCollection CommandEndpointMappings
        {
            get { return (base[CommandEndpointMappingsElementName] as CommandEndpointMappingCollection); }
            set { base[CommandEndpointMappingsElementName] = value; }
        }
    }
}