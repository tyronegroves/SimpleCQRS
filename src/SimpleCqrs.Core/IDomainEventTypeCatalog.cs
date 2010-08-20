using System;
using System.Collections.Generic;

namespace SimpleCqrs.Core
{
    public interface IDomainEventTypeCatalog
    {
        IEnumerable<Type> GetAllEventTypes();
    }
}