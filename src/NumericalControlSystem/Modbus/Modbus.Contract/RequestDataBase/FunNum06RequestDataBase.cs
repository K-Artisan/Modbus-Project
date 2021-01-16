using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modbus.Contract.RequestDataBase
{
    /// <summary>
    /// 功能码06（06 H）的请求帧基础类:
    /// TCP、RTU等共用
    /// </summary>
    public class FunNum06RequestDataBase　: IFunNumRequestDataBase
    {
        public byte DeviceAddress { get; set; }           //设备地址
        public byte FunctionNum { get; set; }             //功能码
        public byte RegisterAddressHigh { get; set; }     //将要设置值的寄存器地址的高8位
        public byte RegisterAddressLow { get; set; }      //将要设置值的寄存器地址的低8位

        //注意：
        //PresetDataHigh 表示第一个字节
        //PresetDataHigh 表示第二个字节
        //已经不是它们字面的意思了
        public byte PresetDataHigh { get; set; }          //PresetDataHigh 表示第一个字节，而不是表示将要写入寄存器的值的高8位
        public byte PresetDataLow { get; set; }           //PresetDataHigh 表示第一个字节，而不是表示将要写入寄存器的值的低8位

        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();

            byteList.Add(DeviceAddress);
            byteList.Add(FunctionNum);
            byteList.Add(RegisterAddressHigh);
            byteList.Add(RegisterAddressLow);
            byteList.Add(PresetDataHigh);
            byteList.Add(PresetDataLow);

            return byteList;
        }
    }
}
