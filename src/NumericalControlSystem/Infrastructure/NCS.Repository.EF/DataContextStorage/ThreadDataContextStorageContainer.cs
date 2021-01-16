using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Data.Entity;

namespace NCS.Repository.EF
{
    public class ThreadDataContextStorageContainer : IDataContextStorageContainer 
    {    
        private static readonly Hashtable _dbContexts = new Hashtable();

        public DbContext GetDataContext()
        {
            DbContext dataContext = null;

            if (_dbContexts.Contains(GetThreadId()))
                dataContext = (DbContext)_dbContexts[GetThreadId()];           

            return dataContext;
        }

        public void Store(DbContext dataContext)
        {
            if (_dbContexts.Contains(GetThreadId()))
                _dbContexts[GetThreadId()] = dataContext;
            else
                _dbContexts.Add(GetThreadId(), dataContext);           
        }

        private static int GetThreadId()
        {
            return Thread.CurrentThread.ManagedThreadId;
        }     
    }
}
