using System.Collections.Generic;

namespace Azir.ModbusServer.TCP.Command
{
    public class ReadRegisterCommand
    {
        public List<byte> ReadCommand { get; set; }
    }
}
