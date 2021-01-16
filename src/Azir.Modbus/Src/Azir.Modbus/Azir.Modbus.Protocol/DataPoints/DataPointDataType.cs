using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.Protocol.DataPoints
{
    /// <summary>
    /// 数据点的数据类型
    /// </summary>
    public enum DataPointDataType
    {
        /// <summary>
        /// //short,  占2字节 ,占1个寄存器
        /// </summary>
        S16, 
        /// <summary>
        /// ushort  占2字节 ,占1个寄存器
        /// </summary>
        U16,
        /// <summary>
        /// int     占4字节 ,占2个寄存器
        /// </summary>
        S32, 
        /// <summary>
        /// uint    占4字节 ,占2个寄存器
        /// </summary>
        U32,
        /// <summary>
        /// //long    占8字节 ,占4个寄存器
        /// </summary>
        S64, 
        /// <summary>
        /// ulong   占8字节 ,占4个寄存器
        /// </summary>
        U64, 
        /// <summary>
        /// float   占4字节 ,占2个寄存器
        /// </summary>
        F32, 
        /// <summary>
        /// double  占8字节 ,占4个寄存器
        /// </summary>
        D64, 
        /// <summary>
        /// Bool    占1位?   ,占1个寄存器
        /// </summary>
        Bit  
    }
}
