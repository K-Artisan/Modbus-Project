using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.FuncitonNum.ModbusRequest;

namespace Azir.Modbus.Protocol.FuncitonNum.CustomerRequest
{
    public class FunNum01CustomerRequestData : ICustomerRequestData<FunNum01RequestDataBase>
    {
        public byte DeviceAddress { get; set; }                         //设备地址
        public FunctionNumType FunctionNum { get; set; }                //功能码
        public ushort StartingRegister { get; set; }                    //起始寄存器地址 
        public ushort NumOfRegisterToRead { get; set; }                 //将要读取的寄存器的个数

        public List<FunNum01RequestDataBase> CovertToFunNumRequestDataBases()
        {
            List<FunNum01RequestDataBase> requestDataBases = new List<FunNum01RequestDataBase>();

            List<FunNum01CustomerRequestData> customerRequestDatas = Splite();
            foreach (FunNum01CustomerRequestData item in customerRequestDatas)
            {
                FunNum01RequestDataBase requestDataBase = CovertToSingleFunNum01RequestDataBase(item);
                requestDataBases.Add(requestDataBase);
            }

            return requestDataBases;
        }

        private List<FunNum01CustomerRequestData> Splite()
        {
            int canOperatingRegisterMaxNumOneTime = ModbusProtocolRule.GetCanOperatingRegisterMaxNumOneTime(this.FunctionNum);
            List<FunNum01CustomerRequestData> customerRequestDatas = new List<FunNum01CustomerRequestData>();

            customerRequestDatas = CustomerRequstDataAuxiliary.Splite<FunNum01CustomerRequestData>(this.FunctionNum, this.NumOfRegisterToRead);
            for (int i = 0; i < customerRequestDatas.Count; i++)
            {
                if (1 == customerRequestDatas.Count)
                {
                    customerRequestDatas[i].DeviceAddress = this.DeviceAddress;
                    customerRequestDatas[i].FunctionNum = this.FunctionNum;
                    customerRequestDatas[i].StartingRegister = (ushort)(this.StartingRegister + canOperatingRegisterMaxNumOneTime * i);
                    customerRequestDatas[i].NumOfRegisterToRead = this.NumOfRegisterToRead;//(ushort)canOperatingRegisterMaxNumOneTime;
                }
                else if (1 < customerRequestDatas.Count)
                {
                    if (i < customerRequestDatas.Count - 1)  //第一个包到倒数第二个包
                    {
                        customerRequestDatas[i].DeviceAddress = this.DeviceAddress;
                        customerRequestDatas[i].FunctionNum = this.FunctionNum;
                        customerRequestDatas[i].StartingRegister = (ushort)(this.StartingRegister + canOperatingRegisterMaxNumOneTime * i);
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
                        customerRequestDatas[i].StartingRegister = (ushort)(this.StartingRegister + canOperatingRegisterMaxNumOneTime * i);
                        customerRequestDatas[i].NumOfRegisterToRead = (ushort)retainRegisterCount;
                    }
                }
            }

            return customerRequestDatas;
        }

        private FunNum01RequestDataBase CovertToSingleFunNum01RequestDataBase(FunNum01CustomerRequestData funNumCustomerRequestData)
        {
            FunNum01RequestDataBase funNumRequestDataBase = new FunNum01RequestDataBase();

            funNumRequestDataBase.DeviceAddress = funNumCustomerRequestData.DeviceAddress;
            funNumRequestDataBase.FunctionNum = Convert.ToByte(Convert.ToInt32(funNumCustomerRequestData.FunctionNum));
            funNumRequestDataBase.StartingRegisterAddressHigh = (byte)(funNumCustomerRequestData.StartingRegister / 256);
            funNumRequestDataBase.StartingRegisterAddressLow = (byte)(funNumCustomerRequestData.StartingRegister % 256);
            funNumRequestDataBase.NumOfRegisterToReadHigh = (byte)(funNumCustomerRequestData.NumOfRegisterToRead / 256);
            funNumRequestDataBase.NumOfRegisterToReadLow = (byte)(funNumCustomerRequestData.NumOfRegisterToRead % 256);

            return funNumRequestDataBase;
        }
    }
}
