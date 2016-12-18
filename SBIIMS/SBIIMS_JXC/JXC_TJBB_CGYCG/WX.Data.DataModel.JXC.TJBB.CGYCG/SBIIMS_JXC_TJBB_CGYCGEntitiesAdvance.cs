using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using WX.Data.DatabaseHelper;
using System.Data.EntityClient;
namespace WX.Data.DataModel
{
    public class SBIIMS_JXC_TJBB_CGYCGEntitiesAdvance : SBIIMS_JXC_TJBB_CGYCGEntities
    {
        public SBIIMS_JXC_TJBB_CGYCGEntitiesAdvance()
            : base(ConnectionString.GetEmbeddedResourceEntityConnectionString(ConnectionString.SBIIMS_JXC, "WX.Data.DataModel.SBIIMS_JXC_TJBB_CGYCG", Assembly.GetExecutingAssembly().GetName().FullName))
        { }
    }
}
