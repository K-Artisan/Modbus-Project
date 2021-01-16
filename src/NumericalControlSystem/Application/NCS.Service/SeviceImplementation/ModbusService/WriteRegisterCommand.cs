using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Service.SeviceImplementation.ModbusService
{
    public class WriteRegisterCommand
    {
        private bool writerRegisterSuccess = true;
        public bool WriterRegisterSuccess
        {
            get { return writerRegisterSuccess; }
            set { writerRegisterSuccess = value; }
        }

        private int writerRegisterFalseCount = 0;
        public int WriterRegisterFalseCount
        {
            get { return writerRegisterFalseCount; }
            set { writerRegisterFalseCount = value; }
        }

        private List<byte> writeCommand = new List<byte>();
        public List<byte> WriteCommand
        {
            get { return writeCommand; }
            set { writeCommand = value; }
        }
    }
}
