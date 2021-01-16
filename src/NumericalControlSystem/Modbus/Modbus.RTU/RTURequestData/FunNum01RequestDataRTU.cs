using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modbus.Contract.RequestDataBase;

namespace Modbus.RTU.RTURequestData
{
    /// <summary>
    /// 功能码01（01 H）的RTU请求帧
    /// </summary>
    public class FunNum01RequestDataRTU : IFunNumRequestDataRTU
    {
        private FunNum01RequestDataBase funNum01RequestDataBase;
        public FunNum01RequestDataBase FunNum01RequestDataBase
        {
            get { return funNum01RequestDataBase; }
            set
            {
                funNum01RequestDataBase = value;
                CRCCheck = new CRCCheck(funNum01RequestDataBase.ToByteList().ToArray());
            }
        }

        CRCCheck CRCCheck { get; set; }

        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();  

            byteList.AddRange(funNum01RequestDataBase.ToByteList());
            byteList.AddRange(CRCCheck.ToByteList());

            return byteList;
        }
    }
}
