using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modbus.Contract.RequestDataBase;

namespace Modbus.RTU.RTURequestData
{
    /// <summary>
    /// 功能码06（06 H）的RTU请求帧
    /// </summary>
    public class FunNum06RequestDataRTU : IFunNumRequestDataRTU
    {
        private FunNum06RequestDataBase funNum06RequestDataBase;
        public FunNum06RequestDataBase FunNum06RequestDataBase
        {
            get { return funNum06RequestDataBase; }
            set
            {
                funNum06RequestDataBase = value;
                CRCCheck = new CRCCheck(funNum06RequestDataBase.ToByteList().ToArray());
            }
        }

        CRCCheck CRCCheck { get; set; }

        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();

            byteList.AddRange(funNum06RequestDataBase.ToByteList());
            byteList.AddRange(CRCCheck.ToByteList());

            return byteList;
        }
    }
}
