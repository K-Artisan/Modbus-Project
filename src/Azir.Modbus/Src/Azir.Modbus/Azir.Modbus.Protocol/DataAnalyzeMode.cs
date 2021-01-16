using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.Protocol
{
    /// <summary>
    /// 表示解析数据方式
    /// </summary>
    public enum DataAnalyzeMode
    {
        /// <summary>
        /// 表示解析数据方式： 数据低位 数据高位 数据低位 数据高位
        /// </summary>
        DataLowToHigh = 0,  
        /// <summary>
        ///  表示解析数据方式： 数据高位 数据低位 数据高位 数据低位
        /// </summary>
        DataHighToLow = 1  
    }
}
