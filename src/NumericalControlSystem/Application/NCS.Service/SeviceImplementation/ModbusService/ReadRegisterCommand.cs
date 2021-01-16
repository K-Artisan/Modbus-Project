using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Service.SeviceImplementation.ModbusService
{
    public class ReadRegisterCommand
    {
        private List<byte>  readCommand = new List<byte>();

        public List<byte> ReadCommand
        {
            get { return readCommand; }
            set { readCommand = value; }
        }
    }
}
