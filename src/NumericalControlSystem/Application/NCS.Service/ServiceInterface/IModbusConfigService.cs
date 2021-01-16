using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Service.Messaging.ModbusConfigService;

namespace NCS.Service.ServiceInterface
{
    public interface IModbusConfigService
    {
        /// <summary>
        /// 将Modbus配置保存到数据库
        /// </summary>
        /// <returns></returns>
        SetModbusConfigToDataBaseResponse SetModbusConfigToDataBase();

        /// <summary>
        /// 设置解析数据的方式
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SetDataAnalyzeModeResponse SetDataAnalyzeMode(SetDataAnalyzeModeRequest request);

        /// <summary>
        /// 获取解析数据的方式
        /// </summary>
        /// <returns></returns>
        GetDataAnalyzeModeResponse GetDataAnalyzeMode();
    }
}
