using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusServer
{   
    /// <summary>
    /// Modbus设备中寄存器的唯一标识。
    /// 与IDModel一一对应
    /// </summary>
    class IDEquipRes
    {
        private int equipID;     //寄存器所属Modbus的Id
        public int EquipID
        {
            get { return equipID; }
            set { equipID = value; }
        }

        private int regID;       //寄存器id
        public int RegID
        {
            get { return regID; }
            set { regID = value; }
        }

        public IDEquipRes()
        {
            equipID = 0;
            regID   = 0;
        }

        public IDEquipRes(int equId, int rId)
        {
            equipID = equId;
            regID = rId;
        }

        /// <summary>
        /// 通过equipID和regID合成唯一的对象Id
        /// </summary>
        /// <returns>唯一的对象Id</returns>
        public ulong ToULongForIndex()
        {
            ulong result = 0;
            result = ((ulong)equipID) << 32; //位运算, result高32位存储equipID,equipID为int，最大值约为21亿

            ulong temp = (ulong)regID;
            result = temp | result; //或运算，result低32位存储regID

            return result;
        }
    }
}
