using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modbus.Contract.RequestDataBase
{
    /// <summary>
    /// 功能码05（05 H）的请求帧基础类:
    /// TCP、RTU等共用,
    /// 注意：ForceDataHigh和ForceDataLow组合的只有两种值,其它值无效：
    /// FF 00:表示ON
    /// 00 00:表示OFF
    /// </summary>
    public class FunNum05RequestDataBase : IFunNumRequestDataBase
    {
        public byte DeviceAddress { get; set; }          //设备地址
        public byte FunctionNum { get; set; }            //功能码
        public byte CoilAddressHigh { get; set; }        //将要设置值的线圈(寄存器)地址的高8位
        public byte CoilAddressLow { get; set; }         //将要设置值的线圈(寄存器)地址的低8位
        public byte ForceDataHigh { get; set; }          //将要强制单个线圈的值的高8位
        public byte ForceDataLow { get; set; }           //将要强制单个线圈的值的低8位

        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();

            byteList.Add(DeviceAddress);
            byteList.Add(FunctionNum);
            byteList.Add(CoilAddressHigh);
            byteList.Add(CoilAddressLow);
            byteList.Add(ForceDataHigh);
            byteList.Add(ForceDataLow);

            return byteList;
        }
    }
}
