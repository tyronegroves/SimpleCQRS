using System.Collections.Generic;
using System.Linq;
using SimpleCQRSDemo.FakeDb;

namespace SimpleCQRSDemo.ReadModel
{
    public class AccountReportReadService
    {
        private FakeAccountTable fakeAccountDb;

        public AccountReportReadService(FakeAccountTable fakeAccountDb)
        {
            this.fakeAccountDb = fakeAccountDb;
        }

        public IEnumerable<AccountReadModel> GetAccounts()
        {
            return from a in fakeAccountDb
                   select new AccountReadModel { Id = a.Id, Name = a.Name };
        }
    }
}