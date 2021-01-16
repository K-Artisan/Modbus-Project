using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Modbus.Contract;

namespace ModbusDriver.RTUModel.View
{
    public interface IFunctionNumView
    {
        FunctionNumType functionNum { get; set; }
        IRequestInfo GetRequestInfo();
    }
}
