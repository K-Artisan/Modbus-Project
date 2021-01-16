using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Modbus.Contract.RequestDataBase;
using NCS.Infrastructure.Configuration;

namespace Modbus.RTU
{
    public static class DataAnalyzeModeConfiger
    {
        #region 数据解析方式



        public static void SaveDataAnalyzeModeToConfigFile(DataAnalyzeMode dataAnalyzeMode)
        {

            try
            {
                string modbusConfigRelativePath = ApplicationSettingsFactory.GetApplicationSettings().ModbusConfigFilePath;
                string modbusConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                                                                 modbusConfigRelativePath);

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

        public static DataAnalyzeMode GetDataAnalyzeModeFromConfigFile()
        {
            DataAnalyzeMode dataAnalyzeMode = DataAnalyzeMode.DataHighToLow;

            try
            {
                string modbusConfigRelativePath = ApplicationSettingsFactory.GetApplicationSettings().ModbusConfigFilePath;
                string modbusConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                                                                 modbusConfigRelativePath);

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


    }
}
