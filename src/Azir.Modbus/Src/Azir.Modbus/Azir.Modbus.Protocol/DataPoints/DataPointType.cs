using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.Protocol.DataPoints
{
    /// <summary>
    /// DataPoint的类型
    /// </summary>
    public enum DataPointType
    {
        ReadByFunNum03 = 0,
        ReadByFunNum01 = 1,
        WriteAndReadByFunNum03 = 2,
        WriteAndReadByFunNum01 = 3


        //AI = 1,
        //DI = 2,
        //AO = 3,
        //DO = 4,
        //ACC = 5
    } 
}
