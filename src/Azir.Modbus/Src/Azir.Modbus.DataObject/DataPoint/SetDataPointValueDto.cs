using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.DataObject.DataPoint
{
    public class SetDataPointValueDto
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
