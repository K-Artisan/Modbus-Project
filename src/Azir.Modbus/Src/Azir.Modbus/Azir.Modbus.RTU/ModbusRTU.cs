using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using Azir.Modbus.Protocol;
using Azir.Modbus.Protocol.DataPoints;
using Azir.Modbus.Protocol.DataReponse;
using Azir.Modbus.Protocol.FuncitonNum.CustomerRequest;
using Azir.Modbus.Protocol.FuncitonNum.ModbusRequest;
using Azir.Modbus.RTU;
using System.Threading;
using Azir.Modbus.RTU.RTURequest;

namespace Azir.Modbus.RTU
{
    public class ModbusRTU : IModbusProtocol
    {
        /// <summary>
        ///  从响应数据字节流中的第几位开始解析数据
        /// </summary>
        protected static int DataAnalyStartIndex = 0;

        #region 构造函数

        public ModbusRTU()
        {
        }

        #endregion 

        #region  IModbusProtocol 成员

        /// <summary>
        /// 根据功能码，执行相应的操作
        /// (暂时是ModbusTCP、UDP公用接口)
        /// </summary>
        /// <typeparam name="T">
        /// IFunNumRequestDataBase
        /// </typeparam>
        /// <param name="customerRequestData">客户端的请求</param>
        /// <returns></returns>
        /// <remarks>
        /// 调用示例：
        ///     FunNum01CustomerRequestData funCustomer01 = new FunNum01CustomerRequestData();
        ///     bool success = OperateFunctionNum(funCustomer01);
        /// </remarks>
        public bool OperateFunctionNum<T>(ICustomerRequestData<T> customerRequestData)
             where T : IFunNumRequestDataBase
        {
            bool succeed = true;

            try
            {
                List<List<byte>> rtuRequesCmdByteStreams = RTURequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);

                foreach (var item in rtuRequesCmdByteStreams)
                {
                    //WriteSerialPort(item.ToArray());
                }

            }
            catch (Exception)
            {
                succeed = false;
            }
            return succeed;
        }

        #endregion


        /// <summary>
        /// 根据数据点获取读取寄存器命令（若干个请求字节流）
        /// </summary>
        /// <param name="dataPoints">
        /// dataPoints中的DataPoint不必满足如下条件：
        /// 1.设备地址相同；
        /// 2.读寄存器用的功能码相同；
        /// 3.相邻DataPoint的寄存器地址是连续的。
        /// 
        /// 该方法内部将自动调整为以上条件
        /// </param>
        /// <returns></returns>
        public static List<List<byte>> CreateReadRegisterCommands(List<DataPoint> dataPoints)
        {
            List<List<byte>> readRegisterCommands = new List<List<byte>>();

            if (null == dataPoints)
            {
                return readRegisterCommands;
            }

            List<List<DataPoint>> dataPointsGroupedForRead = DataPointGrouper.GroupingDataPointsForReadRegister(dataPoints);

            foreach (var dataPointsGroup in dataPointsGroupedForRead)
            {
                List<List<byte>> requestCommadBytes = RTURequestCommandByteStreamCreator.CreateRequestCommandByteStreamForReadRegisterBy(dataPointsGroup);

                foreach (var requestCommadByte in requestCommadBytes)
                {
                    readRegisterCommands.Add(requestCommadByte);
                }
            }

            return readRegisterCommands;
        }


        /// <summary>
        /// 根据数据点获取写寄存器命令（若干个请求字节流)
        /// </summary>
        /// <param name="dataAnalyzeMode">数据解析模式</param>
        /// <param name="dataPoints">
        /// dataPoints中的DataPoint不必满足如下条件：
        /// 1.设备地址相同；
        /// 2.读寄存器用的功能码相同；
        /// 3.相邻DataPoint的寄存器地址是连续的。
        /// 
        /// 该方法内部将自动调整为以上条件
        /// </param>
        /// <returns></returns>
        public static List<List<byte>> CreateWriteRegisterCommands(DataAnalyzeMode dataAnalyzeMode, List<DataPoint> dataPoints)
        {
            List<List<byte>> writeRegisterCommands = new List<List<byte>>();

            if (null == dataPoints || dataPoints.Count < 1)
            {
                return writeRegisterCommands;
            }

            List<List<DataPoint>> dataPointsGroupedForWrite = DataPointGrouper.GroupingDataPointsForWriteRegister(dataPoints);

            foreach (var dataPointsGroup in dataPointsGroupedForWrite)
            {
                List<List<byte>> requestCommadBytes =
                    RTURequestCommandByteStreamCreator.CreateRequestCommandByteStreamForWriteRegisterBy(dataAnalyzeMode, dataPointsGroup);

                foreach (var requestCommadByte in requestCommadBytes)
                {
                    writeRegisterCommands.Add(requestCommadByte);
                }
            }

            return writeRegisterCommands;
        }

