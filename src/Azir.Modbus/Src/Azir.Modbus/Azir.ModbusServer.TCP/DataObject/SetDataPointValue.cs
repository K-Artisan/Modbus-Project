using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.ModbusServer.TCP.DataObject
{
    public class SetDataPointValue
    {
        /// <summary>
        /// 数据点唯一标识
        /// </summary>
        public string DataPointNumber { get; set; }
        /// <summary>
        /// 将要设置的值
        /// </summary>
        public double ValueToSet { get; set; }
    }
}
