using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.Service.AutoMapper
{
    /// <summary>
    /// 自动建立ViewModel与领域实体的映射
    /// </summary>
    public static class AutoMapperBootStrapper
    {
        public static void ConfigureAutoMapper()
        {
            ////DataPoint
            //Mapper.CreateMap<DataPoint, DataPointInfoView>();

            //////DataPointHistoryData
            //Mapper.CreateMap<DataPointHistoryData, DataPointHistoryDataView>();
        }
    }
}
