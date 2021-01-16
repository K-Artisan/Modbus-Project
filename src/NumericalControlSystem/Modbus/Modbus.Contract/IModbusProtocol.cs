using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modbus.Contract.RequestDataBase;

namespace Modbus.Contract
{
    /// <summary>
    /// Modbus驱动的协议
    /// </summary>
    public interface IModbusProtocol
    {
        /// <summary>
        /// 请求帧
        /// </summary>
        List<byte> CurrentRequestData { get; set;}

        /// <summary>
        /// 返回帧
        /// </summary>
        List<byte> CurrentReceiveData { get; set; }

        /// <summary>
        /// 请求帧发生改变事件
        /// </summary>
        event EventHandler<RequstDataEventArgs> OnCurrentRequestDataChanged;

        /// <summary>
        /// 返回帧发生改变事件
        /// </summary>
        event EventHandler<ReceiveDataEventArgs> OnCurrentReceiveDataChanged;

        /// <summary>
        /// 根据功能码，执行相应的操作
        /// (暂时是ModbusTCP、UDP公用接口)
        /// 
        /// 特别注意：
        /// T类型只是针对如下功能码:
        /// 06\16
        /// 其他功能码可以设置为任何类型,因为没用到类型T，
        /// 只是为了统一接口，而使用了泛型函数。
        /// 
        /// </summary>
        /// <typeparam name="T">设置的值的类型，只能是如下类型:
        ///  double\ float\ int \long \short\ uint\ ulong\ ushort
        /// </typeparam>
        /// <param name="customerRequestData">客户端的请求</param>
        /// <returns></returns>
        bool OperateFunctionNum<T>(ICustomerRequestData customerRequestData);

    }
}
