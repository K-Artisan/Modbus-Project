using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modbus.Contract.RequestDataBase;
using NCS.Service.SeviceImplementation.ModbusService;

namespace NCS.Service.Messaging.ModbusConfigService
{
    public class GetDataAnalyzeModeResponse:AbstracttResponseBase

{
    public DataAnalyzeMode DataAnalyzeMode { get; set; }
}
}
