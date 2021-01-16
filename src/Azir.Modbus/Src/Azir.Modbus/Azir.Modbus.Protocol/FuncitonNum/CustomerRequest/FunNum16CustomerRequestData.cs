using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Common;
using Azir.Modbus.Protocol.FuncitonNum.ModbusRequest;

namespace Azir.Modbus.Protocol.FuncitonNum.CustomerRequest
{
    /// <summary>
    /// 功能码16（10 H） 请求帧的客户端格式
    /// </summary>
    /// <typeparam name="T">T的类型只能为如下: 
    ///     double float int long short uint ulong ushort
    /// </typeparam>
    public class FunNum16CustomerRequestData<T> : IGenericCustomerRequestData<FunNum16RequestDataBase, T>
    {
        public byte DeviceAddress { get; set; }                      //设备地址
        public FunctionNumType FunctionNum { get; set; }             //功能码
        public ushort StartingRegisterAddress { get; set; }          //起始寄存器地址
        public int TotalNumOfPresetData { get; set; }                //设置值的个数

        //数据区:将要写入线圈(寄存器)的值
        private List<T> presetData = new List<T>();
        public List<T> PresetData
        {
            get
            {
                return presetData;
            }
            set
            {
                presetData = value;
                InilizationTotalNumOfPresetData();
            }
        }

        public DataAnalyzeMode DataAnalyzeMode { get; set; }

        private void InilizationTotalNumOfPresetData()
        {
            //int byteCountOfT = GetByteCountOfT();

            this.TotalNumOfPresetData = this.presetData.Count;
        }


        #region 构造函数

        public FunNum16CustomerRequestData()
        {
        }

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="dataAnalyzeMode">数据解析方式</param>
        public FunNum16CustomerRequestData(DataAnalyzeMode dataAnalyzeMode)
        {
            this.DataAnalyzeMode = dataAnalyzeMode;
        }

        #endregion

        /// <summary>
        /// 获取目标类型的所占字节数
        /// </summary>
        /// <returns></returns>
        private int GetByteCountOfT()
        {
            int byteCountOfT = GenericBitConverter.GetByteCountOfT<T>();
            return byteCountOfT;
        }

        public List<FunNum16RequestDataBase> CovertToFunNumRequestDataBases()
        {
            List<FunNum16RequestDataBase> funNum16RequestDataBases = new List<FunNum16RequestDataBase>();

            List<FunNum16CustomerRequestData<T>> funNum16CustomerRequestDatas = Splite();
            funNum16RequestDataBases = CovertAllCustomerRequestDataToRequestDataBase(funNum16CustomerRequestDatas);

            return funNum16RequestDataBases;
        }

