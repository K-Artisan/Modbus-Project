﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol;
using Azir.Modbus.Protocol.Auxiliary;
using Azir.Modbus.Protocol.DataPoints;
using Azir.Modbus.Protocol.FuncitonNum;
using Azir.Modbus.Protocol.FuncitonNum.CustomerRequest;

namespace Azir.Modbus.TCP.TCPRequest
{
    public static class TCPRequestCommandByteStreamCreator
    {
        #region 寄存器的读命令转换为字节流

        /// <summary>
        /// 生成modbus请求帧:用于读取寄存器值
        /// </summary>
        /// <param name="dataPointsataPints">
        /// dataPointsataPints中的DataPoint必须同时满足如下条件（缺一不可）；
        /// 1.设备地址相同；
        /// 2.读寄存器用的功能码相同；
        /// 3.相邻DataPoint的寄存器地址是连续的。
        /// </param>
        /// <returns>modbus请求帧</returns>
        public static List<List<byte>> CreateRequestCommandByteStreamForReadRegisterBy(List<DataPoint> dataPointsataPints)
        {
            List<List<byte>> requestCommandByteStreams = new List<List<byte>>();

            DataPoint dataPoint = dataPointsataPints.First();
            switch (dataPoint.DataPointType)
            {
                case DataPointType.ReadByFunNum01:
                case DataPointType.WriteAndReadByFunNum01:
                    {
                        requestCommandByteStreams =
                            CreateRequestCommandByteStreamForFunNum01(dataPointsataPints);
                        break;
                    }

                case DataPointType.ReadByFunNum03:
                case DataPointType.WriteAndReadByFunNum03:
                    {
                        requestCommandByteStreams =
                            CreateRequestCommandByteStreamForFunNum03(dataPointsataPints);
                        break;
                    }

                default:
                    break;
            }

            return requestCommandByteStreams;
        }

        private static List<List<byte>> CreateRequestCommandByteStreamForFunNum01(List<DataPoint> dataPoints)
        {
            List<List<byte>> requestCommandByteStreams = new List<List<byte>>();

            DataPoint dataPoint = dataPoints.First();
            int theFirstRegisterAddress = dataPoints.First().StartRegisterAddress;
            int theLastRegisterAddress = dataPoints.Last().StartRegisterAddress
                + RegisterCountCalculator.GetRegisterCount(dataPoints.Last().DataPointDataType) - 1;
            int registerCount = theLastRegisterAddress - theFirstRegisterAddress + 1;

            FunNum01CustomerRequestData customerRequestData = new FunNum01CustomerRequestData();
            customerRequestData.DeviceAddress = (byte)dataPoint.DeviceAddress;
            customerRequestData.FunctionNum = FunctionNumType.FunctionNum01;
            customerRequestData.StartingRegister = (ushort)theFirstRegisterAddress;
            customerRequestData.NumOfRegisterToRead = (ushort)registerCount;
            requestCommandByteStreams =
                TCPRequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);

            return requestCommandByteStreams;
        }

