using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Infrastructure.Configuration
{
    /// <summary>
    /// 应用程序的配置
    /// </summary>
    public interface IApplicationSettings
    {
        string SystemLoggerName { get; }
        string DebugLoggerName { get; }

        string DataBaseConfigFilePath { get; }
        string ModbusConfigFilePath { get; }
        string SerialPortConfigFilePath { get; }

        string CreateDataBaseSqcritFilePath { get; }
        string NumericalControlSystemDataBaseSqcritFilePath { get; }

        //TODO:根据需要添加其它配置
    }
}
