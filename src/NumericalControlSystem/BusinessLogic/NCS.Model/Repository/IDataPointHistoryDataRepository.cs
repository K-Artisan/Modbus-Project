using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Domain;
using NCS.Infrastructure.Querying;
using NCS.Model.Entity;

namespace NCS.Model.Repository
{
    public interface IDataPointHistoryDataRepository : IRepository<DataPointHistoryData, string>
    {
    }
}
