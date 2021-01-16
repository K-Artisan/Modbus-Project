using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Azir.Infrastructure.Configuration
{
    public class AppConfigApplicationSettings : IApplicationSettings
    {
        public string Log4NetConfigPath
        {
            get { return ConfigurationManager.AppSettings["Log4netConfigPath"]; }
        }

        public string SystemLoggerName
        {
            get { return ConfigurationManager.AppSettings["SystemLoggerName"]; }
        }

        public string DebugLoggerName
        {
            get { return ConfigurationManager.AppSettings["DebugLoggerName"]; }
        }

        public string ErrorLoggerName
        {
            get { return ConfigurationManager.AppSettings["ErrorLoggerName"]; }
        }

        public string DataBaseConfigFilePath
        {
            get { return ConfigurationManager.AppSettings["DataBaseConfigFilePath"]; }
        }

        public string ModbusConfigFilePath
        {
            get { return ConfigurationManager.AppSettings["ModbusConfigFilePath"]; }
        }

        public string SerialPortConfigFilePath
        {
            get { return ConfigurationManager.AppSettings["SerialPortConfigFilePath"]; }
        }

        public string CreateDataBaseSqcritFilePath
        {
            get { return ConfigurationManager.AppSettings["CreateDataBaseSqcritFilePath"]; }
        }

        public string NumericalControlSystemDataBaseSqcritFilePath
        {
            get { return ConfigurationManager.AppSettings["NumericalControlSystemDataBaseSqcritFilePath"]; }
        }






    }
}
