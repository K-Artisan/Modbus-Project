using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Querying;
using NCS.Infrastructure.UnitOfWork;
using NCS.Model.Entity;
using NCS.Model.Entity;
using NCS.Model.Repository;
using NCS.Repository.ADO.DataSession;

namespace NCS.Repository.ADO.Repositories
{
    public class DataPointHistoryDataRepository : Repository<DataPointHistoryData, string>, 
        IDataPointHistoryDataRepository
    {
        public DataPointHistoryDataRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }
    }
   
}
