using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;

namespace NCS.Repository.EF
{
    public class DataContextFactory
    {
        public static DbContext GetDataContext<T>() 
            where T: DbContext, new()
        {
            IDataContextStorageContainer _dataContextStorageContainer = DataContextStorageFactory.CreateStorageContainer();

            DbContext dataContext = _dataContextStorageContainer.GetDataContext();
            if (dataContext == null)
            {
                dataContext = new T();
                _dataContextStorageContainer.Store(dataContext);
            }

            return dataContext;            
        }
    }
}
