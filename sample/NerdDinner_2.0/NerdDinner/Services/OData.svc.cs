using System;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using System.Xml.Linq;
using NerdDinner.Helpers;
using NerdDinner.Models;
using NerdDinner.Services;
using DataServicesJSONP;

namespace NerdDinner
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    [JSONPSupportBehavior]
    public class ODataServices : DataService<NerdDinnerEntities>
    {
    }
}
