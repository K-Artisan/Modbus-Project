using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modbus.Contract.RequestDataBase
{
    /// <summary>
    /// 功能码15（0F H）的请求帧基础类:
    /// TCP、RTU等共用
    /// </summary>
    public class FunNum15RequestDataBase : IFunNumRequestDataBase
    {
        public byte DeviceAddress { get; set; }                      //设备地址
        public byte FunctionNum { get; set; }                        //功能码
        public byte StartingCoilAddressHigh { get; set; }            //将要设置值的线圈(寄存器)起始地址的高8位
        public byte StartingCoilAddressLow { get; set; }             //将要设置值的线圈(寄存器)起始地址的低8位
        public byte NumOfCoilToForceHigh { get; set; }               //将要设置的线圈(寄存器)个数的高8位
        public byte NumOfCoilToForceLow { get; set; }                //将要设置的线圈(寄存器)个数的低8位
        public byte ByteCountOfForceData { get; private set; }       //数据区(ForceData)的字节数

        //数据区:将要写入线圈(寄存器)的值
        private List<byte> forceData;
        public List<byte> ForceData
        {
            get 
            {
                return forceData; 
            }
            set 
            { 
                forceData = value;
                InilizationOtherMembers();
            }
        }

        private void InilizationOtherMembers()
        {
            ByteCountOfForceData = (byte)forceData.Count;            
        }                   


        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();

            byteList.Add(DeviceAddress);
            byteList.Add(FunctionNum);
            byteList.Add(StartingCoilAddressHigh);
            byteList.Add(StartingCoilAddressLow);
            byteList.Add(NumOfCoilToForceHigh);
            byteList.Add(NumOfCoilToForceLow);
            byteList.Add(ByteCountOfForceData);
            byteList.AddRange(ForceData);

            return byteList;
        }
    }
}
