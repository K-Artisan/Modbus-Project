using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modbus.Contract
{
    /// <summary>
    /// CRC校验
    /// </summary>
    public class CRCVerification
    {
       public byte CRCLow { get; set; }
       public byte CRCHigh { get; set; } 
    }
}
