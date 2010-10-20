using System.Configuration;

namespace SimpleCqrs.NServiceBus.Eventing.Config
{
    public class DomainEventEndpointMapping : ConfigurationElement
    {
        private const string DomainEventsPropertyName = "DomainEvents";
        private const string EndpointPropertyName = "Endpoint";

        [ConfigurationProperty(EndpointPropertyName, IsRequired = true, IsKey = true)]
        public string Endpoint
        {
            get { return (string)base[EndpointPropertyName]; }
            set { base[EndpointPropertyName] = value; }
        }

        [ConfigurationProperty(DomainEventsPropertyName, IsRequired = true, IsKey = true)]
        public string DomainEvents
        {
            get { return (string)base[DomainEventsPropertyName]; }
            set { base[DomainEventsPropertyName] = value; }
        }
    }
}