using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azir.Modbus.Protocol;
using Azir.Modbus.Protocol.Auxiliary;
using Azir.Modbus.Protocol.Configer;
using Azir.Modbus.Protocol.DataPoints;
using Azir.Modbus.Protocol.DataReponse;
using Azir.Modbus.TCP;
using Azir.ModbusServer.TCP.Command;
using Azir.ModbusServer.TCP.DataObject;
using Azir.ModbusServer.TCP.Event;
using Azir.ModbusServer.TCP.Mapper;
using Azir.ModbusServer.TCP.Socket;

namespace Azir.ModbusServer.TCP
{
    public class ModbusTCPServer
    {
        public List<ModbusUnit> ModbusUnits { get; set; }

        public delegate void OnDataPointRealValueChangedEventHandler(object obj, DataPointRealValueEventArgs dataPointRealValues);
        public event EventHandler<DataPointRealValueEventArgs> OnDataPointRealValueChanged = delegate { };

        private Thread readMobusThread = null;
        private Thread writeMobusThread = null;
        private static readonly object writeMobusThreadLoker = new object();

        #region ctor

        public ModbusTCPServer()
        {

        }

        #endregion

        /// <summary>
        /// 初始化运行环境
        /// </summary>
        /// <param name="modbusConfigFile">配置文件物理路径（包含名称+后缀）：例如：Config/ModbusConfig.xml</param>
        public void InitializeFromConfigFile(string modbusConfigFile)
        {
            var modbusConfigs = GetModbusConfigFromFile(modbusConfigFile);
            ModbusUnits = new List<ModbusUnit>();

            int number = 0;
            foreach (var modbusConfig in modbusConfigs)
            {
                ModbusUnit modbusUnit = new ModbusUnit();
                modbusUnit.Number = Convert.ToString(++number);
                modbusUnit.Connector = new SockeHelper(modbusConfig.IP, modbusConfig.Port);
                modbusUnit.DataAnalyzeMode = modbusConfig.DataAnalyzeMode;
                modbusUnit.ModulesDic = modbusConfig.ModulesFromConfigFile;
                modbusUnit.DataPointsDic = modbusConfig.DataPointsFromConfigFile;
                var allDataPoints = modbusConfig.DataPointsFromConfigFileList;
                modbusUnit.AllDataPoints = allDataPoints;
                modbusUnit.AllReadRegisterCommands = GetReadRegisterCommands(allDataPoints);

                ModbusUnits.Add(modbusUnit);
            }
        }

        #region 从配置文件读取配置

        /// <summary>
        /// 从配置文件中读取Modbus配置
        /// </summary>
        /// <param name="modbusConfigFile">配置文件物理路径（包含名称+后缀）：例如：Config/ModbusConfig.xml</param>
        /// <returns></returns>
        private List<ModbusConfig> GetModbusConfigFromFile(string modbusConfigFile)
        {
            List<ModbusConfig> modbusConfigs = ModbusConfiger.ReadConfigFormModbusConfigFile(modbusConfigFile);
            return modbusConfigs;
        }

        #endregion

        #region 初始化所有"读寄存器"的命令

        private List<ReadRegisterCommand> GetReadRegisterCommands(List<DataPoint> dataPoints)
        {
            List<ReadRegisterCommand> readRegisterCommands = new List<ReadRegisterCommand>();

            List<List<byte>> requestBytes = ModbusTCP.CreateReadRegisterCommands(dataPoints);

            foreach (var requestByte in requestBytes)
            {
                var readRegisterCommand = new ReadRegisterCommand();
                readRegisterCommand.ReadCommand = requestByte;

                readRegisterCommands.Add(readRegisterCommand);
            }

            return readRegisterCommands;
        }

        #endregion

        #region 停止

