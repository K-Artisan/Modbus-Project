using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.DataObject.DataPoint
{
    /// <summary>
    /// 数据点实时数据
    /// </summary>
    public class DataPointRealValueDto
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

    public class DataPointRealValueEventArgs : EventArgs
    {
        public List<DataPointRealValueDto> DataPointRealValues { get; set; }

        public DataPointRealValueEventArgs(List<DataPointRealValueDto> dataPointRealValues)
        {
            this.DataPointRealValues = dataPointRealValues;
        }
    }
}
