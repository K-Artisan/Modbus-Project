using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.TCP.TCPRequest
{
    /// <summary>
    /// TCP请求数据
    /// </summary>
    public interface IFunNumRequestDataRTU<T>
    {
        List<byte> ToByteList();
    }
}
