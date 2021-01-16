using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modbus.Contract.RequestDataBase;

namespace Modbus.RTU.RTURequestData
{
    /// <summary>
    /// 功能码03（03 H）的RTU请求帧
    /// </summary>
    public class FunNum03RequestDataRTU : IFunNumRequestDataRTU
    {
        private FunNum03RequestDataBase funNum03RequestDataBase;
        public FunNum03RequestDataBase FunNum03RequestDataBase
        {
            get { return funNum03RequestDataBase; }
            set 
            { 
                funNum03RequestDataBase = value;
                CRCCheck = new CRCCheck(funNum03RequestDataBase.ToByteList().ToArray());
            }
        }

        CRCCheck CRCCheck { get; set; }



        #region IFunNumRequestDataRTU 成员

        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();

            byteList.AddRange(funNum03RequestDataBase.ToByteList());
            byteList.AddRange(CRCCheck.ToByteList());

            return byteList;
        }

        #endregion
    }
}
