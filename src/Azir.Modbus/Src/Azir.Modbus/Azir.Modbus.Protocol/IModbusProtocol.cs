using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.DataReponse;
using Azir.Modbus.Protocol.FuncitonNum.CustomerRequest;
using Azir.Modbus.Protocol.FuncitonNum.ModbusRequest;

namespace Azir.Modbus.Protocol
{
    public interface IModbusProtocol
    {
        /// <summary>
        /// 获取请求帧（字节流）
        /// </summary>
        /// <typeparam name="T">客户端请求信息</typeparam>
        /// <param name="customerRequestData">客户端请求信息</param>
        /// <returns>请求帧（字节流）</returns>
        List<List<byte>> CreateRequestByteStreams<T>(ICustomerRequestData<T> customerRequestData)
            where T : IFunNumRequestDataBase;

        /// <summary>
        /// 解析响应字节流
        /// </summary>
        /// <param name="dataAnalyzeMode">数据解析方式</param>
        /// <param name="requestByteData">请求字节流</param>
        /// <param name="receviceByteData">响应字节流</param>
        /// <returns></returns>
        AnalyzeRecivedDataReponse AnalyzeRecivedData(DataAnalyzeMode dataAnalyzeMode, List<byte> requestByteData, List<byte> receviceByteData);
    }
}