        /// <summary>
        /// 获取请求帧（字节流）
        /// </summary>
        /// <typeparam name="T">客户端请求信息</typeparam>
        /// <param name="customerRequestData">客户端请求信息</param>
        /// <returns>请求帧（字节流）</returns>
        /// <remarks>
        /// 调用示例：
        ///     FunNum01CustomerRequestData funCustomer01 = new FunNum01CustomerRequestData();
        ///     bool success = CreateRequestByteStreams(funCustomer01);
        /// </remarks>
        public static List<List<byte>> CreateRequestByteStreamsStatic<T>(ICustomerRequestData<T> customerRequestData)
            where T : IFunNumRequestDataBase
        {
            var rtuRequesCmdByteStreams = new List<List<byte>>();
            try
            {
                rtuRequesCmdByteStreams = RTURequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);
            }
            catch (Exception)
            {
                rtuRequesCmdByteStreams = null;
            }

            return rtuRequesCmdByteStreams;
        }

        /// <summary>
        /// 解析响应字节流
        /// </summary>
        /// <param name="dataAnalyzeMode">数据解析方式</param>
        /// <param name="requestByteData">请求字节流</param>
        /// <param name="receviceByteData">响应字节流</param>
        /// <returns></returns>
        public static AnalyzeRecivedDataReponse AnalyzeRecivedDataStatic(DataAnalyzeMode dataAnalyzeMode, List<byte> requestByteData, List<byte> receviceByteData)
        {
            return ModbusRecivedDataAnalyzer.AnalyzeRecivedData(dataAnalyzeMode, requestByteData, receviceByteData);
        }

        /// <summary>
        /// 将（若干个）寄存器值的值设置为其对应的数据点的值
        /// </summary>
        /// <param name="registers">目标寄存器的集合</param>
        /// <param name="allDataPoints">
        /// 所有目标数据点的集合，包括：
        /// 1.存在目标寄存器的集合对应的数据点的集合；
        /// 2.不存在目标寄存器的集合对应的数据点的集合
        /// </param>
        /// <returns>目标寄存器的集合中对应的数据点的集合(值发生变化的)</returns>
        public static List<DataPoint> SetDataPointValueFromRegisterValue(List<Register> registers,
            List<DataPoint> allDataPoints)
        {
            return DataPointProcessor.SetDataPointValueFromRegisterValue(registers, allDataPoints);
        }

        #region Member of IModbusProtocol
        /// <summary>
        /// 获取请求帧（字节流）
        /// </summary>
        /// <typeparam name="T">客户端请求信息</typeparam>
        /// <param name="customerRequestData">客户端请求信息</param>
        /// <returns>请求帧（字节流）</returns>
        /// <remarks>
        /// 调用示例：
        ///     FunNum01CustomerRequestData funCustomer01 = new FunNum01CustomerRequestData();
        ///     bool success = CreateRequestByteStreams(funCustomer01);
        /// </remarks>
        public List<List<byte>> CreateRequestByteStreams<T>(ICustomerRequestData<T> customerRequestData)
            where T : IFunNumRequestDataBase
        {
            return CreateRequestByteStreamsStatic(customerRequestData);
        }

        /// <summary>
        /// 解析响应字节流
        /// </summary>
        /// <param name="dataAnalyzeMode">数据解析方式</param>
        /// <param name="requestByteData">请求字节流</param>
        /// <param name="receviceByteData">响应字节流</param>
        /// <returns></returns>
        public AnalyzeRecivedDataReponse AnalyzeRecivedData(DataAnalyzeMode dataAnalyzeMode, List<byte> requestByteData, List<byte> receviceByteData)
        {
            return AnalyzeRecivedDataStatic(dataAnalyzeMode, requestByteData, receviceByteData);
        }

        #endregion
    }
}
