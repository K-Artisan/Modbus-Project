using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.TCP.TCPRequest
{
    public  interface ITCPHeader
    {
        List<byte> ToByteList();
    }
}
