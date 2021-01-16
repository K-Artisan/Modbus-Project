using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;

namespace NCS.Repository.EF
{
    public class HttpDataContextStorageContainer : IDataContextStorageContainer 
    {
        private string _dataContextKey = "DataContext";
        
        public DbContext GetDataContext()
        {
            DbContext objectContext = null;
            if (HttpContext.Current.Items.Contains(_dataContextKey))
                objectContext = (DbContext)HttpContext.Current.Items[_dataContextKey];

            return objectContext;
        }

        public void Store(DbContext dataContext)
        {
            if (HttpContext.Current.Items.Contains(_dataContextKey))
                HttpContext.Current.Items[_dataContextKey] = dataContext;
            else
                HttpContext.Current.Items.Add(_dataContextKey, dataContext);  
        }

    }
}
