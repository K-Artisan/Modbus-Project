using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modbus.Contract.RequestDataBase
{
    public class FunNum16RequestDataBase : IFunNumRequestDataBase
    {
        public byte DeviceAddress { get; set; }                      //设备地址
        public byte FunctionNum { get; set; }                        //功能码
        public byte StartingRegisterAddressHigh { get; set; }        //起始寄存器地址高8位
        public byte StartingRegisterAddressLow { get; set; }         //起始寄存器地址低8位
        public byte NumOfRegisterToPresetHigh { get; private set; }  //将要设置的寄存器个数的高8位
        public byte NumOfRegisterToPresetdLow { get; private set; }  //将要设置的寄存器个数的低8位
        public byte ByteCountOfPresetData { get; private set; }      //数据区(PresetData)的字节数

        private List<byte> presetData;                               //数据区:将要写入线圈(寄存器)的值
        public List<byte> PresetData
        {
            get 
            { 
                return presetData;
            }
            set 
            { 
                presetData = value;
                InilizationOtherMembers();
            }
        }

        private void InilizationOtherMembers()
        {
            int numOfRegister = this.presetData.Count / 2;
            NumOfRegisterToPresetHigh = (byte)(numOfRegister / 256);
            NumOfRegisterToPresetdLow = (byte)(numOfRegister % 256);
            ByteCountOfPresetData = (byte)presetData.Count;
        }

        #region IFunNumRequestDataBase

        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();

            byteList.Add(DeviceAddress);
            byteList.Add(FunctionNum);
            byteList.Add(StartingRegisterAddressHigh);
            byteList.Add(StartingRegisterAddressLow);
            byteList.Add(NumOfRegisterToPresetHigh);
            byteList.Add(NumOfRegisterToPresetdLow);
            byteList.Add(ByteCountOfPresetData);
            byteList.AddRange(PresetData);

            return byteList;
        }

        #endregion
    }
}
