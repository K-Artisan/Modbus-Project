using System.Collections.Generic;

namespace Azir.Modbus.RTU.RTURequest
{
    /// <summary>
    /// RTU请求数据
    /// </summary>
    public interface IFunNumRequestDataRTU<T>
    {
        List<byte> ToByteList();
    }
}
