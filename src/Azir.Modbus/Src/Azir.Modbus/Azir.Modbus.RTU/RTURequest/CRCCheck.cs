using System.Collections.Generic;

namespace Azir.Modbus.RTU.RTURequest
{
    /// <summary>
    /// CRC校验：
    ///    
    /// </summary>
    public class CRCCheck
    {
        public byte CRCLow { get; set; }
        public byte CRCHigh { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataToCheck">需要校验的数据</param>
        public CRCCheck(byte[] dataToCheck)
        {
            uint crc16 = CalculateCRC16(dataToCheck, (uint)dataToCheck.Length);
            this.CRCLow = (byte)(crc16 & 0xFF);
            this.CRCHigh = (byte)((crc16 & 0xFF00) >> 8);
        }

        public List<byte> ToByteList()
        {
            List<byte> byteList = new List<byte>();


            //注意：先低位，后高位
            byteList.Add(CRCLow);  
            byteList.Add(CRCHigh);

            return byteList;
        }

        /// <summary>
        /// 计算CRC16校验码(工业中常用16位的CRC,以太网用32位crc)
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="Length">数据的长度</param>
        /// <returns></returns>
        public uint CalculateCRC16(byte[] buff, uint length)
        {
            uint crc16 = 0xFFFF;

            for (uint i = 0; i < length; i++)
            {
                crc16 ^= buff[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc16 & 0x0001) == 1)
                    {
                        crc16 = (crc16 >> 1) ^ 0xA001;
                    }
                    else
                    {
                        crc16 = crc16 >> 1;
                    }
                }
            }

            return crc16;
        }
    }
}
