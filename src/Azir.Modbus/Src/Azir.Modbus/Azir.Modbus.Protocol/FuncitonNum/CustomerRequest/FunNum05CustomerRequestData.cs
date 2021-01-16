using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.FuncitonNum.ModbusRequest;

namespace Azir.Modbus.Protocol.FuncitonNum.CustomerRequest
{
    public class FunNum05CustomerRequestData : ICustomerRequestData<FunNum05RequestDataBase>
    {
        public byte DeviceAddress { get; set; }               //设备地址
        public FunctionNumType FunctionNum { get; set; }      //功能码
        public ushort CoilAddress { get; set; }               //将要设置值的寄存器地址

        /// <summary>
        /// 强制单个线圈的值
        /// 有效值：
        ///     True表示ON，
        ///     否则表示OFF
        /// </summary>
        public bool ON { get; set; }

        public List<FunNum05RequestDataBase> CovertToFunNumRequestDataBases()
        {
            List<FunNum05RequestDataBase> funNum05RequestDataBases = new List<FunNum05RequestDataBase>();
            FunNum05RequestDataBase funNum05RequestDataBase = new FunNum05RequestDataBase();

            funNum05RequestDataBase.DeviceAddress = this.DeviceAddress;
            funNum05RequestDataBase.FunctionNum = Convert.ToByte(Convert.ToInt32(this.FunctionNum));
            funNum05RequestDataBase.CoilAddressHigh = (byte)(this.CoilAddress / 256);
            funNum05RequestDataBase.CoilAddressLow = (byte)(this.CoilAddress % 256);

            if (this.ON)
            {
                //ON
                funNum05RequestDataBase.ForceDataHigh = 0xFF;
                funNum05RequestDataBase.ForceDataLow = 0x00;
            }
            else
            {
                //OFF
                funNum05RequestDataBase.ForceDataHigh = 0x00;
                funNum05RequestDataBase.ForceDataLow = 0x00;
            }

            funNum05RequestDataBases.Add(funNum05RequestDataBase);

            return funNum05RequestDataBases;
        }
    }
}
