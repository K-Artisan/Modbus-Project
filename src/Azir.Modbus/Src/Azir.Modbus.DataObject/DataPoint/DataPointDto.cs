using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.DataPoints;

namespace Azir.Modbus.DataObject.DataPoint
{
    /// <summary>
    /// DTO：数据点
    /// </summary>
    public class DataPointDto
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public int DeviceAddress { get; set; }
        public int StartRegisterAddress { get; set; }
        public DataPointDataType DataPointDataType { get; set; }
        public DataPointType DataPointType { get; set; }
        public string Description { get; set; }

        public double RealTimeValue { get; set; }
        public double ValueToSet { get; set; }
        /// <summary>
        /// 数据点所在模块编号
        /// </summary>
        public string ModuleNumber { get; set; }
        public string ModuleName { get; set; }
        /// <summary>
        /// Modbus处理单元(主要是根据的IP不同进行单元处理)
        /// </summary>
        public string ModbusUnitNumber { get; set; }
        public string ModbusUnitName { get; set; }

    }

}
