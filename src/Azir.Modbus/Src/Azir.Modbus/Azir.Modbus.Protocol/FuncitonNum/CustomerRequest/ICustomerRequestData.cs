using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.FuncitonNum.ModbusRequest;

namespace Azir.Modbus.Protocol.FuncitonNum.CustomerRequest
{
    /// <summary>
    /// 来自界面输入的客户端请求信息（类型），
    /// T的类型：<see cref="IFunNumRequestDataBase"/>的具体实现类
    /// </summary>
    public interface ICustomerRequestData<T>
    {
        /// <summary>
        /// 功能码
        /// </summary>
        FunctionNumType FunctionNum { get; set; }
        /// <summary>
        /// 将客户端请求信息（类型）转换成
        /// <see cref="IFunNumRequestDataBase"/>
        /// </summary>
        /// <returns></returns>
        List<T> CovertToFunNumRequestDataBases();
    } 
}
