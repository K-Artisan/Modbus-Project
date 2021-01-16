using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modbus.Contract.RequestDataBase
{
    /// <summary>
    /// 来自界面输入的请求信息，需要转换成IFunNumRequestDataBase
    /// </summary>
    public interface ICustomerRequestData
    {
        /// <summary>
        /// 功能码
        /// </summary>
        FunctionNumType FunctionNum { get; set; }
   } 
}
