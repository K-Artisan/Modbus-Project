using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.Protocol.DataPoints
{
    public class DataPoint
    {
        /// <summary>
        /// 数据点编号，必须唯一
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 设备（从机）地址，或 单元标识符
        /// 取值范围是一个字节：0-127
        /// </summary>
        public int DeviceAddress { get; set; }

        /// <summary>
        /// 数据点对应的寄存器的起始寄存器地址
        /// ，与<see cref="DataPointDataType"/> 一起决定该数据点的结束寄存器的地址
        /// </summary>
        public int StartRegisterAddress { get; set; }
        /// <summary>
        /// 数据点的数据类型
        /// ，与<see cref="StartRegisterAddress"/> 一起决定该数据点的结束寄存器的地址
        /// </summary>
        public DataPointDataType DataPointDataType { get; set; }

        /// <summary>
        /// 数据点的类型
        /// </summary>
        public DataPointType DataPointType { get; set; }

        /// <summary>
        /// 数据点的实时数据
        /// </summary>
        public double RealTimeValue { get; set; }

        /// <summary>
        /// 将要设置的值
        /// </summary>
        public double ValueToSet { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }

    public static class DataPointEx
    {
        public static DataPoint CopyNew(this DataPoint dataPoint)
        {
            return new DataPoint()
            {
                Number = dataPoint.Number,
                Name = dataPoint.Name,
                DeviceAddress = dataPoint.DeviceAddress,
                StartRegisterAddress = dataPoint.StartRegisterAddress,
                DataPointDataType = dataPoint.DataPointDataType,
                DataPointType = dataPoint.DataPointType,
                RealTimeValue = dataPoint.RealTimeValue,
                ValueToSet = dataPoint.ValueToSet,
                Description = dataPoint.Description

            };
        }
    }
}
