using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.Auxiliary;

namespace Azir.Modbus.Protocol.DataPoints
{
    public static class DataPointGrouper
    {
        #region 将寄存器号分成若干组

        /// <summary>
        /// 为读寄存器分组, 每一组DataPoint都是必须同时满足如下条件（缺一不可）：
        /// 1.设备地址相同；
        /// 2.读寄存器用的功能码相同；
        /// 3.相邻DataPoint的寄存器地址是连续的。
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns>
        /// 若干组DataPoint，每一组DataPoint都是：
        /// 1.设备地址相同；
        /// 2.读寄存器用的功能码相同；
        /// 3.相邻DataPoint的寄存器地址是连续的。
        /// </returns>
        public static List<List<DataPoint>> GroupingDataPointsForReadRegister(List<DataPoint> dataPoints)
        {
            List<List<DataPoint>> result = new List<List<DataPoint>>();

            Dictionary<int, List<DataPoint>> groupingByDeviceAddress =
                GroupingDataPointByDeviceAddress(dataPoints);

            foreach (var deviceAddressGroup in groupingByDeviceAddress)
            {
                Dictionary<DataPointType, List<DataPoint>> groupingByDataPointType =
                    GroupingDataPointByDataPointType(deviceAddressGroup.Value);

                foreach (var dataPointTypeGroup in groupingByDataPointType)
                {
                    List<List<DataPoint>> groupingByRegisterAddressIsContinuous =
                        GroupingDataPointsByRegisterAddressIsContinuous(dataPointTypeGroup.Value);

                    foreach (var registerAddressIsContinuouGroup in groupingByRegisterAddressIsContinuous)
                    {
                        result.Add(registerAddressIsContinuouGroup);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 为写寄存器分组,每一组DataPoint都是必须同时满足如下条件（缺一不可）：
        /// 1.设备地址相同；
        /// 2.写寄存器用的功能码相同；
        /// 3.写的值的数据类型相同（例如：数据类型都是int）
        /// 4.相邻DataPoint的寄存器地址是连续的。
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static List<List<DataPoint>> GroupingDataPointsForWriteRegister(List<DataPoint> dataPoints)
        {
            List<List<DataPoint>> result = new List<List<DataPoint>>();

            Dictionary<int, List<DataPoint>> groupingByDeviceAddress =
                GroupingDataPointByDeviceAddress(dataPoints);

            foreach (var deviceAddressGroup in groupingByDeviceAddress)
            {
                Dictionary<DataPointType, List<DataPoint>> groupingByDataPointType =
                    GroupingDataPointByDataPointType(deviceAddressGroup.Value);

                foreach (var dataPointTypeGroup in groupingByDataPointType)
                {
                    Dictionary<DataPointDataType, List<DataPoint>> groupingByDataType =
                        GroupingDataPointByDataType(dataPointTypeGroup.Value);

                    foreach (var dataTypeGroup in groupingByDataType)
                    {
                        List<List<DataPoint>> groupingByRegisterAddressIsContinuous =
                            GroupingDataPointsByRegisterAddressIsContinuous(dataTypeGroup.Value);

                        foreach (var registerAddressIsContinuouGroup in groupingByRegisterAddressIsContinuous)
                        {
                            result.Add(registerAddressIsContinuouGroup);
                        }
                    }
                }
            }

            return result;
        }

        private static Dictionary<int, List<DataPoint>> GroupingDataPointByDeviceAddress(List<DataPoint> dataPoints)
        {
            Dictionary<int, List<DataPoint>> result = new Dictionary<int, List<DataPoint>>();

            if (null == dataPoints)
            {
                return result;
            }

            Dictionary<int, int> kindOfdeviceAddress = new Dictionary<int, int>();
            int currentDeviceAddress = 0;

            foreach (var dataPoint in dataPoints)
            {
                //寻找有多少种DeviceAddress
                currentDeviceAddress = dataPoint.DeviceAddress;
                if (!kindOfdeviceAddress.ContainsKey(currentDeviceAddress))
                {
                    kindOfdeviceAddress.Add(currentDeviceAddress, 1);
                }
            }

            foreach (var deviceAddressKeyValue in kindOfdeviceAddress)
            {
                int deviceAddress = deviceAddressKeyValue.Key;
                List<DataPoint> dataPointsGroup = dataPoints.FindAll(p => p.DeviceAddress == deviceAddress);
                if (!result.ContainsKey(deviceAddress))
                {
                    result.Add(deviceAddress, dataPointsGroup);
                }
            }

            return result;
        }

        private static Dictionary<DataPointType, List<DataPoint>> GroupingDataPointByDataPointType(List<DataPoint> dataPoints)
        {
            Dictionary<DataPointType, List<DataPoint>> result = new Dictionary<DataPointType, List<DataPoint>>();

            if (null == dataPoints)
            {
                return result;
            }

            List<DataPoint> dataPointsReadByFunNum01 = new List<DataPoint>();
            List<DataPoint> dataPointsReadByFunNum03 = new List<DataPoint>();
            List<DataPoint> dataPointsWriteAndReadByFunNum01 = new List<DataPoint>();
            List<DataPoint> dataPointsWriteAndReadByFunNum03 = new List<DataPoint>();

            foreach (var dataPoint in dataPoints)
            {
                switch (dataPoint.DataPointType)
                {
                    case DataPointType.ReadByFunNum01:
                        dataPointsReadByFunNum01.Add(dataPoint);
                        break;

                    case DataPointType.ReadByFunNum03:
                        dataPointsReadByFunNum03.Add(dataPoint);
                        break;

                    case DataPointType.WriteAndReadByFunNum01:
                        dataPointsWriteAndReadByFunNum01.Add(dataPoint);
                        break;

                    case DataPointType.WriteAndReadByFunNum03:
                        dataPointsWriteAndReadByFunNum03.Add(dataPoint);
                        break;

                    default:
                        break;
                }

            }

            result.Add(DataPointType.ReadByFunNum01, dataPointsReadByFunNum01);
            result.Add(DataPointType.ReadByFunNum03, dataPointsReadByFunNum03);
            result.Add(DataPointType.WriteAndReadByFunNum01, dataPointsWriteAndReadByFunNum01);
            result.Add(DataPointType.WriteAndReadByFunNum03, dataPointsWriteAndReadByFunNum03);

            return result;
        }

        private static Dictionary<DataPointDataType, List<DataPoint>> GroupingDataPointByDataType(List<DataPoint> dataPoints)
        {
            Dictionary<DataPointDataType, List<DataPoint>> result = new Dictionary<DataPointDataType, List<DataPoint>>();

            if (null == dataPoints)
            {
                return result;
            }

            List<DataPoint> dataPointsS16 = new List<DataPoint>();
            List<DataPoint> dataPointsU16 = new List<DataPoint>();
            List<DataPoint> dataPointsS32 = new List<DataPoint>();
            List<DataPoint> dataPointsU32 = new List<DataPoint>();
            List<DataPoint> dataPointsS64 = new List<DataPoint>();
            List<DataPoint> dataPointsU64 = new List<DataPoint>();
            List<DataPoint> dataPointsF32 = new List<DataPoint>();
            List<DataPoint> dataPointsD64 = new List<DataPoint>();
            List<DataPoint> dataPointsBit = new List<DataPoint>();

            foreach (var dataPoint in dataPoints)
            {
                switch (dataPoint.DataPointDataType)
                {
                    case DataPointDataType.S16:
                        dataPointsS16.Add(dataPoint);
                        break;
                    case DataPointDataType.U16:
                        dataPointsU16.Add(dataPoint);
                        break;
                    case DataPointDataType.S32:
                        dataPointsS32.Add(dataPoint);
                        break;
                    case DataPointDataType.U32:
                        dataPointsU32.Add(dataPoint);
                        break;
                    case DataPointDataType.S64:
                        dataPointsS64.Add(dataPoint);
                        break;
                    case DataPointDataType.U64:
                        dataPointsU64.Add(dataPoint);
                        break;
                    case DataPointDataType.F32:
                        dataPointsF32.Add(dataPoint);
                        break;
                    case DataPointDataType.D64:
                        dataPointsD64.Add(dataPoint);
                        break;
                    case DataPointDataType.Bit:
                        dataPointsBit.Add(dataPoint);
                        break;
                    default:
                        break;
                }
            }
            result.Add(DataPointDataType.S16, dataPointsS16);
            result.Add(DataPointDataType.U16, dataPointsU16);
            result.Add(DataPointDataType.S32, dataPointsS32);
            result.Add(DataPointDataType.U32, dataPointsU32);
            result.Add(DataPointDataType.S64, dataPointsS64);
            result.Add(DataPointDataType.U64, dataPointsU64);
            result.Add(DataPointDataType.F32, dataPointsF32);
            result.Add(DataPointDataType.D64, dataPointsD64);
            result.Add(DataPointDataType.Bit, dataPointsBit);

            return result;
        }

        /// 根据DataPints的RegisterAddress分组，
        /// 每个组内的RegisterAddress都是连续的连续，例如：
        /// （1,2,3） (120,121) (300,301,302,303)
        /// </summary>
        /// <param name="dataPointsDataPints">
        /// 
        /// </param>
        private static List<List<DataPoint>> GroupingDataPointsByRegisterAddressIsContinuous(List<DataPoint> dataPointsDataPints)
        {
            List<List<DataPoint>> result = new List<List<DataPoint>>();

            if (null == dataPointsDataPints || dataPointsDataPints.Count < 1)
            {
                return result;
            }

            //先按RegisterAddress升序排序
            dataPointsDataPints.Sort(new DataPointRegisterAddressCompare());

            //小于等于-2 ，因为currDataPointStartRegisterAddress - 1 >=-1
            int previousDataPointEndRegisterAddress = -100;
            List<DataPoint> dataPointsGroup = null;

            for (int i = 0; i < dataPointsDataPints.Count; i++)
            {
                int currDataPointStartRegisterAddress = dataPointsDataPints[i].StartRegisterAddress;

                if (previousDataPointEndRegisterAddress != currDataPointStartRegisterAddress - 1)
                {
                    if (null != dataPointsGroup)
                    {
                        result.Add(dataPointsGroup);
                    }

                    dataPointsGroup = new List<DataPoint>();
                }

                previousDataPointEndRegisterAddress = currDataPointStartRegisterAddress
                    + RegisterCountCalculator.GetRegisterCount(dataPointsDataPints[i].DataPointDataType) - 1;

                dataPointsGroup.Add(dataPointsDataPints[i]);

                if (i == dataPointsDataPints.Count - 1)
                {
                    result.Add(dataPointsGroup);
                }
            }

            return result;
        }

        #endregion
    }
}
