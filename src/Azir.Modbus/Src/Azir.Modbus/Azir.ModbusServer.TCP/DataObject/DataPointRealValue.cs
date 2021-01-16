using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.ModbusServer.TCP.DataObject
{
    /// <summary>
    /// 数据点实时数据
    /// </summary>
    public class DataPointRealValue
    {
        /// <summary>
        /// 数据点编号
        /// </summary>
        public string DataPointNumber { get; set; }
        /// <summary>
        /// 数据点实时数据值
        /// </summary>
        public double DataPointRealTimeValue { get; set; }
        /// <summary>
        /// 将要设置的值
        /// </summary>
        public double ValueToSet { get; set; }
    }

}
