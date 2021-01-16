using System.Collections.Generic;

namespace Azir.ModbusServer.RTU.Command
{
    public class ReadRegisterCommand
    {
        public List<byte> ReadCommand { get; set; }
    }
}
