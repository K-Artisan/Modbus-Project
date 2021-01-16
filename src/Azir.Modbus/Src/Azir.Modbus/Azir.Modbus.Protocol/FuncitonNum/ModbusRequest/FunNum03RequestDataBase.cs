using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.Protocol.FuncitonNum.ModbusRequest
{
    /// <summary>
    /// 功能码03（03 H）的请求帧基础类:
    /// TCP、RTU等共用
    /// </summary>
    public class FunNum03RequestDataBase : IFunNumRequestDataBase
    {
        /// <summary>
        /// 设备地址(TCP中称单元标识符号，RTU中称从站地址)
        /// </summary>
        public byte DeviceAddress { get; set; }  
        /// <summary>
        /// 功能码
        /// </summary>
        public byte FunctionNum { get; set; }                                   
        /// <summary>
        /// 起始寄存器地址高8位
        /// </summary>
        public byte StartingRegisterAddressHigh { get; set; }                   
        /// <summary>
        /// 起始寄存器地址低8位
        /// </summary>
        public byte StartingRegisterAddressLow { get; set; }  
        /// <summary>
        /// 将要读取的寄存器个数的高8位
        /// </summary>
        public byte NumOfRegisterToReadHigh { get; set; }                       
        /// <summary>
        /// 将要读取的寄存器个数的低8位
        /// </summary>
        public byte NumOfRegisterToReadLow { get; set; }                        

        /// <summary>
        /// 转行成字节集合
        /// </summary>
        /// <returns></returns>
        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();

            byteList.Add(DeviceAddress);
            byteList.Add(FunctionNum);
            byteList.Add(StartingRegisterAddressHigh);
            byteList.Add(StartingRegisterAddressLow);
            byteList.Add(NumOfRegisterToReadHigh);
            byteList.Add(NumOfRegisterToReadLow);

            return byteList;
        }
    }
}
