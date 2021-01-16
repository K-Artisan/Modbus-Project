using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Modbus.Contract.RequestDataBase;
using Modbus.RTU;
using NCS.Infrastructure.Configuration;
using NCS.Infrastructure.Logging;
using NCS.Infrastructure.Querying;
using NCS.Infrastructure.UnitOfWork;



using NCS.Service.Messaging.ModbusConfigService;
using NCS.Service.ServiceInterface;
using NCS.Service.SeviceImplementation.ModbusService;
using NCS.Model.Repository;
using NCS.Model.Entity;

namespace NCS.Service.SeviceImplementation
{
    public class ModbusConfigService : IModbusConfigService
    {
        private readonly IDataPointRepository _dataPointRepository;
        private readonly IModuleRepository _moduleRepository;

        private static DataAnalyzeMode currentDataAnalyzeMode = DataAnalyzeMode.DataLowToHigh;

        /// <summary>
        /// key:Module的Number字段,
        /// value:Module对象
        /// </summary>
        private Dictionary<int, Module> _modulesFromConfigFile = new Dictionary<int, Module>();
        /// <summary>
        /// key:DataPoint的Number字段,
        /// value:DataPoint对象
        /// </summary>
        private Dictionary<int, DataPoint> _dataPointsFromConfigFile = new Dictionary<int, DataPoint>();

        public ModbusConfigService(IDataPointRepository dataPointRepository,
                             IModuleRepository moduleRepository)
        {
            _dataPointRepository = dataPointRepository;
            _moduleRepository = moduleRepository;

            currentDataAnalyzeMode = DataAnalyzeModeConfiger.GetDataAnalyzeModeFromConfigFile();
        }


        #region IModbusConfigService members

        public SetModbusConfigToDataBaseResponse SetModbusConfigToDataBase()
        {
            SetModbusConfigToDataBaseResponse response = new SetModbusConfigToDataBaseResponse();

            try
            {
                ReadConfigFormModbusConfigFile();
                WriteConfigToDataBase();
            }
            catch (Exception ex)
            {
                string message = "保存Modbus配置信息到数据库失败。" + ex.Message;
                response.ResponseSucceed = false;
                response.Message = message;
                LoggingFactory.GetLogger().WriteDebugLogger(message + ex.Message);

                return response;
            }

            return response;
        }

        public SetDataAnalyzeModeResponse SetDataAnalyzeMode(SetDataAnalyzeModeRequest request)
        {
            SetDataAnalyzeModeResponse response = new SetDataAnalyzeModeResponse();

            DataAnalyzeMode oldDataAnalyzeMode = currentDataAnalyzeMode;
            try
            {
                currentDataAnalyzeMode = request.DataAnalyzeMode;
                DataAnalyzeModeConfiger.SaveDataAnalyzeModeToConfigFile(request.DataAnalyzeMode);
            }
            catch (Exception ex)
            {
                currentDataAnalyzeMode = oldDataAnalyzeMode;

                string message = "保存数据解析模式失败";
                response.ResponseSucceed = false;
                LoggingFactory.GetLogger().WriteDebugLogger(message + ex.Message);
            }

            return response;
        }
 
        public GetDataAnalyzeModeResponse GetDataAnalyzeMode()
        {
            GetDataAnalyzeModeResponse response = new GetDataAnalyzeModeResponse();

            DataAnalyzeMode oldDataAnalyzeMode = currentDataAnalyzeMode;
            try
            {
                currentDataAnalyzeMode= DataAnalyzeModeConfiger.GetDataAnalyzeModeFromConfigFile();
                response.DataAnalyzeMode = currentDataAnalyzeMode;
            }
            catch (Exception ex)
            {
                currentDataAnalyzeMode = oldDataAnalyzeMode;

                string message = "获取数据解析模式失败";
                response.ResponseSucceed = false;
                LoggingFactory.GetLogger().WriteDebugLogger(message + ex.Message);
            }

            return response;
        }

        #endregion


        #region 从配置文件中读取配置

