using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.DataPoints;

namespace Azir.Modbus.Protocol.Auxiliary
{
    public class DataPointRegisterAddressCompare : IComparer<DataPoint>
    {
        public int Compare(DataPoint x, DataPoint y)
        {
            return x.StartRegisterAddress.CompareTo(y.StartRegisterAddress);
        }
    }
}
