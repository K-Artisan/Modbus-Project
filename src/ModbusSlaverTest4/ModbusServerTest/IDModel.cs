using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusServer
{   
    /// <summary>
    /// 数据库中的id 
    /// 与IDEquipRes 一一对应
    /// </summary>
    class IDModel
    {
        private int devID;     //数据库中寄存器所属Modbus的Id
        public int DevID
        {
            get { return devID; }
            set { devID = value; }
        }

        private UpDataBase.RTWriteProxy.MType type; //类型
        public UpDataBase.RTWriteProxy.MType Type
        {
            get { return type; }
            set { type = value; }
        }

        private int pointID;   //数据库中一个点？
        public int PointID
        {
            get { return pointID; }
            set { pointID = value; }
        }

        public IDModel()
        {
            devID = 0;
            type = UpDataBase.RTWriteProxy.MType.AI;
            pointID = 0;
        }

        public IDModel(int idDev, UpDataBase.RTWriteProxy.MType myType, int idPoint)
        {
            devID = idDev;
            type    = myType;
            pointID = idPoint;
        }

        public IDModel(int idDev, string myType, int idPoint)
        {
            devID = idDev;
            pointID = idPoint;

            switch (myType.Trim())
            {
                case "AI":
                    {
                        type = UpDataBase.RTWriteProxy.MType.AI;
                        break;
                    }

                case "DI":
                    {
                        type = UpDataBase.RTWriteProxy.MType.DI;
                        break;
                    }
                case "AO":
                    {
                        type = UpDataBase.RTWriteProxy.MType.AO;
                        break;
                    }
                case "DO":
                    {
                        type = UpDataBase.RTWriteProxy.MType.DO;
                        break;
                    }
                case "ACC":
                    {
                        type = UpDataBase.RTWriteProxy.MType.ACC;
                        break;
                    }
            } //switch
        }

        /// <summary>
        /// 通过devID、type和pointID合成唯一的对象Id
        /// </summary>
        /// <returns>唯一的对象Id</returns>
        public ulong ToULongForIndex()
        {
            ulong result = 0;
            result = ((ulong)devID) << 32; //位运算, result高32位存储equipID

            ulong temp = (ulong)pointID;
            result = temp | result;       //或运算，result低32位存储regID

            //在equipID的所占的result高位32位中再用其高位8位存储type
            temp = (ulong)type;
            temp = temp << 56;
            result = temp | result;

            return result;
        }
    }


}
