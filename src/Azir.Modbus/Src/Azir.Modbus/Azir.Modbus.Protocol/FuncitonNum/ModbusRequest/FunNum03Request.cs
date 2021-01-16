using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.Protocol.FuncitonNum.Request
{
    /// <summary>
    /// 功能码03（03 H）<see cref="FunctionNumType.FunctionNum03"/> 
    /// 的请求帧的对象
    /// </summary>
    public class FunNum03Request
    {
        ///// <summary>
        ///// 单元标识符 (原“从站地址”)
        ///// </summary>
        //public byte DeviceAddress { get; set; }  
      
        /// <summary>
        /// MODBUS 功能码
        /// </summary>
        public FunctionNumType FunctionNum { get; set; } 
        /// <summary>
        /// 起始寄存器地址(占2个字节，10进制）
        /// </summary>
        public ushort StartingRegisterAddress { get; set; }      
        /// <summary>
        /// 将要读取的寄存器的个数(占2个字节，10进制）
        /// </summary>
        public ushort NumOfRegisterToRead { get; set; } 
    }
}
