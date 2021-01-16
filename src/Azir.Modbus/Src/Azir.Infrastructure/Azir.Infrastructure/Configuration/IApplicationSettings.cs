using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azir.Infrastructure.Configuration
{
    /// <summary>
    /// 应用程序的配置
    /// </summary>
    public interface IApplicationSettings
    {
        string Log4NetConfigPath { get; }
        string SystemLoggerName { get; }
        string DebugLoggerName { get; }
        string ErrorLoggerName { get; }

        string DataBaseConfigFilePath { get; }
        /// <summary>
        /// Modbus配置文件路径
        /// </summary>
        string ModbusConfigFilePath { get; }
        string SerialPortConfigFilePath { get; }

        string CreateDataBaseSqcritFilePath { get; }
        string NumericalControlSystemDataBaseSqcritFilePath { get; }

        //TODO:根据需要添加其它配置

    }
}