        private static List<List<byte>> CreateRequestCommandByteStreamForFunNum03(List<DataPoint> dataPoints)
        {
            List<List<byte>> requestCommandByteStreams = new List<List<byte>>(); ;
            DataPoint firstDataPoint = dataPoints.First();

            int theFirstRegisterAddress = dataPoints.First().StartRegisterAddress;
            int theLastRegisterAddress = dataPoints.Last().StartRegisterAddress
                  + RegisterCountCalculator.GetRegisterCount(dataPoints.Last().DataPointDataType) - 1;
            int registerCount = theLastRegisterAddress - theFirstRegisterAddress + 1;

            FunNum03CustomerRequestData customerRequestData = new FunNum03CustomerRequestData();
            customerRequestData.DeviceAddress = (byte)firstDataPoint.DeviceAddress;
            customerRequestData.FunctionNum = FunctionNumType.FunctionNum03;
            customerRequestData.StartingRegisterAddress = (ushort)theFirstRegisterAddress;
            customerRequestData.NumOfRegisterToRead = (ushort)registerCount;

            requestCommandByteStreams =
                TCPRequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);
            return requestCommandByteStreams;
        }

        #endregion

        #region 寄存器的写命令转化为字节流

        /// <summary>
        /// 生成modbus请求帧:用于写取寄存器值
        /// </summary>
        /// <param name="dataPoints">
        /// dataPointsataPints中的DataPoint必须同时满足如下条件（缺一不可）；
        /// 1.设备地址相同；
        /// 2.写寄存器用的功能码相同；
        /// 3.写的值的数据类型相同（例如：数据类型都是int）
        /// 4.相邻DataPoint的寄存器地址是连续的。
        /// </param>
        /// <returns>modbus请求帧</returns>
        public static List<List<byte>> CreateRequestCommandByteStreamForWriteRegisterBy(DataAnalyzeMode dataAnalyzeMode, List<DataPoint> dataPoints, FunctionNumType functionNumType)
        {
            List<List<byte>> requestCommandByteStreams = new List<List<byte>>();

            if (null == dataPoints || dataPoints.Count < 1)
            {
                return requestCommandByteStreams;
            }

            switch (functionNumType)
            {
                case FunctionNumType.FunctionNum05:
                    requestCommandByteStreams = CreateRequestCommandByteStreamForFunNum05(dataPoints);
                    break;
                case FunctionNumType.FunctionNum06:
                    requestCommandByteStreams = CreateRequestCommandByteStreamForFunNum06(dataAnalyzeMode, dataPoints);
                    break;
                case FunctionNumType.FunctionNum15:
                    requestCommandByteStreams = CreateRequestCommandByteStreamForFunNum15(dataPoints);
                    break;
                case FunctionNumType.FunctionNum16:
                    requestCommandByteStreams = CreateRequestCommandByteStreamForFunNum16(dataAnalyzeMode, dataPoints);
                    break;

                default:
                    break;

            }

            return requestCommandByteStreams;
        }

        /// <summary>
        /// 生成modbus请求帧:用于写取寄存器值
        /// </summary>
        /// <param name="dataPoints">
        /// dataPointsataPints中的DataPoint必须同时满足如下条件（缺一不可）；
        /// 1.设备地址相同；
        /// 2.写寄存器用的功能码相同；
        /// 3.写的值的数据类型相同（例如：数据类型都是int）
        /// 4.相邻DataPoint的寄存器地址是连续的。
        /// </param>
        /// <returns>modbus请求帧</returns>
        public static List<List<byte>> CreateRequestCommandByteStreamForWriteRegisterBy(DataAnalyzeMode dataAnalyzeMode, List<DataPoint> dataPoints)
        {
            List<List<byte>> requestCommandByteStreams = new List<List<byte>>();

            if (null == dataPoints || dataPoints.Count < 1)
            {
                return requestCommandByteStreams;
            }
            DataPoint firstDataPoint = dataPoints.First();

            /*
               根据DataPointDataType来判断使用功能码，有待商榷。
             * 根据DataPointType来判断使用功能码是否才是正确的？
             */
            if (DataPointDataType.Bit == firstDataPoint.DataPointDataType)
            {
                if (dataPoints.Count == 1)
                {
                    requestCommandByteStreams = CreateRequestCommandByteStreamForFunNum05(dataPoints);
                }
                else
                {
                    requestCommandByteStreams = CreateRequestCommandByteStreamForFunNum15(dataPoints);
                }
            }
            else
            {
                if (dataPoints.Count == 1)
                {
                    requestCommandByteStreams = CreateRequestCommandByteStreamForFunNum06(dataAnalyzeMode, dataPoints);
                }
                else
                {
                    requestCommandByteStreams = CreateRequestCommandByteStreamForFunNum16(dataAnalyzeMode, dataPoints);
                }
            }

            return requestCommandByteStreams;
        }

        private static List<List<byte>> CreateRequestCommandByteStreamForFunNum05(List<DataPoint> dataPoints)
        {
            List<List<byte>> requestCommandByteStreams = new List<List<byte>>();
            FunNum05CustomerRequestData customerRequestData = new FunNum05CustomerRequestData();

            //功能码05是设置单个线圈的值，所以只有一个寄存器
            DataPoint firstDataPoint = dataPoints.First();
            int theFirstRegisterAddress = dataPoints.First().StartRegisterAddress;

            customerRequestData.DeviceAddress = (byte)firstDataPoint.DeviceAddress;
            customerRequestData.FunctionNum = FunctionNumType.FunctionNum05;
            customerRequestData.CoilAddress = (ushort)theFirstRegisterAddress;

            if (Math.Abs(firstDataPoint.ValueToSet) > 0)
            {
                customerRequestData.ON = true;
            }
            else
            {
                customerRequestData.ON = false;
            }

            requestCommandByteStreams =
                TCPRequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);

            return requestCommandByteStreams;
        }

        /// <summary>
        /// 因为：功能码06是设置单个寄存器（占两字节，所以T的类型只能为short ushort）的值
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        private static List<List<byte>> CreateRequestCommandByteStreamForFunNum06(DataAnalyzeMode dataAnalyzeMode, List<DataPoint> dataPoints)
        {
            List<List<byte>> requestCommandByteStreams = new List<List<byte>>();

            DataPoint firstDataPoint = dataPoints.First();

            int theFirstRegisterAddress = dataPoints.First().StartRegisterAddress;

            switch (firstDataPoint.DataPointDataType)
            {
                case DataPointDataType.S16:
                    {
                        FunNum06CustomerRequestData<short> customerRequestData =
                            new FunNum06CustomerRequestData<short>();
                        customerRequestData.DataAnalyzeMode = dataAnalyzeMode;

                        customerRequestData.DeviceAddress = (byte)firstDataPoint.DeviceAddress;
                        customerRequestData.FunctionNum = FunctionNumType.FunctionNum06;
                        customerRequestData.RegisterAddress = (ushort)theFirstRegisterAddress;
                        customerRequestData.PresetData = (short)firstDataPoint.ValueToSet;

                        requestCommandByteStreams =
                             TCPRequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);
                        break;
                    }
                case DataPointDataType.U16:
                    {
                        FunNum06CustomerRequestData<ushort> customerRequestData =
                            new FunNum06CustomerRequestData<ushort>();
                        customerRequestData.DataAnalyzeMode = dataAnalyzeMode;


                        customerRequestData.DeviceAddress = (byte)firstDataPoint.DeviceAddress;
                        customerRequestData.FunctionNum = FunctionNumType.FunctionNum06;
                        customerRequestData.RegisterAddress = (ushort)theFirstRegisterAddress;
                        customerRequestData.PresetData = (ushort)firstDataPoint.ValueToSet;

                        requestCommandByteStreams =
                             TCPRequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);
                        break;
                    }
                case DataPointDataType.S32:
                case DataPointDataType.U32:
                case DataPointDataType.S64:
                case DataPointDataType.U64:
                case DataPointDataType.F32:
                case DataPointDataType.D64:
                    //因为：功能码06是设置单个寄存器（占两字节，所以T的类型只能为short ushort）的值
                    //其他T类型只能由功能码16（设置多个寄存器的值）去处理
                    requestCommandByteStreams = CreateRequestCommandByteStreamForFunNum16(dataAnalyzeMode, dataPoints);
                    break;
                case DataPointDataType.Bit:
                    break;
                default:
                    break;
            }

            return requestCommandByteStreams;
        }

        private static List<List<byte>> CreateRequestCommandByteStreamForFunNum15(List<DataPoint> dataPoints)
        {
            List<List<byte>> requestCommandByteStreams = new List<List<byte>>();
            FunNum15CustomerRequestData customerRequestData = new FunNum15CustomerRequestData();

            DataPoint firstDataPoint = dataPoints.First();
            int theFirstRegisterAddress = dataPoints.First().StartRegisterAddress;
            int theLastRegisterAddress = dataPoints.Last().StartRegisterAddress
                  + RegisterCountCalculator.GetRegisterCount(dataPoints.Last().DataPointDataType) - 1;
            int registerCount = theLastRegisterAddress - theFirstRegisterAddress + 1;

            customerRequestData.DeviceAddress = (byte)firstDataPoint.DeviceAddress;
            customerRequestData.FunctionNum = FunctionNumType.FunctionNum15;
            customerRequestData.StartingCoilAddress = (ushort)theFirstRegisterAddress;
            customerRequestData.NumOfCoilToForce = (ushort)registerCount;

            foreach (var dataPoint in dataPoints)
            {
                if (Math.Abs(dataPoint.ValueToSet) > 0)
                {
                    customerRequestData.ForceData.Add(true);
                }
                else
                {
                    customerRequestData.ForceData.Add(false);
                }
            }

            requestCommandByteStreams =
                TCPRequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);

            return requestCommandByteStreams;
        }

        private static List<List<byte>> CreateRequestCommandByteStreamForFunNum16(DataAnalyzeMode dataAnalyzeMode, List<DataPoint> dataPoints)
        {
            List<List<byte>> requestCommandByteStreams = new List<List<byte>>();

            DataPoint firstDataPoint = dataPoints.First();
            int theFirstRegisterAddress = dataPoints.First().StartRegisterAddress;

            switch (firstDataPoint.DataPointDataType)
            {
                case DataPointDataType.S16:
                    {
                        FunNum16CustomerRequestData<short> customerRequestData =
                            new FunNum16CustomerRequestData<short>();
                        customerRequestData.DataAnalyzeMode = dataAnalyzeMode;

                        customerRequestData.DeviceAddress = (byte)firstDataPoint.DeviceAddress;
                        customerRequestData.FunctionNum = FunctionNumType.FunctionNum16;
                        customerRequestData.StartingRegisterAddress = (ushort)theFirstRegisterAddress;

                        foreach (var dataPoint in dataPoints)
                        {
                            customerRequestData.PresetData.Add((short)dataPoint.ValueToSet);
                        }

                        requestCommandByteStreams =
                             TCPRequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);
                        break;
                    }
                case DataPointDataType.U16:
                    {
                        FunNum16CustomerRequestData<ushort> customerRequestData =
                            new FunNum16CustomerRequestData<ushort>();
                        customerRequestData.DataAnalyzeMode = dataAnalyzeMode;

                        customerRequestData.DeviceAddress = (byte)firstDataPoint.DeviceAddress;
                        customerRequestData.FunctionNum = FunctionNumType.FunctionNum16;
                        customerRequestData.StartingRegisterAddress = (ushort)theFirstRegisterAddress;

                        foreach (var dataPoint in dataPoints)
                        {
                            customerRequestData.PresetData.Add((ushort)dataPoint.ValueToSet);
                        }

                        requestCommandByteStreams =
                             TCPRequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);
                        break;
                    }
                case DataPointDataType.S32:
                    {
                        FunNum16CustomerRequestData<int> customerRequestData =
                            new FunNum16CustomerRequestData<int>();
                        customerRequestData.DataAnalyzeMode = dataAnalyzeMode;

                        customerRequestData.DeviceAddress = (byte)firstDataPoint.DeviceAddress;
                        customerRequestData.FunctionNum = FunctionNumType.FunctionNum16;
                        customerRequestData.StartingRegisterAddress = (ushort)theFirstRegisterAddress;

                        foreach (var dataPoint in dataPoints)
                        {
                            customerRequestData.PresetData.Add((int)dataPoint.ValueToSet);
                        }

                        requestCommandByteStreams =
                             TCPRequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);
                        break;
                    }
                case DataPointDataType.U32:
                    {
                        FunNum16CustomerRequestData<uint> customerRequestData =
                            new FunNum16CustomerRequestData<uint>();
                        customerRequestData.DataAnalyzeMode = dataAnalyzeMode;

                        customerRequestData.DeviceAddress = (byte)firstDataPoint.DeviceAddress;
                        customerRequestData.FunctionNum = FunctionNumType.FunctionNum16;
                        customerRequestData.StartingRegisterAddress = (ushort)theFirstRegisterAddress;

                        foreach (var dataPoint in dataPoints)
                        {
                            customerRequestData.PresetData.Add((uint)dataPoint.ValueToSet);
                        }

                        requestCommandByteStreams =
                             TCPRequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);
                        break;
                    }
                case DataPointDataType.S64:
                    {
                        FunNum16CustomerRequestData<long> customerRequestData =
                            new FunNum16CustomerRequestData<long>();
                        customerRequestData.DataAnalyzeMode = dataAnalyzeMode;

                        customerRequestData.DeviceAddress = (byte)firstDataPoint.DeviceAddress;
                        customerRequestData.FunctionNum = FunctionNumType.FunctionNum16;
                        customerRequestData.StartingRegisterAddress = (ushort)theFirstRegisterAddress;

                        foreach (var dataPoint in dataPoints)
                        {
                            customerRequestData.PresetData.Add((long)dataPoint.ValueToSet);
                        }

                        requestCommandByteStreams =
                             TCPRequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);
                        break;
                    }
                case DataPointDataType.U64:
                    {
                        FunNum16CustomerRequestData<ulong> customerRequestData =
                            new FunNum16CustomerRequestData<ulong>();
                        customerRequestData.DataAnalyzeMode = dataAnalyzeMode;

                        customerRequestData.DeviceAddress = (byte)firstDataPoint.DeviceAddress;
                        customerRequestData.FunctionNum = FunctionNumType.FunctionNum16;
                        customerRequestData.StartingRegisterAddress = (ushort)theFirstRegisterAddress;

                        foreach (var dataPoint in dataPoints)
                        {
                            customerRequestData.PresetData.Add((ulong)dataPoint.ValueToSet);
                        }

                        requestCommandByteStreams =
                             TCPRequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);
                        break;
                    }
                case DataPointDataType.F32:
                    {
                        FunNum16CustomerRequestData<float> customerRequestData =
                            new FunNum16CustomerRequestData<float>();
                        customerRequestData.DataAnalyzeMode = dataAnalyzeMode;

                        customerRequestData.DeviceAddress = (byte)firstDataPoint.DeviceAddress;
                        customerRequestData.FunctionNum = FunctionNumType.FunctionNum16;
                        customerRequestData.StartingRegisterAddress = (ushort)theFirstRegisterAddress;

                        foreach (var dataPoint in dataPoints)
                        {
                            customerRequestData.PresetData.Add((float)dataPoint.ValueToSet);
                        }

                        requestCommandByteStreams =
                             TCPRequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);
                        break;
                    }
                case DataPointDataType.D64:
                    {
                        FunNum16CustomerRequestData<double> customerRequestData =
                            new FunNum16CustomerRequestData<double>();
                        customerRequestData.DataAnalyzeMode = dataAnalyzeMode;

                        customerRequestData.DeviceAddress = (byte)firstDataPoint.DeviceAddress;
                        customerRequestData.FunctionNum = FunctionNumType.FunctionNum16;
                        customerRequestData.StartingRegisterAddress = (ushort)theFirstRegisterAddress;

                        foreach (var dataPoint in dataPoints)
                        {
                            customerRequestData.PresetData.Add((double)dataPoint.ValueToSet);
                        }

                        requestCommandByteStreams =
                             TCPRequesCommandCreator.CreateRequestCommandByteStream(customerRequestData);
                        break;
                    }
                case DataPointDataType.Bit:
                    break;
                default:
                    break;
            }


            return requestCommandByteStreams;
        }

        #endregion
    }
}
