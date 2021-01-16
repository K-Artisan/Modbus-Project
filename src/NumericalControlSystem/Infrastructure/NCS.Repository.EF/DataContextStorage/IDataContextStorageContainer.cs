using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace NCS.Repository.EF
{
    public interface IDataContextStorageContainer
    {
        DbContext GetDataContext();
        void Store(DbContext libraryDataContext);
    }
}
