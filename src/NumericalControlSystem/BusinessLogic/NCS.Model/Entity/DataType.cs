using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Model.Entity
{
    public enum DataType
    {
        S16, //short,  占2字节 ,占1个寄存器
        U16, //ushort  占2字节 ,占1个寄存器
        S32, //int     占4字节 ,占2个寄存器
        U32, //uint    占4字节 ,占2个寄存器
        S64, //long    占8字节 ,占4个寄存器
        U64, //ulong   占8字节 ,占4个寄存器
        F32, //float   占4字节 ,占2个寄存器
        D64, //double  占8字节 ,占4个寄存器
        Bit  //Bool            ,占1个寄存器
    }
}
