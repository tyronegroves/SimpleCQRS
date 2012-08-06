using System.Configuration;

namespace SimpleCqrs.NServiceBus.Eventing.Config
{
    public class DomainEventBusConfig : ConfigurationSection
    {
        private const string DomainEventEndpointMappingsElementName = "DomainEventEndpointMappings";

        [ConfigurationProperty(DomainEventEndpointMappingsElementName, IsRequired = false)]
        public DomainEventEndpointMappingCollection DomainEventEndpointMappings
        {
            get { return (base[DomainEventEndpointMappingsElementName] as DomainEventEndpointMappingCollection); }
            set { base[DomainEventEndpointMappingsElementName] = value; }
        }
    }
}