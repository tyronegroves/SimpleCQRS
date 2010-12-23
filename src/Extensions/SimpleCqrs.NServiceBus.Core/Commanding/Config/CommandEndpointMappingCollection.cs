using System.Configuration;

namespace SimpleCqrs.NServiceBus.Commanding.Config
{
    public class CommandEndpointMappingCollection : ConfigurationElementCollection
    {
        public void Add(CommandEndpointMapping mapping)
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
            return new CommandEndpointMapping();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new CommandEndpointMapping {Commands = elementName};
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CommandEndpointMapping)element).Commands;
        }

        public int IndexOf(CommandEndpointMapping mapping)
        {
            return BaseIndexOf(mapping);
        }

        public void Remove(CommandEndpointMapping mapping)
        {
            if(BaseIndexOf(mapping) >= 0)
            {
                BaseRemove(mapping.Commands);
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

        public CommandEndpointMapping this[int index]
        {
            get { return (CommandEndpointMapping)BaseGet(index); }
            set
            {
                if(BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public new CommandEndpointMapping this[string name]
        {
            get { return (CommandEndpointMapping)BaseGet(name); }
        }
    }
}