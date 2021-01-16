using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.DataReponse;

namespace Azir.Modbus.Protocol.DataPoints
{
    /// <summary>
    /// 数据点处理器
    /// </summary>
    public class DataPointProcessor
    {
        /// <summary>
        /// 将（若干个）寄存器值的值设置为其对应的数据点的值
        /// </summary>
        /// <param name="registers">目标寄存器的集合</param>
        /// <param name="allDataPoints">
        /// 所有目标数据点的集合，包括：
        /// 1.存在目标寄存器的集合对应的数据点的集合；
        /// 2.不存在目标寄存器的集合对应的数据点的集合
        /// </param>
        /// <returns>目标寄存器的集合中对应的数据点的集合</returns>
        public static List<DataPoint> SetDataPointValueFromRegisterValue(List<Register> registers, List<DataPoint> allDataPoints)
        {
            List<DataPoint> dataPointsWhoseRealTimeDataChanged = new List<DataPoint>();

            if (null == registers || null == allDataPoints)
            {
                return dataPointsWhoseRealTimeDataChanged;
            }

            //目标寄存器的集合中对应的数据点的集合

            for (int i = 0; i < registers.Count; i++)
            {
                DataPoint dataPoint = allDataPoints.Find(p => p.DeviceAddress == registers[i].DeviceAddress
                                                              && p.StartRegisterAddress == registers[i].RegisterAddress);

                if (null != dataPoint)
                {
                    switch (dataPoint.DataPointDataType)
                    {
                        case DataPointDataType.S16:
                            {
                                byte[] byteValues = BitConverter.GetBytes(registers[i].RegisterValue);
                                double realTimeValue = BitConverter.ToInt16(byteValues, 0);

                                if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)  //值发生变化了才赋值，下同
                                {
                                    dataPoint.RealTimeValue = realTimeValue;
                                    dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                }
                                break;
                            }

                        case DataPointDataType.U16:
                            {
                                byte[] byteValues = BitConverter.GetBytes(registers[i].RegisterValue);
                                double realTimeValue = BitConverter.ToUInt16(byteValues, 0);

                                if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                {
                                    dataPoint.RealTimeValue = realTimeValue;
                                    dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                }

                                break;
                            }

                        case DataPointDataType.S32:
                            {
                                if (i + 1 < registers.Count)
                                {
                                    byte[] byteValuesLow = BitConverter.GetBytes(registers[i].RegisterValue);
                                    byte[] byteValuesHigh = BitConverter.GetBytes(registers[i + 1].RegisterValue);
                                    byte[] byteValues = new byte[]
                                    {
                                        byteValuesLow[0],
                                        byteValuesLow[1],
                                        byteValuesHigh[0],
                                        byteValuesHigh[1]

                                    };

                                    double realTimeValue = BitConverter.ToInt32(byteValues, 0);

                                    if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                    {
                                        dataPoint.RealTimeValue = realTimeValue;
                                        dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                    }
                                }
                                break;
                            }

                        case DataPointDataType.U32:
                            {
                                if (i + 1 < registers.Count)
                                {
                                    byte[] byteValuesLow = BitConverter.GetBytes(registers[i].RegisterValue);
                                    byte[] byteValuesHigh = BitConverter.GetBytes(registers[i + 1].RegisterValue);
                                    byte[] byteValues = new byte[]
                                    {
                                        byteValuesLow[0],
                                        byteValuesLow[1],
                                        byteValuesHigh[0],
                                        byteValuesHigh[1]

                                    };

                                    double realTimeValue = BitConverter.ToUInt32(byteValues, 0);

                                    if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                    {
                                        dataPoint.RealTimeValue = realTimeValue;
                                        dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                    }
                                }

                                break;
                            }

                        case DataPointDataType.S64:
                            {
                                if (i + 3 < registers.Count)
                                {
                                    byte[] byteValues01 = BitConverter.GetBytes(registers[i].RegisterValue);
                                    byte[] byteValues02 = BitConverter.GetBytes(registers[i + 1].RegisterValue);
                                    byte[] byteValues03 = BitConverter.GetBytes(registers[i + 2].RegisterValue);
                                    byte[] byteValues04 = BitConverter.GetBytes(registers[i + 3].RegisterValue);

                                    byte[] byteValues = new byte[]
                                    {
                                        byteValues01[0],
                                        byteValues01[1],
                                        byteValues02[0],
                                        byteValues02[1],
                                        byteValues03[0],
                                        byteValues03[1],
                                        byteValues04[0],
                                        byteValues04[1]
                                    };

                                    double realTimeValue = BitConverter.ToInt64(byteValues, 0);

                                    if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                    {
                                        dataPoint.RealTimeValue = realTimeValue;
                                        dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                    }
                                }

                                break;
                            }

                        case DataPointDataType.U64:
                            {
                                if (i + 3 < registers.Count)
                                {
                                    byte[] byteValues01 = BitConverter.GetBytes(registers[i].RegisterValue);
                                    byte[] byteValues02 = BitConverter.GetBytes(registers[i + 1].RegisterValue);
                                    byte[] byteValues03 = BitConverter.GetBytes(registers[i + 2].RegisterValue);
                                    byte[] byteValues04 = BitConverter.GetBytes(registers[i + 3].RegisterValue);

                                    byte[] byteValues = new byte[]
                                    {
                                        byteValues01[0],
                                        byteValues01[1],
                                        byteValues02[0],
                                        byteValues02[1],
                                        byteValues03[0],
                                        byteValues03[1],
                                        byteValues04[0],
                                        byteValues04[1]
                                    };

                                    double realTimeValue = BitConverter.ToUInt64(byteValues, 0);

                                    if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                    {
                                        dataPoint.RealTimeValue = realTimeValue;
                                        dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                    }
                                }

                                break;
                            }

                        case DataPointDataType.F32:
                            {
                                if (i + 1 < registers.Count)
                                {
                                    byte[] byteValuesLow = BitConverter.GetBytes(registers[i].RegisterValue);
                                    byte[] byteValuesHigh = BitConverter.GetBytes(registers[i + 1].RegisterValue);
                                    byte[] byteValues = new byte[]
                                    {
                                        byteValuesLow[0],
                                        byteValuesLow[1],
                                        byteValuesHigh[0],
                                        byteValuesHigh[1]

                                    };

                                    double realTimeValue = BitConverter.ToSingle(byteValues, 0);

                                    if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                    {
                                        dataPoint.RealTimeValue = realTimeValue;
                                        dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                    }
                                }

                                break;
                            }

                        case DataPointDataType.D64:
                            {
                                if (i + 3 < registers.Count)
                                {
                                    byte[] byteValues01 = BitConverter.GetBytes(registers[i].RegisterValue);
                                    byte[] byteValues02 = BitConverter.GetBytes(registers[i + 1].RegisterValue);
                                    byte[] byteValues03 = BitConverter.GetBytes(registers[i + 2].RegisterValue);
                                    byte[] byteValues04 = BitConverter.GetBytes(registers[i + 3].RegisterValue);

                                    byte[] byteValues = new byte[]
                                    {
                                        byteValues01[0],
                                        byteValues01[1],
                                        byteValues02[0],
                                        byteValues02[1],
                                        byteValues03[0],
                                        byteValues03[1],
                                        byteValues04[0],
                                        byteValues04[1]
                                    };

                                    double realTimeValue = BitConverter.ToDouble(byteValues, 0);

                                    if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                    {
                                        dataPoint.RealTimeValue = realTimeValue;
                                        dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                    }
                                }

                                break;
                            }

                        case DataPointDataType.Bit:
                            {
                                double realTimeValue = registers[i].RegisterValue;

                                if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                {
                                    dataPoint.RealTimeValue = realTimeValue;
                                    dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                }
                                break;
                            }

                        default:
                            throw new ArgumentOutOfRangeException();

                    }
                }
            }

            return dataPointsWhoseRealTimeDataChanged;
        }

    }
}
