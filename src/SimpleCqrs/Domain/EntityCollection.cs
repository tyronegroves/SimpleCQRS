using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SimpleCqrs.Domain
{
    public class EntityCollection<TEntity> : Collection<TEntity> where TEntity : Entity
    {
        private readonly AggregateRoot aggregateRoot;

        public EntityCollection(AggregateRoot aggregateRoot)
        {
            this.aggregateRoot = aggregateRoot;
        }

        public TEntity FindById(Guid id)
        {
            return this.SingleOrDefault(entity => entity.Id == id);
        }

        protected override void InsertItem(int index, TEntity entity)
        {
            aggregateRoot.RegisterEntity(entity);
            base.InsertItem(index, entity);
        }
    }
}