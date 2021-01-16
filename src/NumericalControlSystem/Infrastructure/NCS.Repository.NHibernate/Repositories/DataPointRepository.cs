using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.UnitOfWork;
using NCS.Model.Entity;
using NCS.Model.Repository;


namespace NCS.Repository.NHibernate.Repositories
{
    public class DataPointRepository : Repository<DataPoint,int>, IDataPointRepository
    {
        public DataPointRepository(IUnitOfWork uow)
            : base(uow)
        {
            
        }
    }
}
