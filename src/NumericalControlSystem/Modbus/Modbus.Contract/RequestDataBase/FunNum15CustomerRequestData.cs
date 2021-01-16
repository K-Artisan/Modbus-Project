using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modbus.Contract.RequestDataBase
{
    public class FunNum15CustomerRequestData : ICustomerRequestData
    {
        public byte DeviceAddress { get; set; }                      //设备地址
        public FunctionNumType FunctionNum { get; set; }             //功能码
        public ushort StartingCoilAddress{ get; set; }               //将要设置值的线圈(寄存器)起始地址
        public ushort NumOfCoilToForce { get; set; }

        /// <summary>
        /// 数据区：强制多个线圈的值
        /// 有效值：
        ///     True表示ON，
        ///     否则表示OFF
        /// </summary>
        private List<bool> forceData = new List<bool>();
        public List<bool> ForceData
        {
            get { return forceData; }
            set
            {
                forceData = value;
            }
        }

        public void InitializeNumOfCoilToForce()
        {
            this.NumOfCoilToForce = (ushort)this.ForceData.Count;
        }


        public List<FunNum15RequestDataBase> CovertToFunNum15RequestDataBases()
        {
            List<FunNum15RequestDataBase> funNum15RequestDataBases = new List<FunNum15RequestDataBase>();

            List<FunNum15CustomerRequestData> customerRequestDatas = Splite();

            funNum15RequestDataBases = CovertAllCustomerRequestDataToRequestDataBase(customerRequestDatas);

            return funNum15RequestDataBases;
        }


        private List<FunNum15CustomerRequestData> Splite()
        {
            List<FunNum15CustomerRequestData> customerRequestDatas = new List<FunNum15CustomerRequestData>();
            
            int canOperatingRegisterMaxNumOneTime = OperatingRegisteMaxNumOneTime.GetCanOperatingRegisterMaxNumOneTime(this.FunctionNum);
            InitializeNumOfCoilToForce();

            customerRequestDatas = CustomerRequstDataAuxiliary.Splite<FunNum15CustomerRequestData>(
                FunctionNumType.FunctionNum15, 
                this.NumOfCoilToForce);
            for (int i = 0; i < customerRequestDatas.Count; i++)
            {
                if (1 == customerRequestDatas.Count)
                {
                    customerRequestDatas[i].DeviceAddress = this.DeviceAddress;
                    customerRequestDatas[i].FunctionNum = this.FunctionNum;
                    customerRequestDatas[i].StartingCoilAddress = (ushort)(this.StartingCoilAddress + canOperatingRegisterMaxNumOneTime * i);
                    customerRequestDatas[i].NumOfCoilToForce = (ushort)this.ForceData.Count;
                    customerRequestDatas[i].ForceData = this.ForceData;
                }
                else if (1 < customerRequestDatas.Count)
                {
                    if (i < customerRequestDatas.Count - 1)  //第一个包到倒数第二个包
                    {
                        customerRequestDatas[i].DeviceAddress = this.DeviceAddress;
                        customerRequestDatas[i].FunctionNum = this.FunctionNum;
                        customerRequestDatas[i].StartingCoilAddress = (ushort)(this.StartingCoilAddress + canOperatingRegisterMaxNumOneTime * i);
                        customerRequestDatas[i].NumOfCoilToForce = (ushort)canOperatingRegisterMaxNumOneTime;                        
                        customerRequestDatas[i].ForceData = this.ForceData.GetRange(canOperatingRegisterMaxNumOneTime * i, canOperatingRegisterMaxNumOneTime);
                    }
                    else if (i == customerRequestDatas.Count - 1) //倒数第一个包
                    {
                        int retainForceDataCount = 0;
                        if (0 == (this.NumOfCoilToForce % canOperatingRegisterMaxNumOneTime))
                        {
                            retainForceDataCount = canOperatingRegisterMaxNumOneTime;
                        }
                       else
                        {
                            retainForceDataCount = this.NumOfCoilToForce % canOperatingRegisterMaxNumOneTime;
                        }
                        customerRequestDatas[i].DeviceAddress = this.DeviceAddress;
                        customerRequestDatas[i].FunctionNum = this.FunctionNum;
                        customerRequestDatas[i].StartingCoilAddress = (ushort)(this.StartingCoilAddress + canOperatingRegisterMaxNumOneTime * i);
                        customerRequestDatas[i].NumOfCoilToForce = (ushort)retainForceDataCount;                        
                        customerRequestDatas[i].ForceData = this.ForceData.GetRange(canOperatingRegisterMaxNumOneTime * i, retainForceDataCount);
                    }
                }
            }

            return customerRequestDatas;
        }

        private List<FunNum15RequestDataBase> CovertAllCustomerRequestDataToRequestDataBase(List<FunNum15CustomerRequestData> customerRequestDatas)
        {
            List<FunNum15RequestDataBase> requestDataBases = new List<FunNum15RequestDataBase>();

            foreach (FunNum15CustomerRequestData item in customerRequestDatas)
            {
                FunNum15RequestDataBase requestDataBase = CovertSingleCustomerRequestDataToSingleRequestDataBase(item);
                requestDataBases.Add(requestDataBase);
            }

            return requestDataBases;
        }

        private FunNum15RequestDataBase CovertSingleCustomerRequestDataToSingleRequestDataBase(FunNum15CustomerRequestData customerRequestData)
        {
            FunNum15RequestDataBase requestDataBase = new FunNum15RequestDataBase();

            requestDataBase.DeviceAddress = customerRequestData.DeviceAddress;
            requestDataBase.FunctionNum = Convert.ToByte(Convert.ToInt32(customerRequestData.FunctionNum));
            requestDataBase.StartingCoilAddressHigh = (byte)(customerRequestData.StartingCoilAddress / 256);
            requestDataBase.StartingCoilAddressLow = (byte)(customerRequestData.StartingCoilAddress % 256);
            requestDataBase.NumOfCoilToForceHigh = (byte)(customerRequestData.NumOfCoilToForce / 256);
            requestDataBase.NumOfCoilToForceLow = (byte)(customerRequestData.NumOfCoilToForce % 256);

            List<byte> forceDataTemp = new List<byte>();
            int bitCountInOneByte = 8;
            int byteCountInOneCustomerRequest = (int)Math.Ceiling((double)customerRequestData.ForceData.Count / (double)bitCountInOneByte);
            int retainByteCount = customerRequestData.ForceData.Count % bitCountInOneByte;

            //不满8位（一个字节的位数）用false（即：0）补齐
            //保证customerRequestData.ForceData.Count是8的整数倍
            for (int i = 0; i < retainByteCount; i++)
            {
                bool shortfall = false;
                customerRequestData.ForceData.Add(shortfall);
            }

            for (int i = 0; i < byteCountInOneCustomerRequest; i++)
            {
                byte requestByte = 0;
                int k = bitCountInOneByte * i;
                
                byte byte1 = customerRequestData.ForceData[k + 0] ? (byte)1 : (byte)0;   //一个字节中的最低位
                byte byte2 = customerRequestData.ForceData[k + 1] ? (byte)1 : (byte)0;
                byte byte3 = customerRequestData.ForceData[k + 2] ? (byte)1 : (byte)0;
                byte byte4 = customerRequestData.ForceData[k + 3] ? (byte)1 : (byte)0;
                byte byte5 = customerRequestData.ForceData[k + 4] ? (byte)1 : (byte)0;
                byte byte6 = customerRequestData.ForceData[k + 5] ? (byte)1 : (byte)0;
                byte byte7 = customerRequestData.ForceData[k + 6] ? (byte)1 : (byte)0;
                byte byte8 = customerRequestData.ForceData[k + 7] ? (byte)1 : (byte)0;   //一个字节中的最高位

                requestByte = (byte)(requestByte | (byte1 << 0));
                requestByte = (byte)(requestByte | (byte2 << 1));
                requestByte = (byte)(requestByte | (byte3 << 2));
                requestByte = (byte)(requestByte | (byte4 << 3));
                requestByte = (byte)(requestByte | (byte5 << 4));
                requestByte = (byte)(requestByte | (byte6 << 5));
                requestByte = (byte)(requestByte | (byte7 << 6));
                requestByte = (byte)(requestByte | (byte8 << 7));                                           
                                                               
                forceDataTemp.Add(requestByte);
            }

            requestDataBase.ForceData = forceDataTemp;

            return requestDataBase;
        }
    }
}
