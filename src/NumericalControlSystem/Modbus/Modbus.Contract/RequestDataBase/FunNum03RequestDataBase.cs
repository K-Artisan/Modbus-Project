using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modbus.Contract.RequestDataBase
{
    /// <summary>
    /// 功能码03（03 H）的请求帧基础类:
    /// TCP、RTU等共用
    /// </summary>
    public class FunNum03RequestDataBase : IFunNumRequestDataBase
    {
        public byte DeviceAddress { get; set; }                                  //设备地址
        public byte FunctionNum { get; set; }                                    //功能码
        public byte StartingRegisterAddressHigh { get; set; }                    //起始寄存器地址高8位
        public byte StartingRegisterAddressLow { get; set; }                     //起始寄存器地址低8位
        public byte NumOfRegisterToReadHigh { get; set; }                        //将要读取的寄存器个数的高8位
        public byte NumOfRegisterToReadLow { get; set; }                         //将要读取的寄存器个数的低8位

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
