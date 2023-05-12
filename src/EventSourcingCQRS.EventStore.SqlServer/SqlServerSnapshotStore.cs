using EventSourcingCQRS.Eventing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcingCQRS.Domain;

namespace EventSourcingCQRS.EventStore.SqlServer
{
    public class SqlServerSnapshotStore : ISnapshotStore
    {
        public Task<Snapshot> GetSnapshot(Guid aggregateRootId, CancellationToken cancellationToken)
        {
            Snapshot documents = null;
            return Task.FromResult(documents) ;

        }

        public Task SaveSnapshot<TSnapshot>(TSnapshot snapshot, CancellationToken cancellationToken) where TSnapshot : Snapshot
        {
            throw new NotImplementedException();
        }
    }
}
