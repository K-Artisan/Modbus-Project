using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ModbusServer
{
    class DBModbusID
    {
        int devId;                          //Modbus设备Id       
        public int DevId
        {
            get { return devId; }
            set { devId = value; }
        }

        int pointId;                        //点Id
        public int PointId
        {
            get { return PointId; }
            set { PointId = value; }
        }

        UpDataBase.RTWriteProxy.MType type; //类型
        public UpDataBase.RTWriteProxy.MType Type
        {
            get { return type; }
            set { type = value; }
        }

        DateTime dateTime;           //时间
        public System.DateTime DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }

        public DBModbusID()
        {
            devId = 0;
            pointId = 0;
            type = UpDataBase.RTWriteProxy.MType.AI;
            dateTime = DateTime.Now;
        }



    }
}
