using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modbus.Common;

namespace Modbus.Contract.RequestDataBase
{
    /// <summary>
    /// 功能码06（06 H） 请求帧的客户端格式
    /// </summary>
    /// <typeparam name="T">PresetData的类型：
    /// T只能为: short ushort
    /// T不能为: double float int long  uint ulong 
    /// 因为：功能码06是设置单个寄存器（占两字节，所以T的类型只能为short ushort）的值
    /// </typeparam>
    public class FunNum06CustomerRequestData<T> : IGenericCustomerRequestData<T> 
    {
        public byte DeviceAddress { get; set; }               //设备地址
        public FunctionNumType FunctionNum { get; set; }      //功能码
        public ushort RegisterAddress { get; set; }           //将要设置值的寄存器地址
    
        /// <summary>
        /// 将要写入寄存器的值.
        /// 取值范围：
        ///     32位有符号整数：-2147483648 -- +2147483647
        /// </summary>
        //public int  PresetData { get; set; }     
        
        /// <summary>
        /// 将要写入寄存器的值,
        /// 因为：功能码06是设置单个寄存器（占两字节，
        /// 所以T的类型只能为short ushort）的值
        /// </summary> 
        public T PresetData { get; set; }

        public DataAnalyzeMode DataAnalyzeMode { get; set; }


        //TODO:测试用，可删除
        #region PresetData 的类型为int时候,测试用，可删除

        //public List<FunNum06RequestDataBase> CovertToFunNum06RequestDataBases()
        //{
        //    List<FunNum06RequestDataBase> funNum06RequestDataBases = new List<FunNum06RequestDataBase>();

        //    if (Int16.MinValue <= PresetData && PresetData <= Int16.MaxValue)
        //    {
        //        //16位的值

        //        FunNum06RequestDataBase funNum06RequestDataBase = new FunNum06RequestDataBase();

        //        funNum06RequestDataBase.DeviceAddress = this.DeviceAddress;
        //        funNum06RequestDataBase.FunctionNum =  Convert.ToByte(Convert.ToInt32(this.FunctionNum));
        //        funNum06RequestDataBase.RegisterAddressHigh = (byte)(this.RegisterAddress / 256);
        //        funNum06RequestDataBase.RegisterAddressLow = (byte)(this.RegisterAddress % 256);
        //        funNum06RequestDataBase.PresetDataHigh = (byte)(this.PresetData / 256);
        //        funNum06RequestDataBase.PresetDataLow = (byte)(this.PresetData % 256);

        //        ///TODO:测试,可删除
        //        byte[] presetDatabyte = BitConverter.GetBytes(this.PresetData);

        //        funNum06RequestDataBases.Add(funNum06RequestDataBase);
        //    }
        //    else if ((Int32.MinValue<= PresetData && PresetData < Int16.MinValue)
        //        || (Int16.MaxValue < PresetData && PresetData <= Int32.MaxValue))

        //    {
        //        //32位的值:转化为字节数组后，从高位到低位顺序设为：[高2][低2][高1][低1]
        //        //分两次发送：
        //        //第一次：设置的值为：32位值的[高1][低1]
        //        //第二次：寄存器地址加1，设置的值为：32位值的[高2][低2]

        //        byte[] bytePresetData =  BitConverter.GetBytes(PresetData);

        //        //TODO:测试,可删除
        //        int intPresetData = BitConverter.ToInt32(bytePresetData, 0);

        //        for (int i = 0, k = 0; i < 2; i++, k+=2)
        //        {
        //            FunNum06RequestDataBase funNum06RequestDataBase = new FunNum06RequestDataBase();

        //            funNum06RequestDataBase.DeviceAddress = this.DeviceAddress;
        //            funNum06RequestDataBase.FunctionNum = Convert.ToByte(Convert.ToInt32(this.FunctionNum));
        //            funNum06RequestDataBase.RegisterAddressHigh = (byte)((this.RegisterAddress + i) / 256);
        //            funNum06RequestDataBase.RegisterAddressLow = (byte)((this.RegisterAddress + i) % 256);
        //            funNum06RequestDataBase.PresetDataHigh = bytePresetData[k + 1];
        //            funNum06RequestDataBase.PresetDataLow = bytePresetData[k];

        //            funNum06RequestDataBases.Add(funNum06RequestDataBase);
        //        }

        //    }
        //    return funNum06RequestDataBases;
        //}

        #endregion

        public List<FunNum06RequestDataBase> CovertToFunNum06RequestDataBases()
        {
            List<FunNum06RequestDataBase> funNum06RequestDataBases = new List<FunNum06RequestDataBase>();

            //LH LH ....
            byte[] bytePresetData = GenericBitConverter.GetBytes<T>(PresetData);
            if (null == bytePresetData)
            {
                return funNum06RequestDataBases;
            }

            int countOfRequestDataBase = bytePresetData.Length / 2;
            for (int i = 0, k = 0; i <= countOfRequestDataBase && k < bytePresetData.Length; i++, k += 2)
            {
                FunNum06RequestDataBase funNum06RequestDataBase = new FunNum06RequestDataBase();

                funNum06RequestDataBase.DeviceAddress = this.DeviceAddress;
                funNum06RequestDataBase.FunctionNum = Convert.ToByte(Convert.ToInt32(this.FunctionNum));
                funNum06RequestDataBase.RegisterAddressHigh = (byte)((this.RegisterAddress + i) / 256);
                funNum06RequestDataBase.RegisterAddressLow = (byte)((this.RegisterAddress + i) % 256);

                //bytePresetData:LH LH ....
                //注意：
                //funNum06RequestDataBase.PresetDataHigh 表示第一个字节
                //funNum06RequestDataBase.PresetDataHigh 表示第二个字节
                //已经不是它们字面的意思了

                //根据数据解析模式反转数据高低位
                if (this.DataAnalyzeMode == DataAnalyzeMode.DataHighToLow)
                {
                    //HL　HL
                    funNum06RequestDataBase.PresetDataHigh = bytePresetData[k + 1]; //第一个字节被赋值bytePresetData的高位
                    funNum06RequestDataBase.PresetDataLow = bytePresetData[k];      
                }
                else
                {
                    //LH LH
                    funNum06RequestDataBase.PresetDataHigh = bytePresetData[k];    //第一个字节被赋值bytePresetData的低位
                    funNum06RequestDataBase.PresetDataLow = bytePresetData[k + 1];
                }

                funNum06RequestDataBases.Add(funNum06RequestDataBase);
            }

            return funNum06RequestDataBases;
        }

    }
}
