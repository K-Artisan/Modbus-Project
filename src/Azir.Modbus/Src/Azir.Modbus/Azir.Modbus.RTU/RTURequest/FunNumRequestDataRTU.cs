using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.FuncitonNum.ModbusRequest;

namespace Azir.Modbus.RTU.RTURequest
{
    public class FunNumRequestDataRTU
    {
        private IFunNumRequestDataBase funNumRequestDataBase;
        public IFunNumRequestDataBase FunNumRequestDataBase
        {
            get { return funNumRequestDataBase; }
            set
            {
                funNumRequestDataBase = value;
                CRCCheck = new CRCCheck(funNumRequestDataBase.ToByteList().ToArray());
            }
        }

        CRCCheck CRCCheck { get; set; }

        #region Ctor

        public FunNumRequestDataRTU(IFunNumRequestDataBase funNumRequestDataBase)
        {
            this.funNumRequestDataBase = funNumRequestDataBase;
            CRCCheck = new CRCCheck(funNumRequestDataBase.ToByteList().ToArray());
        }

        #endregion


        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();
            CRCCheck = new CRCCheck(funNumRequestDataBase.ToByteList().ToArray());

            byteList.AddRange(funNumRequestDataBase.ToByteList());
            byteList.AddRange(CRCCheck.ToByteList());

            return byteList;
        }
    }
}
