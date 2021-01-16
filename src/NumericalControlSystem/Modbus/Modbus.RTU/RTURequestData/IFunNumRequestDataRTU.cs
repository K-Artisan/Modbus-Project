using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modbus.RTU.RTURequestData
{
    public interface IFunNumRequestDataRTU
    {
        List<byte> ToByteList();
    }
}