        private List<FunNum16CustomerRequestData<T>> Splite()
        {
            List<FunNum16CustomerRequestData<T>> customerRequestDatas = new List<FunNum16CustomerRequestData<T>>();

            int canOperatingRegisterMaxNumOneTime = ModbusProtocolRule.GetCanOperatingRegisterMaxNumOneTime(this.FunctionNum);
            int byteCountOfT = GetByteCountOfT();
            int byteCountOfOneCustomerRequestData = byteCountOfT * canOperatingRegisterMaxNumOneTime;
            int presetDataCountOfOneCustomerRequestData = canOperatingRegisterMaxNumOneTime;

            InilizationTotalNumOfPresetData();
            customerRequestDatas = CustomerRequstDataAuxiliary.Splite<FunNum16CustomerRequestData<T>>(this.FunctionNum, this.TotalNumOfPresetData);

            for (int i = 0; i < customerRequestDatas.Count; i++)
            {
                if (1 == customerRequestDatas.Count)
                {
                    customerRequestDatas[i].DeviceAddress = this.DeviceAddress;
                    customerRequestDatas[i].FunctionNum = this.FunctionNum;
                    customerRequestDatas[i].StartingRegisterAddress = (ushort)(this.StartingRegisterAddress + byteCountOfOneCustomerRequestData * i);
                    customerRequestDatas[i].PresetData = this.PresetData;
                    customerRequestDatas[i].InilizationTotalNumOfPresetData();
                }
                else if (1 < customerRequestDatas.Count)
                {
                    if (i < customerRequestDatas.Count - 1)  //第一个包到倒数第二个包
                    {
                        customerRequestDatas[i].DeviceAddress = this.DeviceAddress;
                        customerRequestDatas[i].FunctionNum = this.FunctionNum;
                        customerRequestDatas[i].StartingRegisterAddress = (ushort)(this.StartingRegisterAddress + byteCountOfOneCustomerRequestData * i);
                        customerRequestDatas[i].PresetData = this.PresetData.GetRange(presetDataCountOfOneCustomerRequestData * i, presetDataCountOfOneCustomerRequestData);
                        customerRequestDatas[i].InilizationTotalNumOfPresetData();
                    }
                    else if (i == customerRequestDatas.Count - 1) //倒数第一个包
                    {
                        int retainPresetDataCount = 0;
                        if (0 == (this.TotalNumOfPresetData % presetDataCountOfOneCustomerRequestData))
                        {
                            retainPresetDataCount = presetDataCountOfOneCustomerRequestData;
                        }
                        else
                        {
                            retainPresetDataCount = this.TotalNumOfPresetData % presetDataCountOfOneCustomerRequestData;
                        }
                        customerRequestDatas[i].DeviceAddress = this.DeviceAddress;
                        customerRequestDatas[i].FunctionNum = this.FunctionNum;
                        customerRequestDatas[i].StartingRegisterAddress = (ushort)(this.StartingRegisterAddress + byteCountOfOneCustomerRequestData * i);
                        customerRequestDatas[i].PresetData = this.PresetData.GetRange(presetDataCountOfOneCustomerRequestData * i, retainPresetDataCount);
                        customerRequestDatas[i].InilizationTotalNumOfPresetData();
                    }
                }
            }

            return customerRequestDatas;
        }

        private List<FunNum16RequestDataBase> CovertAllCustomerRequestDataToRequestDataBase(List<FunNum16CustomerRequestData<T>> funNum16CustomerRequestDatas)
        {
            List<FunNum16RequestDataBase> funNum16RequestDataBases = new List<FunNum16RequestDataBase>();

            foreach (FunNum16CustomerRequestData<T> item in funNum16CustomerRequestDatas)
            {
                FunNum16RequestDataBase requestDataBase = CovertSingleCustomerRequestDataToSingleRequestDataBase(item);

                funNum16RequestDataBases.Add(requestDataBase);
            }

            return funNum16RequestDataBases;
        }

        private FunNum16RequestDataBase CovertSingleCustomerRequestDataToSingleRequestDataBase(FunNum16CustomerRequestData<T> customerRequestData)
        {
            int byteCountOfT = GetByteCountOfT();
            FunNum16RequestDataBase requestDataBase = new FunNum16RequestDataBase();

            requestDataBase.DeviceAddress = customerRequestData.DeviceAddress;
            requestDataBase.FunctionNum = Convert.ToByte(Convert.ToInt32(customerRequestData.FunctionNum));
            requestDataBase.StartingRegisterAddressHigh = (byte)(customerRequestData.StartingRegisterAddress / 256);
            requestDataBase.StartingRegisterAddressLow = (byte)(customerRequestData.StartingRegisterAddress % 256);

            List<byte> presetDataTemp = new List<byte>();
            foreach (T singlePresetData in customerRequestData.PresetData)
            {
                //LH LH...
                byte[] bytePresetData = GenericBitConverter.GetBytes<T>(singlePresetData);

                if (this.DataAnalyzeMode == DataAnalyzeMode.DataHighToLow)
                {
                    //HL HL
                    for (int i = 0; i <= bytePresetData.Length / 2 && i < bytePresetData.Length; i += 2)
                    {
                        presetDataTemp.Add(bytePresetData[i + 1]); //先加入高位
                        presetDataTemp.Add(bytePresetData[i]);     //后加入低位
                    }
                }
                else
                {
                    //LH LH
                    presetDataTemp.AddRange(bytePresetData.ToList());
                }

            }
            requestDataBase.PresetData = presetDataTemp;

            return requestDataBase;
        }



    }
}
