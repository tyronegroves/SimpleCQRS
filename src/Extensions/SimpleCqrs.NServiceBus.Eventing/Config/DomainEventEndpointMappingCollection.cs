using System.Configuration;

namespace SimpleCqrs.NServiceBus.Eventing.Config
{
    public class DomainEventEndpointMappingCollection : ConfigurationElementCollection
    {
        public void Add(DomainEventEndpointMapping mapping)
        {
            BaseAdd(mapping);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, true);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DomainEventEndpointMapping();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new DomainEventEndpointMapping {DomainEvents = elementName};
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DomainEventEndpointMapping)element).DomainEvents;
        }

        public int IndexOf(DomainEventEndpointMapping mapping)
        {
            return BaseIndexOf(mapping);
        }

        public void Remove(DomainEventEndpointMapping mapping)
        {
            if(BaseIndexOf(mapping) >= 0)
            {
                BaseRemove(mapping.DomainEvents);
            }
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        public DomainEventEndpointMapping this[int index]
        {
            get { return (DomainEventEndpointMapping)BaseGet(index); }
            set
            {
                if(BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public new DomainEventEndpointMapping this[string name]
        {
            get { return (DomainEventEndpointMapping)BaseGet(name); }
        }
    }
}