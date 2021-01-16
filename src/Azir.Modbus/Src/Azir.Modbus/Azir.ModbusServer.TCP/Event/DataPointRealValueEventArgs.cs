using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.ModbusServer.TCP.DataObject;

namespace Azir.ModbusServer.TCP.Event
{
    public class DataPointRealValueEventArgs : EventArgs
    {
        public List<DataPointRealValue> DataPointRealValues { get; set; }

        public DataPointRealValueEventArgs(List<DataPointRealValue> dataPointRealValues)
        {
            this.DataPointRealValues = dataPointRealValues;
        }
    }
}
