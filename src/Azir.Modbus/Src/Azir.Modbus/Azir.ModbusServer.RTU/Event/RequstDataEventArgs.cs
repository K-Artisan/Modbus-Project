using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.ModbusServer.RTU.Event
{
    public class RequstDataEventArgs : EventArgs
    {
        public List<byte> RequstData { get; set; }

        public RequstDataEventArgs(List<byte> requstData)
        {
            this.RequstData = requstData;
        }

    }
}
