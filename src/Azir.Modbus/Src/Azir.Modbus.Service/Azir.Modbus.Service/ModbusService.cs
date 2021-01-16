using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azir.Infrastructure.Configuration;
using Azir.Infrastructure.Logging;
using Azir.Modbus.DataObject.DataPoint;
using Azir.Modbus.Protocol.DataPoints;
using Azir.Modbus.Protocol.DataReponse;
using Azir.Modbus.Service.Mapping;
using Azir.Modbus.TCP;
using Azir.ModbusServer.TCP;
using Azir.ModbusServer.TCP.Command;
using Azir.ModbusServer.TCP.DataObject;
using Azir.ModbusServer.TCP.Event;
using mdob=Azir.Modbus.DataObject;
using md=Azir.ModbusServer.TCP;

namespace Azir.Modbus.Service
{
    public sealed class ModbusService
    {
        #region 变量

        private static ModbusService _instance = null;
        private static readonly object SynObject = new object();
   
        public ModbusTCPServer ModbusTCPServer { get; set; }

        public delegate void OnDataPointRealValueChangedEventHandler(object obj, mdob.DataPoint.DataPointRealValueEventArgs dataPointRealValues);
        public event EventHandler<mdob.DataPoint.DataPointRealValueEventArgs> OnDataPointRealValueChanged = delegate { };
       
        #endregion

        #region 构造函数
        private ModbusService()
        {
            try
            {
                Init();
            }
            catch (Exception ex)
            {
                LoggingFactory.GetLogger().WriteErrorLogger(string.Format("ModbusService()异常:{0}--{1}", ex.Message, ex.InnerException));
            }

        }
        #endregion

        #region 单例

        public static ModbusService Instance
        {
            get
            {
                // Double-Checked Locking
                if (null == _instance)
                {
                    lock (SynObject)
                    {
                        if (null == _instance)
                        {
                            _instance = new ModbusService();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region 初始化

        private bool Init()
        {
            bool success = true;

            try
            {
                var modbusConfigFile = ApplicationSettingsFactory.GetApplicationSettings().ModbusConfigFilePath;
                ModbusTCPServer = GetModbusTCPServerFromConfigFile(modbusConfigFile);
            }
            catch (Exception ex)
            {
                success = false;
                LoggingFactory.GetLogger().WriteErrorLogger(string.Format("ModbusService->Init()异常:{0}--{1}", ex.Message, ex.InnerException));
            }

            return success;
        }

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
                LoggingFactory.GetLogger().WriteErrorLogger(string.Format("GetModbusTCPServerFromConfigFile发生异常:{0}--{1}", ex.Message, ex.InnerException));
                return modbusTCPServer;
            }

            return modbusTCPServer;
        }

        #endregion

        #region 启动或停止

        public void Start()
        {
            try
            {
                this.ModbusTCPServer.StartModbus();
                //注册事件
                ModbusTCPServer.OnDataPointRealValueChanged += DoOnDataPointRealValueChanged;
            }
            catch (Exception ex)
            {
                LoggingFactory.GetLogger().WriteErrorLogger(string.Format("ModbusService->Start()异常:{0}--{1}", ex.Message, ex.InnerException));
            }
        }

        public void ReStart()
        {
            try
            {
                Stop();
                Init();
                Start();
            }
            catch (Exception)
            {
            }
        }

        public void Stop()
        {
            try
            {
                ModbusTCPServer.Stop();
            }
            catch (Exception ex)
            {
                LoggingFactory.GetLogger().WriteErrorLogger(string.Format("ModbusService->Stop()异常:{0}--{1}", ex.Message, ex.InnerException));
            }
        }

        #endregion

        #region 数据点

        public List<DataPointDto> GetAllDataPoints()
        {
            List<DataPointDto> allDataPoint = new List<DataPointDto>();
            List<ModbusUnit> modbusUnits = ModbusTCPServer.ModbusUnits;
            foreach (var modbusUnit in modbusUnits)
            {
                var dataPointOfunit = DataPointDtoMapper.CreateDataPointDtos(modbusUnit);
                allDataPoint.AddRange(dataPointOfunit);
            }

            return allDataPoint;
        }

        #endregion

        #region 读取数据点数据

        private void DoOnDataPointRealValueChanged(object sender, md.Event.DataPointRealValueEventArgs e)
        {
            List<DataPointRealValue> dataPointRealValues = e.DataPointRealValues;
            List<DataPointRealValueDto> dataPointRealValueDtos = DataObjectMapper.ConvertToListFrom(dataPointRealValues);

            RaiseCurrentReceiveDataChangedEvent(dataPointRealValueDtos);
        }

        /// <summary>
        /// 激活数据点发生改变事件
        /// </summary>
        /// <param name="dataPointRealValues"></param>
        private void RaiseCurrentReceiveDataChangedEvent(List<DataPointRealValueDto> dataPointRealValues)
        {
            if (null != dataPointRealValues && dataPointRealValues.Any())
            {
                if (null != OnDataPointRealValueChanged)
                {
                    mdob.DataPoint.DataPointRealValueEventArgs dataEventArgs = new mdob.DataPoint.DataPointRealValueEventArgs(dataPointRealValues);
                    foreach (EventHandler<mdob.DataPoint.DataPointRealValueEventArgs> hanlder in OnDataPointRealValueChanged.GetInvocationList())
                    {
                        hanlder(this, dataEventArgs);
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
        public void SetDataPointValue(SetDataPointValueDto setDpDto)
        {
            SetDataPointValue setDataPointValue = new SetDataPointValue()
            {
                DataPointNumber = setDpDto.DataPointNumber,
                ValueToSet = setDpDto.ValueToSet,
            };
            this.ModbusTCPServer.AddDataPointToSetValue(setDataPointValue);
        }

        #endregion
    }
}