        public void Stop()
        {
            try
            {
                StopModbus();
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region 读写运行机制

        #region 读写数据 初始化


        /// <summary>
        /// 通过配置文件初始化ModbusTcp服务
        /// </summary>
        /// <param name="modbusConfigFile">配置文件物理路径（包含名称+后缀）：例如：Config/ModbusConfig.xml</param>
        /// <returns></returns>
        private ModbusTCPServer GetModbusTCPServerFromConfigFile(string modbusConfigFile)
        {
            ModbusTCPServer modbusTCPServer = null;

            try
            {
                modbusTCPServer = new ModbusTCPServer();
                modbusTCPServer.InitializeFromConfigFile(modbusConfigFile);
            }
            catch (Exception ex)
            {
                return modbusTCPServer;
            }

            return modbusTCPServer;
        }

        #endregion

        #region 启动或停止

        public void StartModbus()
        {
            try
            {
                InitializeReadMobusThread();
                InitializeWriteMobusThread();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void InitializeReadMobusThread()
        {
            this.readMobusThread = new Thread(new ThreadStart(ReadMobusThread));
            this.readMobusThread.IsBackground = true;
            this.readMobusThread.Start();
        }

        private void InitializeWriteMobusThread()
        {
            this.writeMobusThread = new Thread(new ThreadStart(WriteMobusThread));
            this.writeMobusThread.IsBackground = true;
            this.writeMobusThread.Start();
        }

        private void ReadMobusThread()
        {
            try
            {
                ReadModbus();
            }
            catch (Exception)
            {
                //异常会使得线程Stoped,处于Stoped状态的线程不能重启。
                //故需要重新初始化线程并启动。
                InitializeReadMobusThread();
            }

        }

        private void WriteMobusThread()
        {
            try
            {
                WriteModbus();
            }
            catch (Exception)
            {
                //异常会使得线程Stoped,处于Stoped状态的线程不能重启。
                //故需要重新初始化线程并启动。
                InitializeReadMobusThread();
            }

        }

        public void ReStartModbus()
        {
            try
            {
                StopModbus();
                StartModbus();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void StopModbus()
        {
            try
            {
                foreach (var modbusUnit in ModbusUnits)
                {
                    modbusUnit.Connector.Stop();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion


        #region 读取数据点数据
        private void ReadModbus()
        {
            try
            {
                foreach (var modbusUint in this.ModbusUnits)
                {
                    if (!modbusUint.Connector.IsConnect())
                    {
                        modbusUint.Connector.Connect();
                    }
                }

                while (CanReadModbus())
                {
                    foreach (var modbusUint in this.ModbusUnits)
                    {
                        foreach (var readRegisterCommand in modbusUint.AllReadRegisterCommands)
                        {
                            var recvBytes = modbusUint.Connector.Send(readRegisterCommand.ReadCommand);
                            if (recvBytes != null)
                            {
                                AnalyzeRecivedDataReponse reponse = ModbusTCP.AnalyzeRecivedDataStatic(modbusUint.DataAnalyzeMode, readRegisterCommand.ReadCommand, recvBytes);
                                if (reponse.ModbusReponseSuccess && reponse.AnalyzeRecivedDataSuccess)
                                {
                                    List<DataPoint> dataPointsWhoseRealTimeDataChanged = ModbusTCP.SetDataPointValueFromRegisterValue(reponse.Registers, modbusUint.AllDataPoints);

                                    List<DataPointRealValue> dataPointRealValues = DataObjectMapper.ConvertToListFrom(dataPointsWhoseRealTimeDataChanged);
                                    RaiseCurrentReceiveDataChangedEvent(dataPointRealValues);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ReadModbus();
            }
        }

        private bool CanReadModbus()
        {
            return true;
        }

        /// <summary>
        /// 激活数据点发生改变事件
        /// </summary>
        /// <param name="dataPointRealValues"></param>
        private void RaiseCurrentReceiveDataChangedEvent(List<DataPointRealValue> dataPointRealValues)
        {
            if (null != dataPointRealValues && dataPointRealValues.Any())
            {
                if (null != OnDataPointRealValueChanged)
                {
                    DataPointRealValueEventArgs requstDataEventArgs = new DataPointRealValueEventArgs(dataPointRealValues);
                    foreach (EventHandler<DataPointRealValueEventArgs> hanlder in OnDataPointRealValueChanged.GetInvocationList())
                    {
                        hanlder(this, requstDataEventArgs);
                    }
                }
            }
        }
        #endregion

        #region 写数据点


        /// <summary>
        /// 设置DataPiont的值
        /// </summary>
        /// <param name="setDpDto"></param>
        public void AddDataPointToSetValue(SetDataPointValue setDpDto)
        {
            var toFindModbusUnit = FindModbusUnit(this.ModbusUnits, setDpDto.DataPointNumber);
            if (toFindModbusUnit != null)
            {
                List<DataPoint> toWriteDataPoints = new List<DataPoint>();
                var toWriteDataPoint = toFindModbusUnit.AllDataPoints.FirstOrDefault(p => p.Number == setDpDto.DataPointNumber);
                if (toWriteDataPoint != null)
                {
                    var copyDp = toWriteDataPoint.CopyNew();
                    copyDp.ValueToSet = setDpDto.ValueToSet;
                    toWriteDataPoints.Add(copyDp);
                }

                List<List<byte>> requestBytes = ModbusTCP.CreateWriteRegisterCommands(toFindModbusUnit.DataAnalyzeMode, toWriteDataPoints);
                foreach (var requestByte in requestBytes)
                {
                    lock (writeMobusThreadLoker)
                    {
                        WriteRegisterCommand writeRegisterCommand = new WriteRegisterCommand();
                        writeRegisterCommand.WriteCommand = requestByte;
                        toFindModbusUnit.ToWriteRegisterCommands.Enqueue(writeRegisterCommand); 
                    }
                }
            }
        }

        private void WriteModbus()
        {
            try
            {
                foreach (var modbusUint in this.ModbusUnits)
                {
                    if (!modbusUint.Connector.IsConnect())
                    {
                        modbusUint.Connector.Connect();
                    }
                }

                while (CanWirteModbus())
                {
                    foreach (var modbusUint in this.ModbusUnits)
                    {
                        if (modbusUint.ToWriteRegisterCommands != null && modbusUint.ToWriteRegisterCommands.Count > 0)
                        {
                            WriteRegisterCommand currentWriteRegisterCommand = null;
                            List<byte> recvBytes = null;
                            lock (writeMobusThreadLoker)
                            {
                                currentWriteRegisterCommand = modbusUint.ToWriteRegisterCommands.Dequeue();
                            }

                            if (currentWriteRegisterCommand != null)
                            {
                                recvBytes = modbusUint.Connector.Send(currentWriteRegisterCommand.WriteCommand);
                                if (recvBytes != null)
                                {
                                    AnalyzeRecivedDataReponse reponse = ModbusTCP.AnalyzeRecivedDataStatic(modbusUint.DataAnalyzeMode, currentWriteRegisterCommand.WriteCommand, recvBytes);
                                    if (reponse.ModbusReponseSuccess && reponse.AnalyzeRecivedDataSuccess)
                                    {
                                        var dataPointsWhoseRealTimeDataChanged = ModbusTCP.SetDataPointValueFromRegisterValue(reponse.Registers, modbusUint.AllDataPoints);

                                        List<DataPointRealValue> dataPointRealValues = DataObjectMapper.ConvertToListFrom(dataPointsWhoseRealTimeDataChanged);
                                        RaiseCurrentReceiveDataChangedEvent(dataPointRealValues);
                                    }
                                } 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteModbus();
            }
        }

        private bool CanWirteModbus()
        {
            return true;
        }

        #endregion

        #endregion

        #region 辅助函数

        /// <summary>
        /// 在目标ModbusUnit集合中查找数据点所在的ModbusUnit
        /// </summary>
        /// <param name="allModbusUnits">目标ModbusUnit集合</param>
        /// <param name="dataPointNumber">数据点编号（唯一标识）</param>
        /// <returns>目标ModbusUnit</returns>
        public static ModbusUnit FindModbusUnit(List<ModbusUnit> allModbusUnits, string dataPointNumber)
        {
            ModbusUnit toFindModbusUnit = null;

            if (allModbusUnits != null && !string.IsNullOrWhiteSpace(dataPointNumber))
            {
                foreach (var modbusUnit in allModbusUnits)
                {
                    if (modbusUnit.AllDataPoints.Any(p => p.Number == dataPointNumber))
                    {
                        toFindModbusUnit = modbusUnit;
                        break;
                    }
                }
            }

            return toFindModbusUnit;
        }

        #endregion
    }
}
