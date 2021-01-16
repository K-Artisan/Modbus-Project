using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.FuncitonNum.ModbusRequest;

namespace Azir.Modbus.Protocol.FuncitonNum.CustomerRequest
{
    /// <summary>
    /// 功能码03（03 H） 请求帧的客户端格式
    /// </summary>
    public class FunNum03CustomerRequestData : ICustomerRequestData<FunNum03RequestDataBase>
    {
        /// <summary>
        /// 设备地址
        /// </summary>
        public byte DeviceAddress { get; set; }   
        /// <summary>
        /// 功能码
        /// </summary>
        public FunctionNumType FunctionNum { get; set; }

        /// <summary>
        /// 起始寄存器地址 
        /// </summary>
      
        public ushort StartingRegisterAddress { get; set; }   
        /// <summary>
        /// 将要读取的寄存器的个数
        /// </summary>
        public ushort NumOfRegisterToRead { get; set; }                 

        public FunNum03CustomerRequestData()
        {
        }

        public List<FunNum03RequestDataBase> CovertToFunNumRequestDataBases()
        {
            List<FunNum03RequestDataBase> funNum03RequestDataBases = new List<FunNum03RequestDataBase>();

            List<FunNum03CustomerRequestData> funNum03CustomerRequestDatas = Splite();
            foreach (FunNum03CustomerRequestData item in funNum03CustomerRequestDatas)
            {
                FunNum03RequestDataBase funNum03RequestDataBase = CovertToSingleFunNum03RequestDataBase(item);
                funNum03RequestDataBases.Add(funNum03RequestDataBase);
            }

            return funNum03RequestDataBases;
        }

        /// <summary>
        /// 将每个请求帧的客户端格式分成更小的请求帧：
        /// 原因：每次可以操作的寄存器个数有限。
        /// </summary>
        /// <returns>若干个小包</returns>
        private List<FunNum03CustomerRequestData> Splite()
        {
            int canOperatingRegisterMaxNumOneTime = ModbusProtocolRule.GetCanOperatingRegisterMaxNumOneTime(this.FunctionNum);
            List<FunNum03CustomerRequestData> customerRequestDatas = null;

            customerRequestDatas = CustomerRequstDataAuxiliary.Splite<FunNum03CustomerRequestData>(this.FunctionNum, this.NumOfRegisterToRead);

            for (int i = 0; i < customerRequestDatas.Count; i++)
            {
                if (1 == customerRequestDatas.Count)
                {
                    customerRequestDatas[i].DeviceAddress = this.DeviceAddress;
                    customerRequestDatas[i].FunctionNum = this.FunctionNum;
                    customerRequestDatas[i].StartingRegisterAddress = (ushort)(this.StartingRegisterAddress + canOperatingRegisterMaxNumOneTime * i);
                    customerRequestDatas[i].NumOfRegisterToRead = this.NumOfRegisterToRead;
                }
                else if (1 < customerRequestDatas.Count)
                {
                    if (i < customerRequestDatas.Count - 1)  //第一个包到倒数第二个包
                    {
                        customerRequestDatas[i].DeviceAddress = this.DeviceAddress;
                        customerRequestDatas[i].FunctionNum = this.FunctionNum;
                        customerRequestDatas[i].StartingRegisterAddress = (ushort)(this.StartingRegisterAddress + canOperatingRegisterMaxNumOneTime * i);
                        customerRequestDatas[i].NumOfRegisterToRead = (ushort)canOperatingRegisterMaxNumOneTime;
                    }
                    else if (i == customerRequestDatas.Count - 1) //倒数第一个包
                    {
                        int retainRegisterCount = 0;
                        if (0 == (this.NumOfRegisterToRead % canOperatingRegisterMaxNumOneTime))
                        {
                            retainRegisterCount = canOperatingRegisterMaxNumOneTime;
                        }
                        else
                        {
                            retainRegisterCount = this.NumOfRegisterToRead % canOperatingRegisterMaxNumOneTime;
                        }

                        customerRequestDatas[i].DeviceAddress = this.DeviceAddress;
                        customerRequestDatas[i].FunctionNum = this.FunctionNum;
                        customerRequestDatas[i].StartingRegisterAddress = (ushort)(this.StartingRegisterAddress + canOperatingRegisterMaxNumOneTime * i);
                        customerRequestDatas[i].NumOfRegisterToRead = (ushort)retainRegisterCount;
                    }
                }
            }

            return customerRequestDatas;
        }

        private FunNum03RequestDataBase CovertToSingleFunNum03RequestDataBase(FunNum03CustomerRequestData funNum03CustomerRequestData)
        {
            FunNum03RequestDataBase funNumRequestDataBase = new FunNum03RequestDataBase();

            funNumRequestDataBase.DeviceAddress = funNum03CustomerRequestData.DeviceAddress;
            funNumRequestDataBase.FunctionNum = Convert.ToByte(Convert.ToInt32(funNum03CustomerRequestData.FunctionNum));
            funNumRequestDataBase.StartingRegisterAddressHigh = (byte)(funNum03CustomerRequestData.StartingRegisterAddress / 256);
            funNumRequestDataBase.StartingRegisterAddressLow = (byte)(funNum03CustomerRequestData.StartingRegisterAddress % 256);
            funNumRequestDataBase.NumOfRegisterToReadHigh = (byte)(funNum03CustomerRequestData.NumOfRegisterToRead / 256);
            funNumRequestDataBase.NumOfRegisterToReadLow = (byte)(funNum03CustomerRequestData.NumOfRegisterToRead % 256);

            return funNumRequestDataBase;
        }
    }
}
