using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusServer
{
    class ResModbusID
    {
        int resId;             //寄存器id
        public int ResId
        {
            get { return resId; }
            set { resId = value; }
        }
        double resValue;       //寄存器值
        public double ResValue
        {
            get { return resValue; }
            set { resValue = value; }
        }
        int devId;             //寄存器所属设备id
        public int DevId
        {
            get { return devId; }
            set { devId = value; }
        }

        public ResModbusID()
        {
            resId = 0;
            resValue = 0;
            devId = 0;
        }
    }
}
