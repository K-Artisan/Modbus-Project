using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Service.SeviceImplementation.ModbusService
{
    /// <summary>
    /// 寄存器
    /// </summary>
    public class Register
    {
        /// <summary>
        /// 设备（主机）地址
        /// 取值范围是一个字节：0-127
        /// </summary>
        public int DeviceAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 寄存器的地址
        /// </summary>
        public int RegisterAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 寄存器的值
        /// </summary>
        public UInt16 RegisterValue
        {
            get;
            set;
        }

        /// <summary>
        /// 按数据的低到高顺序加入
        /// </summary>
        public List<byte> LowToHighDataBytes
        {
            get { return lowToHighDataBytes; }
            set { lowToHighDataBytes = value; }
        }

        private List<byte> lowToHighDataBytes = new List<byte>();
    }
}
