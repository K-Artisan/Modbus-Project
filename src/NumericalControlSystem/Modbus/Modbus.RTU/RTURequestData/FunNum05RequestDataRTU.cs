using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modbus.Contract.RequestDataBase;

namespace Modbus.RTU.RTURequestData
{
    /// <summary>
    /// 功能码05（05 H）的RTU请求帧
    /// </summary>
    public class FunNum05RequestDataRTU : IFunNumRequestDataRTU
    {
        private FunNum05RequestDataBase funNum05RequestDataBase;
        public FunNum05RequestDataBase FunNum05RequestDataBase
        {
            get { return funNum05RequestDataBase; }
            set
            {
                funNum05RequestDataBase = value;
                CRCCheck = new CRCCheck(funNum05RequestDataBase.ToByteList().ToArray());
            }
        }

        CRCCheck CRCCheck { get; set; }

        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();

            byteList.AddRange(funNum05RequestDataBase.ToByteList());
            byteList.AddRange(CRCCheck.ToByteList());

            return byteList;
        }
    }
}
