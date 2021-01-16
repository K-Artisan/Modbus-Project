using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol;
using Azir.Modbus.Protocol.DataPoints;
using Azir.ModbusServer.TCP.Command;
using Azir.ModbusServer.TCP.Socket;

namespace Azir.ModbusServer.TCP
{
    /// <summary>
    /// Modbus处理单元
    /// 主要是根据的IP不同进行单元处理
    /// </summary>
    public class ModbusUnit
    {
        /// <summary>
        /// ModbusUnit编号，唯一标识
        /// </summary>
        public string Number { get; set; }
        public SockeHelper Connector { get; set; }
        public DataAnalyzeMode DataAnalyzeMode { get; set; }
        /// <summary>
        /// 模块
        /// key:Module的Number字段,
        /// value:Module对象
        /// </summary>
        public Dictionary<string, Module> ModulesDic { get; set; }
        /// <summary>
        /// 数据点
        /// key:DataPoint的Number字段,
        /// value:DataPoint对象
        /// </summary>
        public Dictionary<string, DataPoint> DataPointsDic { get; set; }
        public List<DataPoint> AllDataPoints { get; set; }
        public List<ReadRegisterCommand> AllReadRegisterCommands { get; set; }
        public List<WriteRegisterCommand> AllWriterRegisterCommands { get; set; }

       private Queue<WriteRegisterCommand> toWriteRegisterCommands = new Queue<WriteRegisterCommand>();
        /// <summary>
        /// 写寄存器命令
        /// </summary>
        public Queue<WriteRegisterCommand> ToWriteRegisterCommands
        {
            get { return toWriteRegisterCommands; }
            set { toWriteRegisterCommands = value; }
        }

    }
}
