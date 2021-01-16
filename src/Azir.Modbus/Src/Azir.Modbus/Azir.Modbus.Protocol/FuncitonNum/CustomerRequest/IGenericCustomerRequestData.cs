using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.FuncitonNum.ModbusRequest;

namespace Azir.Modbus.Protocol.FuncitonNum.CustomerRequest
{
    /// <summary>
    /// 泛型（Generic）ICustomerRequestData
    /// </summary>
    /// T的类型：<see cref="IFunNumRequestDataBase"/>的具体实现类，
    /// G的类型：设置的值的类型
    /// <typeparam name="T">设置的值的类型，只能是如下类型:
    ///  double\ float\ int \long \short\ uint\ ulong\ ushort
    /// 
    /// </typeparam>
    public interface IGenericCustomerRequestData<T,G> : ICustomerRequestData<T>
    {
    }
}