        public bool ReadConfigFormModbusConfigFile()
        {
            bool success = true;
            try
            {
                ReadConfigFile();
            }
            catch (Exception ex)
            {
                string modbusConfigRelativePath =
                    ApplicationSettingsFactory.GetApplicationSettings().ModbusConfigFilePath;
                string modbusConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                                                                 modbusConfigRelativePath);

                string message = "\rModbus配置文件:\r" + modbusConfigFile + "中\r" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);
                throw new Exception(message);
            }


            return success;
        }

        private void ReadConfigFile()
        {
            string modbusConfigRelativePath = ApplicationSettingsFactory.GetApplicationSettings().ModbusConfigFilePath;
            string modbusConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                                                             modbusConfigRelativePath);

            if (System.IO.File.Exists(modbusConfigFile))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(modbusConfigFile);

                XmlNodeList moduleNodes = xmlDoc.SelectNodes("/ModbusConfig/Module");
                if (null != moduleNodes && moduleNodes.Count > 0)
                {
                    ClearConfigDictionary();
                    int moduleId = 0;
                    int dataPointId = 0;

                    foreach (XmlNode moduleNode in moduleNodes)
                    {
                        // Module
                        moduleId++;
                        XmlElement moduleElement = (XmlElement) moduleNode;
                        string moduleNumber = Convert.ToString(moduleId);
                        moduleElement.SetAttribute("Id", string.Format("{0:D4}", moduleId));

                        string moduleName = moduleElement.GetAttribute("Name");
                        string moduleDescription = moduleElement.GetAttribute("Description");

                        Module module = new Module();

                        //UnitOfWork机制和数据库外键的设计，决定了要预先设置数据库中的主键Id
                        //尽管数据库自己生产主键。
                        module.Id = moduleId;

                        AddModule(module,
                                  moduleNumber,
                                  moduleName,
                                  moduleDescription);

                        //DataPoint
                        XmlNodeList dataPointNodes = moduleNode.ChildNodes;
                        if (dataPointNodes.Count > 0)
                        {
                            foreach (XmlNode dataPointNode in dataPointNodes)
                            {
                                dataPointId++;
                                XmlElement dataPointElement = (XmlElement)dataPointNode;

                                string dataPointNumber = Convert.ToString(dataPointId);
                                dataPointElement.SetAttribute("Id", string.Format("{0:D5}", dataPointId));

                                string dataPointName = dataPointElement.GetAttribute("Name");
                                string deviceAddress = dataPointElement.GetAttribute("DeviceAddress");
                                string dataPointStartingRegister = dataPointElement.GetAttribute("StartingRegister");
                                string dataPointDataType = dataPointElement.GetAttribute("DataType");
                                string dataDataDataPointType = dataPointElement.GetAttribute("DataPointType");
                                string datapointDescription = dataPointElement.GetAttribute("Description");

                                DataPoint dataPoint = new DataPoint();

                                //UnitOfWork机制和数据库外键的设计，决定了要预先设置数据库中的主键Id
                                //尽管数据库自己生产主键。
                                dataPoint.Id = dataPointId;

                                AddDataPoint(module,
                                             dataPoint,
                                             dataPointNumber,
                                             deviceAddress,
                                             dataPointStartingRegister,
                                             dataPointName,
                                             dataPointDataType,
                                             dataDataDataPointType,
                                             datapointDescription);
                            }
                        }
                    }

                    xmlDoc.Save(modbusConfigFile);
                }
            }
        }

        private void ClearConfigDictionary()
        {
            _modulesFromConfigFile.Clear();
            _dataPointsFromConfigFile.Clear();
        }

        private void AddModule(Module module, 
            string moduleNumber, 
            string moduleName, 
            string moduleDescription)
        {
            module.Number = Convert.ToInt32(moduleNumber);
            module.Name = string.IsNullOrEmpty(moduleName) ? " " : moduleName;
            module.Description = string.IsNullOrEmpty(moduleDescription) ? " " : moduleDescription;
            _modulesFromConfigFile.Add(module.Number, module);
        }

        private void AddDataPoint(Module module, 
            DataPoint dataPoint,
            string dataPointNumber, 
            string deviceAddress,
            string dataPointStartingRegister, 
            string dataPointName,
            string dataPointDataType, 
            string dataDataDataPointType, 
            string datapointDescription)
        {
            VerifyDataPoint(dataPointNumber, deviceAddress, dataPointStartingRegister);

            dataPoint.ModuleBelongTo = module;
            dataPoint.Number = Convert.ToInt32(dataPointNumber);
            dataPoint.Name = string.IsNullOrEmpty(dataPointName) ? " " : dataPointName;
            dataPoint.DeviceAddress = Convert.ToInt32(deviceAddress);
            dataPoint.StartRegisterAddress = Convert.ToInt32(dataPointStartingRegister);

            try
            {
                dataPoint.DataType = (DataType)Enum.Parse(typeof(DataType), dataPointDataType, true);
            }
            catch (Exception)
            {
                throw new Exception("DataType的值" + dataPointDataType + "是无效值，请确认填写有效的值");
            }

            try
            {
                dataPoint.DataPointType = (DataPointType)Enum.Parse(typeof(DataPointType), dataDataDataPointType, true);
            }
            catch (Exception)
            {
                throw new Exception("DataPointType的值" + dataPointDataType + "是无效值，请确认填写有效的值");
                ;
            }

            dataPoint.Description = string.IsNullOrEmpty(dataPointName) ? " " : datapointDescription;


            _dataPointsFromConfigFile.Add(dataPoint.Number, dataPoint);
            
        }


        private void VerifyDataPoint(string number, string deviceAddress, string startingRegister)
        {
            if (string.IsNullOrWhiteSpace(deviceAddress))
            {
                string message = "Id=" + number + "的DataPoin中存在若干个值为空或空格DeviceAddress，请为其填写有效值";
                
                throw new Exception(message);
            }

            if (string.IsNullOrWhiteSpace(startingRegister))
            {
                string message = "Id=" + number + "的DataPoin中存在若干个值为空或空格的StartingRegister，请为其填写有效值";
 
                throw new Exception(message);
            }
        }

        #endregion

        #region 将配置文件中读取的配置写数据库中

        public void WriteConfigToDataBase()
        {
            ClearConfigWhichInDataBse();

            //Module是DataPoint的外键，先添加Module
            foreach (KeyValuePair<int, Module> keyValuePair in _modulesFromConfigFile)
            {
                _moduleRepository.Add(keyValuePair.Value);
            }

            //UnitOfWork机制，调用Commit（）才提交到数据库，
            var uwModuleRepository = _moduleRepository as IUnitOfWorkRepository;
            if (uwModuleRepository != null)
            {
                uwModuleRepository.UnitOfWork.Commit();
            }

            //Module是DataPoint的外键,故先设置DataPoint的数据库表中的外键值，否则添加失败
            SetForeignKeyForDataPiont();

            foreach (KeyValuePair<int, DataPoint> keyValuePair in _dataPointsFromConfigFile)
            {
                _dataPointRepository.Add(keyValuePair.Value);
            }

            var uwDataPointReposity = _dataPointRepository as IUnitOfWorkRepository;
            if (uwDataPointReposity != null)
            {
                uwDataPointReposity.UnitOfWork.Commit();
            }

        }

        private void SetForeignKeyForDataPiont()
        {
            IEnumerable<Module> modules = _moduleRepository.FindAll();
            foreach (Module module in modules)
            {
                foreach (KeyValuePair<int, DataPoint> dataPointKeyValuePair in _dataPointsFromConfigFile)
                {
                    if (module.Number == dataPointKeyValuePair.Value.ModuleBelongTo.Number)
                    {
                        //ModuleBelongTo.Id是DataPoint的外键
                        dataPointKeyValuePair.Value.ModuleBelongTo.Id = module.Id;
                    }
                }
            }
        }

        private void ClearConfigWhichInDataBse()
        {
            //Module是DataPoint的外键，先删DataPoint
            Query dataPointQuery = new Query();
            _dataPointRepository.Remove(dataPointQuery);

            Query moduleQuery = new Query();
            _moduleRepository.Remove(moduleQuery);

        }

        #endregion

        public static DataAnalyzeMode GetCurrentDataAnalyzeMode()
        {
            return currentDataAnalyzeMode;
        }
    }
}
