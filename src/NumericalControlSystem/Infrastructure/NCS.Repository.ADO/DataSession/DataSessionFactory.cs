using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Logging;
using NCS.Model.Entity;
using NCS.Repository.ADO.DataSession;

namespace NCS.Repository.ADO.DataSession
{
    public static class DataSessionFactory
    {
        public static IDataSession<T, TEntityKey> GetDataSession<T, TEntityKey>()
        {
            IDataSession<T, TEntityKey> dataSession = null;

            Type type = typeof (T);

            if (type == typeof(DataPoint))
            {
                dataSession = (IDataSession<T, TEntityKey>)new DataPointDataSession();
            }
            else if (type == typeof(Module))
            {
                dataSession = (IDataSession<T, TEntityKey>)new ModuleDataSession();
            }
            else if (type == typeof(DataPointHistoryData))
            {
                dataSession = (IDataSession<T, TEntityKey>)new DataPointHistoryDataSession();
            }
            else
            {
                throw new MissDataSessionException("Unsupport DataSession for type:" + type.Name);
            }

            return dataSession;
        }
    }
}
