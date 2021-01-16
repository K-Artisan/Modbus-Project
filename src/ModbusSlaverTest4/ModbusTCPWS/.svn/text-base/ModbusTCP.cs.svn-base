using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ModbusTCPWS
{
    public partial class ModbusTCP : ServiceBase
    {
        System.Threading.Thread t;
        ModbusServer.Modbus modbus;
        public ModbusTCP()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            t = new System.Threading.Thread(ThreadS);
            t.Start();
        }

        protected override void OnStop()
        {
            try
            {
                t.Abort();
            }
            catch
            {
            }
            try
            {
                modbus.Dispose();
            }
            catch
            {
            }
        }

        private void ThreadS()
        {
            modbus = new ModbusServer.Modbus();
            modbus.StartModbus(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ModbusTCPCfg.xml"));
            while (true)
            {
                System.Threading.Thread.Sleep(15000);
            }
        }
    }
}
