using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.DataPoints;

namespace Azir.Modbus.Protocol.Configer
{
    public class ModbusConfig
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 模块
        /// key:Module的Number字段,
        /// value:Module对象
        /// </summary>
        public Dictionary<string, Module> ModulesFromConfigFile { get; set; }
        /// <summary>
        /// 数据点
        /// key:DataPoint的Number字段,
        /// value:DataPoint对象
        /// </summary>
        public Dictionary<string, DataPoint> DataPointsFromConfigFile { get; set; }
        public List<DataPoint> DataPointsFromConfigFileList { get; set; }

        /// <summary>
        /// 数据解析方式
        /// </summary>
        public DataAnalyzeMode  DataAnalyzeMode { get; set; }
    }
}
