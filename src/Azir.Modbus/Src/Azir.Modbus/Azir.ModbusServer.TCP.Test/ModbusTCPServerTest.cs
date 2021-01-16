using System;
using System.Collections.Generic;
using System.Diagnostics;
using Azir.Modbus.Protocol;
using Azir.Modbus.Protocol.DataPoints;
using Azir.Modbus.Protocol.DataReponse;
using Azir.Modbus.TCP;
using Azir.ModbusServer.TCP.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Azir.ModbusServer.TCP.Test
{
    [TestClass]
    public class ModbusTCPServerTest
    {

        private static string modbusConfigFile = @"../../Confing/ModbusConfig.xml";

        /// <summary>
        /// 获取数据点的实时数据
        /// </summary>
        [TestMethod]
        public void GetDataPointRealValueTest()
        {
            ModbusTCPServer modbusTCPServer = new ModbusTCPServer();
            modbusTCPServer.InitializeFromConfigFile(modbusConfigFile);

             List<DataPoint> allDataPointsWhoseRealTimeDataChanged = new List<DataPoint>();
            foreach (var modbusUint in modbusTCPServer.ModbusUnits)
            {
                if (modbusUint.Connector.Connect())
                {
                    int i = 0;
                    foreach (var readRegisterCommand in modbusUint.AllReadRegisterCommands)
                    {
                        ++i;
                        var recvBytes = modbusUint.Connector.Send(readRegisterCommand.ReadCommand);
                        var sendMsg = string.Format("{0}SendBytes:{1} ", i, readRegisterCommand.ReadCommand.ToString());

                        Debug.WriteLine(sendMsg);

                        if (recvBytes != null)
                        {
                            var recvMsg = string.Format("{0}RecvBytes:{1} ", i, recvBytes.ToString());
                            Debug.WriteLine(recvMsg);

                            AnalyzeRecivedDataReponse reponse = ModbusTCP.AnalyzeRecivedDataStatic(modbusUint.DataAnalyzeMode, readRegisterCommand.ReadCommand, recvBytes);
                            if (reponse.ModbusReponseSuccess && reponse.AnalyzeRecivedDataSuccess)
                            {
                                List<DataPoint> dataPointsWhoseRealTimeDataChanged = ModbusTCP.SetDataPointValueFromRegisterValue(reponse.Registers, modbusUint.AllDataPoints);
                                allDataPointsWhoseRealTimeDataChanged.AddRange(dataPointsWhoseRealTimeDataChanged);
                            }
                        }
                        else
                        {
                            Debug.WriteLine(i + ".RecvBytes: null");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 设置数据点的值
        /// </summary>
        [TestMethod]
        public void SetDataPointRealValueByTest()
        {
            ModbusTCPServer modbusTCPServer = new ModbusTCPServer();
            modbusTCPServer.InitializeFromConfigFile(modbusConfigFile);

            foreach (var modbusUint in modbusTCPServer.ModbusUnits)
            {
                List<DataPoint> allDataPointsWhoseRealTimeDataChangeds = new List<DataPoint>();
                List<WriteRegisterCommand> allWriterRegisterCommands = new List<WriteRegisterCommand>();

                List<DataPoint> canWriteDataPoints =
                    modbusUint.AllDataPoints.FindAll(p => p.DataPointType == DataPointType.WriteAndReadByFunNum01
                                                               || p.DataPointType == DataPointType.WriteAndReadByFunNum03);

                #region 模拟数据

                foreach (var writeDataPoint in canWriteDataPoints)
                {
                    if (writeDataPoint.DataPointType == DataPointType.WriteAndReadByFunNum01)
                    {
                        var dataPointNo = Convert.ToInt32(writeDataPoint.Number);
                        if (dataPointNo % 2 == 0)
                        {
                            writeDataPoint.ValueToSet = 1;
                        }
                        else
                        {
                            writeDataPoint.ValueToSet = 0;
                        }
                    }
                    else if (writeDataPoint.DataPointType == DataPointType.WriteAndReadByFunNum03)
                    {
                        writeDataPoint.ValueToSet = Convert.ToDouble(writeDataPoint.Number);
                    }
                }


                #endregion

                List<List<byte>> requestBytes = ModbusTCP.CreateWriteRegisterCommands(modbusUint.DataAnalyzeMode, canWriteDataPoints);

                foreach (var requestByte in requestBytes)
                {
                    WriteRegisterCommand writeRegisterCommand = new WriteRegisterCommand();
                    writeRegisterCommand.WriteCommand = requestByte;

                    allWriterRegisterCommands.Add(writeRegisterCommand);
                }

                if (modbusUint.Connector.Connect())
                {
                    int i = 0;
                    foreach (var writeRegisterCommand in allWriterRegisterCommands)
                    {
                        ++i;
                        var recvBytes = modbusUint.Connector.Send(writeRegisterCommand.WriteCommand);
                        if (recvBytes != null)
                        {
                            AnalyzeRecivedDataReponse reponse = ModbusTCP.AnalyzeRecivedDataStatic(modbusUint.DataAnalyzeMode, writeRegisterCommand.WriteCommand, recvBytes);
                            if (reponse.ModbusReponseSuccess && reponse.AnalyzeRecivedDataSuccess)
                            {
                                var dataPointsWhoseRealTimeDataChanged = ModbusTCP.SetDataPointValueFromRegisterValue(reponse.Registers, modbusUint.AllDataPoints);

                                allDataPointsWhoseRealTimeDataChangeds.AddRange(dataPointsWhoseRealTimeDataChanged);
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void SetDataPointRealValueByFunNun16Test()
        {
            ModbusTCPServer modbusTCPServer = new ModbusTCPServer();
            modbusTCPServer.InitializeFromConfigFile(modbusConfigFile);

            foreach (var modbusUint in modbusTCPServer.ModbusUnits)
            {
                List<DataPoint> dataPointsWhoseRealTimeDataChanged = new List<DataPoint>();

                List<DataPoint> dataPoints = new List<DataPoint>();

                dataPoints.Add(new DataPoint()
                {
                    DeviceAddress = 1,
                    StartRegisterAddress = 83,
                    DataPointType = DataPointType.WriteAndReadByFunNum03,
                    DataPointDataType = DataPointDataType.F32,
                    RealTimeValue = -1,
                    ValueToSet = 101.7
                });

                List<List<byte>> writeRegisterCommandBytes = ModbusTCP.CreateWriteRegisterCommands(DataAnalyzeMode.DataHighToLow, dataPoints);

                if (modbusUint.Connector.Connect())
                {
                    foreach (var writeBytes in writeRegisterCommandBytes)
                    {
                        var recvBytes = modbusUint.Connector.Send(writeBytes);
                        if (recvBytes != null)
                        {
                            AnalyzeRecivedDataReponse reponse = ModbusTCP.AnalyzeRecivedDataStatic(modbusUint.DataAnalyzeMode, writeBytes, recvBytes);
                            if (reponse.ModbusReponseSuccess && reponse.AnalyzeRecivedDataSuccess)
                            {
                                dataPointsWhoseRealTimeDataChanged = ModbusTCP.SetDataPointValueFromRegisterValue(reponse.Registers, dataPoints);
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void SetDataPointRealValueByFunNun03Test()
        {
            ModbusTCPServer modbusTCPServer = new ModbusTCPServer();
            modbusTCPServer.InitializeFromConfigFile(modbusConfigFile);

            foreach (var modbusUint in modbusTCPServer.ModbusUnits)
            {
                List<DataPoint> dataPointsWhoseRealTimeDataChanged = new List<DataPoint>();

                List<DataPoint> dataPoints = new List<DataPoint>();

                dataPoints.Add(new DataPoint()
                {
                    DeviceAddress = 1,
                    StartRegisterAddress = 83,
                    DataPointType = DataPointType.WriteAndReadByFunNum03,
                    DataPointDataType = DataPointDataType.F32,
                    RealTimeValue = -1,
                });

                List<List<byte>> registerCommandBytes = ModbusTCP.CreateReadRegisterCommands(dataPoints);

                if (modbusUint.Connector.Connect())
                {
                    foreach (var writeBytes in registerCommandBytes)
                    {
                        var recvBytes = modbusUint.Connector.Send(writeBytes);
                        if (recvBytes != null)
                        {
                            AnalyzeRecivedDataReponse reponse = ModbusTCP.AnalyzeRecivedDataStatic(modbusUint.DataAnalyzeMode, writeBytes, recvBytes);
                            if (reponse.ModbusReponseSuccess && reponse.AnalyzeRecivedDataSuccess)
                            {
                                dataPointsWhoseRealTimeDataChanged = ModbusTCP.SetDataPointValueFromRegisterValue(reponse.Registers, dataPoints);
                            }
                        }
                    }
                }
            }


        }
    }
}
