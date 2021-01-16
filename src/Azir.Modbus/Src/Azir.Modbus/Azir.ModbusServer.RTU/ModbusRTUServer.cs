using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol;
using Azir.Modbus.Protocol.Configer;
using Azir.Modbus.Protocol.DataPoints;
using Azir.Modbus.Protocol.DataReponse;
using Azir.Modbus.RTU;
using Azir.ModbusServer.RTU.Command;
using Azir.ModbusServer.RTU.Event;
using Azir.ModbusServer.RTU.SeriaPort;

namespace Azir.ModbusServer.RTU
{
    public class ModbusRTUServer
    {
        private SerialPortHelper serialPort;
        private ModbusRTU modbusRtu;

        private List<byte> currentRequestByteData = null;
        private List<byte> currentRecivedByteData = null;

        private bool canSendNextRequestCommandBytes = true;     //用于保证：请求必须有响应数据后才能再发下一条请求
        private DataAnalyzeMode dataAnalyzeMode = Modbus.Protocol.DataAnalyzeMode.DataHighToLow;
        private List<DataPoint> allDataPoints = null;
        private List<ReadRegisterCommand> allReadRegisterCommands = new List<ReadRegisterCommand>();
        private List<WriteRegisterCommand> allWriterRegisterCommands = new List<WriteRegisterCommand>();

        #region Public Methon

        public SerialPortHelper SerialPort
        {
            get { return serialPort; }
            set { serialPort = value; }
        }

        public DataAnalyzeMode DataAnalyzeMode
        {
            get { return dataAnalyzeMode; }
            set { dataAnalyzeMode = value; }
        }

        public List<DataPoint> AllDataPoints
        {
            get { return allDataPoints; }
            set { allDataPoints = value; }
        }

        public List<ReadRegisterCommand> AllReadRegisterCommands
        {
            get { return allReadRegisterCommands; }
            set { allReadRegisterCommands = value; }
        }

        public List<WriteRegisterCommand> AllWriterRegisterCommands
        {
            get { return allWriterRegisterCommands; }
            set { allWriterRegisterCommands = value; }
        }
        #endregion

        #region ctor

        public ModbusRTUServer()
        {
        }

        #endregion

        #region 从配置文件初始化运行环境

        /// <summary>
        /// 初始化运行环境
        /// </summary>
        /// <param name="modbusConfigFile">配置文件物理路径（包含名称+后缀）：例如：Config/ModbusConfig.xml</param>
        /// <param name="serialPortConfigFile">配置文件获路径（包含文件名+后缀）例如：Config/SerialPortConfig.xml</param>
        public void InitializeFromConfigFile(string modbusConfigFile, string serialPortConfigFile)
        {
            ModbusConfig modbusConfig = new ModbusConfig();
            modbusConfig = GetModbusConfigFromFile(modbusConfigFile);
            dataAnalyzeMode = modbusConfig.DataAnalyzeMode;
            allDataPoints = modbusConfig.DataPointsFromConfigFileList.ToList();
            allReadRegisterCommands = GetReadRegisterCommands(allDataPoints);

            //初始化串口
            var serialPorObj = CreateSerialPortFromConfigFile(serialPortConfigFile);
            this.serialPort = new SerialPortHelper(serialPorObj);
        }


        /// <summary>
        /// 从配置文件中读取Modbus配置
        /// </summary>
        /// <param name="modbusConfigFile">配置文件物理路径（包含名称+后缀）：例如：Config/ModbusConfig.xml</param>
        /// <returns></returns>
        private ModbusConfig GetModbusConfigFromFile(string modbusConfigFile)
        {
            var modbusConfigs = ModbusConfiger.ReadConfigFormModbusConfigFile(modbusConfigFile);
            var modbusConfig = modbusConfigs.FirstOrDefault();
            return modbusConfig;
        }

        #endregion

        #region 串口SerialPort

        /// <summary>
        /// 通过配置文件获取串口对象
        /// </summary>
        /// <param name="serialPortConfigFile">配置文件获路径（包含文件名+后缀）例如：Config/SerialPortConfig.xml</param>
        /// <returns></returns>
        private SerialPort CreateSerialPortFromConfigFile(string serialPortConfigFile)
        {
            var serolPort = SerialPorConfigerHelper.GetSerialPortFormConfigFile(serialPortConfigFile);
            return serolPort;
        }

        /// <summary>
        /// 设置串口的修改（包括保持到配置文件）
        /// </summary>
        /// <param name="serialPort"></param>
        /// <param name="serialPortConfigFile"></param>
        /// <returns></returns>
        public bool SetModbusRtuSerialPort(SerialPort serialPort, string serialPortConfigFile)
        {
            bool success = true;
            try
            {
                if (this.serialPort.TryOpenSerialPort())
                {
                    InitializeSerialPort(serialPort);
                    SerialPorConfigerHelper.SaveSerialPortToConfigFile(serialPort, serialPortConfigFile);
                }
                else
                {
                    success = false;
                }
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
        }

        /// <summary>
        /// 初始化串口通信
        /// </summary>
        /// <param name="serialPort"></param>
        private void InitializeSerialPort(SerialPort serialPort)
        {
            this.serialPort = new SerialPortHelper(serialPort);
            this.serialPort.OpenCurrentSerialPort();

            this.serialPort.OnCurrentRequestDataChanged += new EventHandler<RequstDataEventArgs>(SerialPort_OnRequestDataChanged);
            this.serialPort.OnCurrentReceiveDataChanged += new EventHandler<ReceiveDataEventArgs>(SerialPort_OnReceiveDataChanged);
        }

        private void SerialPort_OnRequestDataChanged(object sender, RequstDataEventArgs e)
        {
            List<byte> requstData = e.RequstData;
            this.currentRequestByteData = requstData;
        }

        private void SerialPort_OnReceiveDataChanged(object sender, ReceiveDataEventArgs e)
        {
            List<byte> recevideData = e.ReceiveData;
            //this.canSendNextRequestCommandBytes = true;

            if (null != recevideData)
            {
                this.currentRecivedByteData = recevideData;
            }
        }

        #endregion

        #region 初始化所有"读寄存器"的命令

        private List<ReadRegisterCommand> GetReadRegisterCommands(List<DataPoint> dataPoints)
        {
            List<ReadRegisterCommand> readRegisterCommands = new List<ReadRegisterCommand>();

            List<List<byte>> requestBytes = ModbusRTU.CreateReadRegisterCommands(dataPoints);

            foreach (var requestByte in requestBytes)
            {
                var readRegisterCommand = new ReadRegisterCommand();
                readRegisterCommand.ReadCommand = requestByte;

                readRegisterCommands.Add(readRegisterCommand);
            }

            return readRegisterCommands;
        }

        #endregion
    }
}
