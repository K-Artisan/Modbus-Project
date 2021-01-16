using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Service.Messaging.DataPointService;
using NCS.Service.Messaging.ModbusService;

namespace NCS.Service.ServiceInterface
{
    public interface IModbusService
    {
        /// <summary>
        /// 获取指定DataPoint的实时数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetDataPointRealTimeDataResponse GetDataPointRealTimeData(GetDataPointRealTimeDataRequest request);

        /// <summary>
        /// 获取所有的DataPoint的实时数据
        /// </summary>
        /// <returns></returns>
        GetAllDataPointsRealTimeDataResponse GetAllDataPointsRealTimeData();

        /// <summary>
        /// 设置DataPiont的值（即：向Modbus发送命令，写寄存器的值）
        /// </summary>
        /// <param name="request"></param>
        void SetDataPointValue(SetDataPointValueRequest request);

    }
}
