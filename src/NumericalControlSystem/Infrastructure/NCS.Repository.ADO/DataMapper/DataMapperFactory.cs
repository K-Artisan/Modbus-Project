using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Model.Entity;
using NCS.Repository.ADO.DataSession;

namespace NCS.Repository.ADO.DataMapper
{
    public static class DataMapperFactory
    {
        // TODO:希望以后能将IDataMapper配置到xml文件中，
        // TODO：而不是不用这不灵活的if判断语句
        public static IDataMapper<T> GetDataMapper<T>()
        {
            IDataMapper<T> dataMapper = null;

            Type type = typeof(T);

            if (type == typeof(DataPoint))
            {
                dataMapper = (IDataMapper<T>)new DataPointDataMapper();
            }
            else if (type == typeof(Module))
            {
                dataMapper = (IDataMapper<T>) new ModuleDataMapper();
            }
            else if (type == typeof(DataPointHistoryData))
            {
                dataMapper = (IDataMapper<T>)new DataPointHistoryDataDataMapper();
            }
            else
            {
                throw new Exception("Unsupport DataSession for type:" + type.Name);
            }

            return dataMapper;
        }
    }
}
