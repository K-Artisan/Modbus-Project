using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.FuncitonNum.ModbusRequest;

namespace Azir.Modbus.TCP.TCPRequest
{
    public class FunNumRequestDataTCP
    {
        public TCPHeader TCPHeader { get; set; }      

        private IFunNumRequestDataBase funNumRequestDataBase;
        public IFunNumRequestDataBase FunNumRequestDataBase
        {
            get { return funNumRequestDataBase; }
            set
            {
                funNumRequestDataBase = value;
            }
        }
      
        #region Ctor

        public FunNumRequestDataTCP(IFunNumRequestDataBase funNumRequestDataBase)
        {
            this.funNumRequestDataBase = funNumRequestDataBase;
            TCPHeader = new TCPHeader(funNumRequestDataBase.ToByteList().Count());
        }

        #endregion


        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();

            byteList.AddRange(TCPHeader.ToByteList());
            byteList.AddRange(funNumRequestDataBase.ToByteList());

            return byteList;
        }
    }
}
