using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modbus.Contract.RequestDataBase;

namespace Modbus.RTU.RTURequestData
{
    /// <summary>
    /// 功能码15（0F H）的RTU请求帧
    /// </summary>
    public class FunNum15RequestDataRTU : IFunNumRequestDataRTU
    {
        private FunNum15RequestDataBase funNum15RequestDataBase;
        public FunNum15RequestDataBase FunNum15RequestDataBase
        {
            get { return funNum15RequestDataBase; }
            set
            {
                funNum15RequestDataBase = value;
                CRCCheck = new CRCCheck(funNum15RequestDataBase.ToByteList().ToArray());
            }
        }

        CRCCheck CRCCheck { get; set; }

        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();

            byteList.AddRange(funNum15RequestDataBase.ToByteList());
            byteList.AddRange(CRCCheck.ToByteList());

            return byteList;
        }
    }
}
