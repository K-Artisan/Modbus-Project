using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Modbus modbus = new Modbus())
            {
               // modbus.StartModbus("..\\modbusConfig.xml");
                modbus.StartModbus(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ModbusTCPCfg.xml"));
            }

        }
    }
}
