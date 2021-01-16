using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Model.Entity
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
    } 
}
