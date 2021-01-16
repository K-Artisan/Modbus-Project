using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Azir.Modbus.Protocol.DataPoints;

namespace Azir.Modbus.Protocol.Configer
{
    public class ModbusConfiger
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="modbusConfigFile">配置文件物理路径（包含名称+后缀）：例如：Config/ModbusConfig.xml</param>
        public ModbusConfiger(string modbusConfigFile)
        {
        }

        #endregion

        #region 从配置文件中读取配置

        /// <summary>
        /// 从配置文件中读取Modbus配置
        /// </summary>
        /// <returns></returns>
        /// <param name="modbusConfigFile">配置文件物理路径（包含名称+后缀）：例如：Config/ModbusConfig.xml</param>
        public static List<ModbusConfig> ReadConfigFormModbusConfigFile(string modbusConfigFile)
        {
            try
            {
                return  ReadModbusConfigFromFile(modbusConfigFile);

            }
            catch (Exception ex)
            {
                string message = "\rModbus配置文件:\r" + modbusConfigFile + "中\r" + ex.Message;
                //LoggingFactory.GetLogger().WriteDebugLogger(message);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// 从配置文件中读取：模块和数据点的配置
        /// </summary>
        /// <param name="modbusConfigFile">配置文件物理路径（包含名称+后缀）：例如：Config/ModbusConfig.xml </param>
        private static List<ModbusConfig> ReadModbusConfigFromFile(string modbusConfigFile)
        {
            var modbusConfigs = new List<ModbusConfig>();

            if (System.IO.File.Exists(modbusConfigFile))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(modbusConfigFile);

                XmlNodeList modbusConfigNodes = xmlDoc.SelectNodes("/ModbusConfigs/ModbusConfig");
                if (null != modbusConfigNodes && modbusConfigNodes.Count > 0)
                {
                    int moduleId = 0;
                    int dataPointId = 0;

                    foreach (XmlNode mdCfgNode in modbusConfigNodes)
                    {
                        var modbusConfig = new ModbusConfig()
                        {
                            DataAnalyzeMode = DataAnalyzeMode.DataHighToLow,
                            ModulesFromConfigFile = new Dictionary<string, Module>(),
                            DataPointsFromConfigFile = new Dictionary<string, DataPoint>(),
                            DataPointsFromConfigFileList = new List<DataPoint>()
                        };
                        XmlElement mdCfgNodeElement = (XmlElement)mdCfgNode;

                        XmlNode dataAnalyzeModeNode = mdCfgNodeElement.SelectSingleNode("DataAnalyzeMode");
                        XmlElement dataAnalyzeModeElement = (XmlElement)dataAnalyzeModeNode;
                        if (dataAnalyzeModeElement != null)
                        {
                            modbusConfig.DataAnalyzeMode = (DataAnalyzeMode)Enum.Parse(typeof(DataAnalyzeMode), dataAnalyzeModeElement.GetAttribute("Value"), true);
                        }

                        XmlNode IPNode = mdCfgNodeElement.SelectSingleNode("IP");
                        XmlElement IPNodeElement = (XmlElement)IPNode;
                        if (dataAnalyzeModeElement != null)
                        {
                            modbusConfig.IP = IPNodeElement.GetAttribute("Value");
                        }

                        XmlNode PortNode = mdCfgNodeElement.SelectSingleNode("Port");
                        XmlElement PortNodeElement = (XmlElement)PortNode;
                        if (dataAnalyzeModeElement != null)
                        {
                            modbusConfig.Port = Convert.ToInt32(PortNodeElement.GetAttribute("Value"));
                        }

                        XmlNodeList moduleNodes = mdCfgNodeElement.SelectNodes("Module");
                        if (null != moduleNodes && moduleNodes.Count > 0)
                        {
                            ClearConfigDictionary(modbusConfig);
                        
                            foreach (XmlNode moduleNode in moduleNodes)
                            {
                                // Module
                                moduleId++;
                                XmlElement moduleElement = (XmlElement)moduleNode;
                                string moduleNumber = Convert.ToString(moduleId);
                                moduleElement.SetAttribute("Number", moduleNumber);

                                string moduleName = moduleElement.GetAttribute("Name");
                                string moduleDescription = moduleElement.GetAttribute("Description");

                                Module module = new Module();
                                module.Number = moduleNumber;

                                AddModule(modbusConfig,
                                          module,
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
                                        dataPointElement.SetAttribute("Number", dataPointNumber);

                                        string dataPointName = dataPointElement.GetAttribute("Name");
                                        string deviceAddress = dataPointElement.GetAttribute("DeviceAddress");
                                        string dataPointStartingRegister = dataPointElement.GetAttribute("StartingRegister");
                                        string dataPointDataType = dataPointElement.GetAttribute("DataType");
                                        string dataDataDataPointType = dataPointElement.GetAttribute("DataPointType");
                                        string datapointDescription = dataPointElement.GetAttribute("Description");

                                        DataPoint dataPoint = new DataPoint();
                                        dataPoint.Number = Convert.ToString(dataPointId);

                                        AddDataPoint(modbusConfig,
                                                     dataPoint,
                                                     dataPointNumber,
                                                     deviceAddress,
                                                     dataPointStartingRegister,
                                                     dataPointName,
                                                     dataPointDataType,
                                                     dataDataDataPointType,
                                                     datapointDescription);

                                        module.DataPoints.Add(dataPoint);
                                    }
                                }
                            }

                            modbusConfigs.Add(modbusConfig);
                        }
                    }
                }

                xmlDoc.Save(modbusConfigFile);
            }

            return modbusConfigs;
        }

