using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modbus.Contract.RequestDataBase
{
    /// <summary>
    /// 泛型（Generic）ICustomerRequestData
    /// </summary>
    /// <typeparam name="T">设置的值的类型，只能是如下类型:
    ///  double\ float\ int \long \short\ uint\ ulong\ ushort
    /// </typeparam>
    public interface IGenericCustomerRequestData<T> : ICustomerRequestData
    {
    }
}
