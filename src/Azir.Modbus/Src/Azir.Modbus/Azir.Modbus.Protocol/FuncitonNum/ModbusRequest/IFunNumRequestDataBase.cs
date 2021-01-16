using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.Protocol.FuncitonNum.ModbusRequest
{
    /// <summary>
    /// 直接发送给Modbus的请求帧（组）
    /// 之所以名称有后缀Base，表明TCP、RTU方式公用的接口
    /// </summary>
    public interface IFunNumRequestDataBase
    {
        /// <summary>
        /// 返回若干请求帧：
        /// （如果需要客户端的请求ICustomerRequestData分解成若干的IFunNumRequestDataBase）
        /// </summary>
        /// <returns></returns>
        List<byte> ToByteList();
    }
}