        private static void AddModule(ModbusConfig modbusConfig,
            Module module,
            string moduleNumber,
            string moduleName,
            string moduleDescription)
        {
            module.Number = moduleNumber;
            module.Name = string.IsNullOrEmpty(moduleName) ? " " : moduleName;
            module.Description = string.IsNullOrEmpty(moduleDescription) ? " " : moduleDescription;

            if (!modbusConfig.ModulesFromConfigFile.ContainsKey(moduleNumber))
            {
                modbusConfig.ModulesFromConfigFile.Add(module.Number, module);
            }
            else
            {
                throw new Exception("Moduled Number=" + moduleNumber + "有重复");
            }
        }

        private static  void AddDataPoint(ModbusConfig modbusConfig,
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

            dataPoint.Number = dataPointNumber;
            dataPoint.Name = string.IsNullOrEmpty(dataPointName) ? " " : dataPointName;
            dataPoint.DeviceAddress = Convert.ToInt32(deviceAddress);
            dataPoint.StartRegisterAddress = Convert.ToInt32(dataPointStartingRegister);

            try
            {
                dataPoint.DataPointDataType = (DataPointDataType)Enum.Parse(typeof(DataPointDataType), dataPointDataType, true);
            }
            catch (Exception)
            {
                throw new Exception("DataPoint Number=" + dataPointNumber + "的DataType的值" + dataPointDataType + "是无效值，请确认填写有效的值");
            }

            try
            {
                dataPoint.DataPointType = (DataPointType)Enum.Parse(typeof(DataPointType), dataDataDataPointType, true);
            }
            catch (Exception)
            {
                throw new Exception("DataPoint Number=" + dataPointNumber + "的DataPointType的值" + dataPointDataType + "是无效值，请确认填写有效的值");
                ;
            }

            dataPoint.Description = string.IsNullOrEmpty(dataPointName) ? " " : datapointDescription;

            if (!modbusConfig.DataPointsFromConfigFile.ContainsKey(dataPointNumber))
            {
                modbusConfig.DataPointsFromConfigFile.Add(dataPoint.Number, dataPoint);
                modbusConfig.DataPointsFromConfigFileList.Add(dataPoint);
            }
            else
            {
                throw new Exception("DataPoint Number=" + dataPointNumber + "有重复");
            }
        }

        /// <summary>
        /// 校验数据点的配置是否正确
        /// </summary>
        /// <param name="number">数据点编号</param>
        /// <param name="deviceAddress">设备地址</param>
        /// <param name="startingRegister">起始寄存器地址</param>
        private static void VerifyDataPoint(string number, string deviceAddress, string startingRegister)
        {
            if (string.IsNullOrWhiteSpace(deviceAddress))
            {
                string message = "Number=" + number + "的DataPoin中存在若干个值为空或空格DeviceAddress，请为其填写有效值";

                throw new Exception(message);
            }

            if (string.IsNullOrWhiteSpace(startingRegister))
            {
                string message = "Number=" + number + "的DataPoin中存在若干个值为空或空格的StartingRegister，请为其填写有效值";

                throw new Exception(message);
            }
        }

        #endregion

        #region 数据解析方式

        /// <summary>
        /// 设置数据解析方式
        /// </summary>
        /// <param name="dataAnalyzeMode">数据解析方式</param>
        /// <param name="modbusConfigFile">配置文件物理路径（包含名称+后缀）：例如：Config/ModbusConfig.xml </param>
        public static void SaveDataAnalyzeModeToConfigFile(DataAnalyzeMode dataAnalyzeMode, string modbusConfigFile)
        {
            try
            {
                if (System.IO.File.Exists(modbusConfigFile))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(modbusConfigFile);

                    XmlNode dataAnalyzeModeNode = xmlDoc.SelectSingleNode("/ModbusConfig/DataAnalyzeMode");
                    XmlElement dataAnalyzeModeElement = (XmlElement)dataAnalyzeModeNode;
                    if (dataAnalyzeModeElement != null)
                        dataAnalyzeModeElement.SetAttribute("Value", Convert.ToString(dataAnalyzeMode));

                    xmlDoc.Save(modbusConfigFile);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 获取数据解析方式
        /// </summary>
        /// <param name="modbusConfigFile">配置文件物理路径（包含名称+后缀）：例如：Config/ModbusConfig.xml </param>
        /// <returns></returns>
        public static DataAnalyzeMode GetDataAnalyzeModeFromConfigFile(string modbusConfigFile)
        {
            DataAnalyzeMode dataAnalyzeMode = DataAnalyzeMode.DataHighToLow;

            try
            {
                if (System.IO.File.Exists(modbusConfigFile))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(modbusConfigFile);

                    XmlNode dataAnalyzeModeNode = xmlDoc.SelectSingleNode("/ModbusConfig/DataAnalyzeMode");
                    XmlElement dataAnalyzeModeElement = (XmlElement)dataAnalyzeModeNode;
                    if (dataAnalyzeModeElement != null)
                    {
                        dataAnalyzeMode = (DataAnalyzeMode)Enum.Parse(typeof(DataAnalyzeMode), dataAnalyzeModeElement.GetAttribute("Value"), true);
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            return dataAnalyzeMode;
        }

        #endregion

        #region 辅助方法

        private static void ClearConfigDictionary(ModbusConfig modbusConfig)
        {
            modbusConfig.ModulesFromConfigFile.Clear();
            modbusConfig.DataPointsFromConfigFile.Clear();
        }

        #endregion
    }
}
