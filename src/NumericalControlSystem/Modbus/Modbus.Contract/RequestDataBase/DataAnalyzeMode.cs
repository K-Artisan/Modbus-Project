using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modbus.Contract.RequestDataBase
{
    /// <summary>
    /// 表示解析数据方式
    /// </summary>
    public enum DataAnalyzeMode
    {
        DataLowToHigh = 0, // 表示解析数据方式： 数据低位 数据高位 数据低位 数据高位
        DataHighToLow = 1  // 表示解析数据方式： 数据高位 数据低位 数据高位 数据低位
    }
}
