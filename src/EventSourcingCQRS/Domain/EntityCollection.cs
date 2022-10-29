using System.Collections.ObjectModel;

namespace EventSourcingCQRS.Domain
{
    public class EntityCollection<TEntity> : Collection<TEntity> where TEntity : Entity
    {
        private readonly AggregateRoot _aggregateRoot;

        public EntityCollection(AggregateRoot aggregateRoot)
        {
            _aggregateRoot = aggregateRoot;
        }

        public TEntity FindById(Guid id)
        {
            return this.SingleOrDefault(entity => entity.Id == id);
        }

        protected override void InsertItem(int index, TEntity entity)
        {
            _aggregateRoot.RegisterEntity(entity);
            base.InsertItem(index, entity);
        }
    }
}