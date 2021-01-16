using System;
using System.Collections.Generic;
using System.Diagnostics;
using Azir.Modbus.Protocol;
using Azir.Modbus.Protocol.DataPoints;
using Azir.Modbus.Protocol.DataReponse;
using Azir.Modbus.RTU;
using Azir.ModbusServer.RTU.Event;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Azir.ModbusServer.RTU.Test
{
    [TestClass]
    public class ModbusRTUServerTest
    {
        private static string modbusConfigFile = @"../../Confing/ModbusConfig.xml";
        private static string serialPortConfigFile = @"../../Confing/SerialPortConfig.xml";
        private List<byte> currentRequestByteData = null;
        private List<byte> currentRecivedByteData = null;
        private bool canSendNextRequestCommandBytes = true;     //用于保证：请求必须有响应数据后才能再发下一条请求
        private DataAnalyzeMode dataAnalyzeMode = Modbus.Protocol.DataAnalyzeMode.DataHighToLow;
        private List<DataPoint> allDataPoints = null;

        /// <summary>
        /// 获取数据点的实时数据
        /// </summary>
        [TestMethod]
        public void GetDataPointRealValueTest()
        {
            ModbusRTUServer modbusRTUServer = new ModbusRTUServer();
            modbusRTUServer.InitializeFromConfigFile(modbusConfigFile, serialPortConfigFile);

            modbusRTUServer.SerialPort.OnCurrentRequestDataChanged += new EventHandler<RequstDataEventArgs>(SerialPort_OnRequestDataChanged);
            modbusRTUServer.SerialPort.OnCurrentReceiveDataChanged += new EventHandler<ReceiveDataEventArgs>(SerialPort_OnReceiveDataChanged);

            dataAnalyzeMode = modbusRTUServer.DataAnalyzeMode;
            allDataPoints = modbusRTUServer.AllDataPoints;

            if (modbusRTUServer.SerialPort.TryOpenSerialPort())
            {
                foreach (var readRegisterCommand in modbusRTUServer.AllReadRegisterCommands)
                {
                    modbusRTUServer.SerialPort.WriteSerialPort(readRegisterCommand.ReadCommand.ToArray());
                }
            }
        }

        [TestMethod]
        public void SetDataPointRealValueByFunNun16Test()
        {
            ModbusRTUServer modbusRTUServer = new ModbusRTUServer();
            modbusRTUServer.InitializeFromConfigFile(modbusConfigFile, serialPortConfigFile);
            List<DataPoint> dataPoints = new List<DataPoint>();

            modbusRTUServer.SerialPort.OnCurrentRequestDataChanged += new EventHandler<RequstDataEventArgs>(SerialPort_OnRequestDataChanged);
            modbusRTUServer.SerialPort.OnCurrentReceiveDataChanged += new EventHandler<ReceiveDataEventArgs>(SerialPort_OnReceiveDataChanged);

            dataAnalyzeMode = modbusRTUServer.DataAnalyzeMode;
            allDataPoints = modbusRTUServer.AllDataPoints;

            dataPoints.Add(new DataPoint()
            {
                DeviceAddress = 1,
                StartRegisterAddress = 83,
                DataPointType = DataPointType.WriteAndReadByFunNum03,
                DataPointDataType = DataPointDataType.F32,
                RealTimeValue = -1,
                ValueToSet = 101.7
            });

            List<List<byte>> writeRegisterCommandBytes = ModbusRTU.CreateWriteRegisterCommands(DataAnalyzeMode.DataHighToLow, dataPoints);

            if (modbusRTUServer.SerialPort.TryOpenSerialPort())
            {
                foreach (var writeBytes in writeRegisterCommandBytes)
                {
                    modbusRTUServer.SerialPort.WriteSerialPort(writeBytes.ToArray());
                }
            }
        }

        private void SerialPort_OnRequestDataChanged(object sender, RequstDataEventArgs e)
        {
            List<byte> requstData = e.RequstData;
            this.currentRequestByteData = requstData;
            this.canSendNextRequestCommandBytes = false;
        }

        private void SerialPort_OnReceiveDataChanged(object sender, ReceiveDataEventArgs e)
        {
            List<byte> recevideData = e.ReceiveData;
            this.canSendNextRequestCommandBytes = true;

            if (null != recevideData)
            {
                this.currentRecivedByteData = recevideData;
                AnalyzeRecivedDataReponse reponse = ModbusRTU.AnalyzeRecivedDataStatic(this.dataAnalyzeMode, this.currentRequestByteData, recevideData);
                if (reponse.ModbusReponseSuccess && reponse.AnalyzeRecivedDataSuccess)
                {
                    var dataPointsWhoseRealTimeDataChanged = ModbusRTU.SetDataPointValueFromRegisterValue(reponse.Registers, this.allDataPoints);
                }
            }
        }

        [TestMethod]
        public void SetDataPointRealValueByFunNun03Test()
        {
            ModbusRTUServer modbusRTUServer = new ModbusRTUServer();
            modbusRTUServer.InitializeFromConfigFile(modbusConfigFile, serialPortConfigFile);
            List<DataPoint> dataPointsWhoseRealTimeDataChanged = new List<DataPoint>();

            modbusRTUServer.SerialPort.OnCurrentRequestDataChanged += new EventHandler<RequstDataEventArgs>(SerialPort_OnRequestDataChanged);
            modbusRTUServer.SerialPort.OnCurrentReceiveDataChanged += new EventHandler<ReceiveDataEventArgs>(SerialPort_OnReceiveDataChanged);

            dataAnalyzeMode = modbusRTUServer.DataAnalyzeMode;
            allDataPoints = modbusRTUServer.AllDataPoints;

            List<DataPoint> dataPoints = new List<DataPoint>();
            dataPoints.Add(new DataPoint()
            {
                DeviceAddress = 1,
                StartRegisterAddress = 83,
                DataPointType = DataPointType.WriteAndReadByFunNum03,
                DataPointDataType = DataPointDataType.F32,
                RealTimeValue = -1,
            });

            List<List<byte>> registerCommandBytes = ModbusRTU.CreateReadRegisterCommands(dataPoints);

            if (modbusRTUServer.SerialPort.TryOpenSerialPort())
            {
                foreach (var writeBytes in registerCommandBytes)
                {
                     modbusRTUServer.SerialPort.WriteSerialPort(writeBytes.ToArray());
                }
            }

        }
    }
}
