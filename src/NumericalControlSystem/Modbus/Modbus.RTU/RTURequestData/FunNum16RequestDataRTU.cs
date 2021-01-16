using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modbus.Contract.RequestDataBase;

namespace Modbus.RTU.RTURequestData
{
    /// <summary>
    /// 功能码16（16 H）的RTU请求帧
    /// </summary>
    public class FunNum16RequestDataRTU : IFunNumRequestDataRTU
    {
        private FunNum16RequestDataBase funNum16RequestDataBase;
        public FunNum16RequestDataBase FunNum16RequestDataBase
        {
            get { return funNum16RequestDataBase; }
            set
            {
                funNum16RequestDataBase = value;
                CRCCheck = new CRCCheck(funNum16RequestDataBase.ToByteList().ToArray());
            }
        }

        CRCCheck CRCCheck { get; set; }

        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();

            byteList.AddRange(funNum16RequestDataBase.ToByteList());
            byteList.AddRange(CRCCheck.ToByteList());

            return byteList;
        }
    }
}
