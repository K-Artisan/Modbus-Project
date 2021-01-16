using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azir.Modbus.TCP.TCPRequest
{
    public class TCPHeader : ITCPHeader
    {
        /// <summary>
        /// 事务处理标识高位
        /// </summary>
        public byte TransactionIdHigh { get; set; }
        /// <summary>
        /// 事务处理标识低位
        /// </summary>
        public byte TransactionIdLow { get; set; }
        /// <summary>
        /// 协议标识符高位
        /// </summary>
        public byte ProtocolIdHigh { get; set; }   
        /// <summary>
        /// 协议标识符低位
        /// </summary>
        public byte ProtocolIdLow { get; set; }
        /// <summary>
        /// 功能码请求长度高位
        /// </summary>
        public byte FunNumRequstDataLenthHigh { get; set; }
        /// <summary>
        /// 功能码请求长度低位
        /// </summary>
        public byte FunNumRequstDataLenthLow { get; set; }

        /// <summary>
        /// TCP报头总长度
        /// </summary>
        public  static int TCPDataByteLenth = 6;

        #region Ctor

        /// <summary>
        /// TCP报头
        /// </summary>
        /// <param name="funNumRequstDataLenth">
        /// 请求数据区（即：TCP报头之后的请求帧部分）的长度
        /// </param>
        public TCPHeader(int funNumRequstDataLenth)
        {
            TransactionIdHigh = 0;
            TransactionIdLow = 0;
            ProtocolIdHigh = 0;
            ProtocolIdLow = 0;

            FunNumRequstDataLenthHigh = (byte)(funNumRequstDataLenth / 256);
            FunNumRequstDataLenthLow = (byte)(funNumRequstDataLenth % 256);
        }

        #endregion

        #region Member Of ITCPHeader

        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();

            byteList.Add(TransactionIdHigh);
            byteList.Add(TransactionIdLow);
            byteList.Add(ProtocolIdHigh);
            byteList.Add(ProtocolIdLow);
            byteList.Add(FunNumRequstDataLenthHigh);
            byteList.Add(FunNumRequstDataLenthLow);

            return byteList;
        } 

        #endregion
    }
}
