using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusServer
{
    /*---------------------------------------------------------------------------------------
     * TcpDataPacket
     *--------------------------------------------------------------------------------------*/
    /// <summary>
    /// Tcp包头数据
    /// </summary>
    class TcpDataPacket
    {
        protected byte tcpFirst;        //TCP包头第一字节
        public byte TcpFirst
        {
            get { return tcpFirst; }
            set { tcpFirst = value; }
        }

        protected byte tcpSecond;       //TCP包头第二字节
        public byte TcpSecond
        {
            get { return tcpSecond; }
            set { tcpSecond = value; }
        }

        protected byte tcpThird;        //TCP包头第三字节
        public byte TcpThird
        {
            get { return tcpThird; }
            set { tcpThird = value; }
        }

        protected byte tcpFourth;       //TCP包头第四字节
        public byte TcpFourth
        {
            get { return tcpFourth; }
            set { tcpFourth = value; }
        }

        protected byte tcpFifth;        //TCP包头第五字节
        public byte TcpFifth
        {
            get { return tcpFifth; }
            set { tcpFifth = value; }
        }

        protected byte tcpSixth;        //TCP包头第六字节，表示除TCP包头外的内容长度（单位：字节）
        public byte TcpSixth
        {
            get { return tcpSixth; }
            set { tcpSixth = value; }
        }

        public TcpDataPacket()
        {
            tcpFirst = 0;
            tcpSecond = 0;
            tcpThird = 0;
            tcpFourth = 0;
            tcpFifth = 0;
            tcpSixth = 6;
        }
    }


    /*---------------------------------------------------------------------------------------
     * ModbusSendDataStruct
     *--------------------------------------------------------------------------------------*/
    /// <summary>
    /// ModBus请求包
    /// </summary>
    class ModbusSendDataStruct : TcpDataPacket
    {
        byte addr;               //地址域
        public byte Addr
        {
            get { return addr; }
            set { addr = value; }
        }

        byte funNum;             //功能码
        public byte FunNum
        {
            get { return funNum; }
            set { funNum = value; }
        }

        ushort startReg;         //开始地址（起始寄存器的Id）
        public ushort StartReg
        {
            get { return startReg; }
            set { startReg = value; }
        }


        //由于该ModbusRecvDataStruct.regDataLen是byte类型，表示的值范围是0-255；
        //所以理论上要求：
        //03功能码：请求包中regCount（寄存器个数）<=127; 现实建议是1-120
        //01功能码：请求包中regCount（寄存器个数）<=255*8 = 2040; 现实建议是1-1000
        //。。。。
        //如果大于各自的范围，分批发送吧。
        ushort regCount;        //01和03功能码时候这表示寄存器个数   
                                //05功能码时候这表示AO的开（0xFF00）、关（0x0000） 
                                //06功能码时候这表示要写入的寄存器值
        public ushort RegCount
        {
            get { return regCount; }
            set { regCount = value; }
        }

        public ModbusSendDataStruct()
            : base()
        {
            addr = 1;
            funNum = 3;
            startReg = 0;
            regCount = 0;
        }

        /// <summary>
        /// 将请求包数据封装成字节数组
        /// </summary>
        /// <returns>封装好的请求包</returns>
        public byte[] ToArray()
        {
            byte[] res = new byte[12];

            res[0] = tcpFirst;
            res[1] = tcpSecond;
            res[2] = tcpThird;
            res[3] = tcpFourth;
            res[4] = tcpFifth;
            res[5] = tcpSixth;
            res[6] = addr;
            res[7] = funNum;
            res[8] = (byte)(startReg / 256);
            res[9] = (byte)(startReg % 256);
            res[10] = (byte)(regCount / 256);
            res[11] = (byte)(regCount % 256);

            if (5 == res[7])    //05功能码（DO）发送到数据不一样哦！
            {
                switch (regCount)
                {
                    case 0:   //DI 关
                        res[10] = 0x00;
                        res[11] = 0x00;
                        break;
                    case 1:   //DI 开
                        res[10] = 0xFF;
                        res[11] = 0x00;
                        break;
                    default: //DI 开(非零值也表示开)
                        res[10] = 0xFF;
                        res[11] = 0x00;
                        break;
                }
            }

            return res;
        }
    }


    /*---------------------------------------------------------------------------------------
     * ModbusRecvDataStruct
     *--------------------------------------------------------------------------------------*/
    /// <summary>
    /// ModBus接收数据包
    /// </summary>
    class ModbusRecvDataStruct : TcpDataPacket
    {
        byte addr;               //地址域
        public byte Addr
        {
            get { return addr; }
            set { addr = value; }
        }

        byte funNum;             //功能码
        public byte FunNum
        {
            get { return funNum; }
            set { funNum = value; }
        }


        //由于该ModbusRecvDataStruct.regDataLen是byte类型，表示的值范围是0-255；
        //所以理论上要求：
        //03功能码：请求包中regCount（寄存器个数）<=127; 现实建议是1-120
        //01功能码：请求包中regCount（寄存器个数）<=255*8 = 2040; 现实建议是1-1000
        //。。。。
        //如果大于各自的范围，分批发送吧。
        byte regDataLen;      //响应数据（数据域，即表示寄存器值的数据）的长度（单位：字节）！！！                         
        public byte RegDataLen
        {
            get { return regDataLen; }
            set { regDataLen = value; }
        }

        IList<double> recvData = new List<double>(); //存储ModBus接收到的数据包(解析后的)
        public IList<double> RecvData
        {
            get { return recvData; }
            set { recvData = value; }
        }

        IList<double> recvResData = new List<double>(); //存储ModBus接收数据包中寄存器的数据(解析后的)
        public IList<double> RecvResData
        {
            get { return recvResData; }
            set { recvResData = value; }
        }

        public ModbusRecvDataStruct()
            : base()
        {
            addr = 1;
            funNum = 3;
            regDataLen = 0;
        }

        /// <summary>
        /// 解析服务器响应数据，并且保存到字段recvData中
        /// </summary>
        /// <param name="data">服务器响应数据</param>
        /// <returns>解析后的接收数据包</returns>
        //public IList<double> ParseRecvData(byte[] data)
        //{
        //    for (int i = 0; i < 9; ++i) //byte[]中0-8是非寄存器值
        //   {
        //       recvData.Add(data[i]);
        //       //Debug.WriteLine("ParseRecvData[{0}]: {1}", i, recvData[i]);
        //   }

        //   int registerNum = 0; //寄存器的个数
        //   registerNum = data[8] / 2;
        //   for (int i = 0; i < registerNum; ++i)
        //   {
        //       ushort temp = 0;
        //       //byte[]中9以后是2个字节表示一个寄存器值
        //       temp = (ushort)(data[9 + 2 * i] * 256 + data[10 + 2 * i]);
        //       recvData.Add(temp);
        //       //Debug.WriteLine("ParseRecvData[{0}]: {1}", i + 9, recvData[i + 9]);
        //   }

        //   return recvData;
        //} //end of ParseRecvData

        /// <summary>
        /// 解析服务器响应数据中的寄存器数据,并且保存到字段recvResData中
        /// </summary>
        /// <param name="data">服务器响应数据</param>
        /// <returns>解析后的寄存器数据</returns>
        //public IList<double> ParseRecvResData(byte[] data)
        //{
        //    int registerNum = 0; //寄存器的个数
        //    registerNum = data[8] / 2;
        //    for (int i = 0; i < registerNum; ++i)
        //    {
        //        ushort temp = 0;
        //        temp = (ushort)(data[9 + 2 * i] * 256 + data[10 + 2 * i]);
        //        recvResData.Add(temp);
        //        //Debug.WriteLine("ParseRecvResData[{0}]: {1}", i, recvData[i]);
        //    }
        //    return recvResData;
        //} //end of ParseRecvResData


        ///// <summary>
        /////  将收到的数据根据功能码解析成UInt16数据
        /////  功能码决定了从字节流中提取和组装数据的方式
        ///// </summary>
        ///// <param name="recData">收到的数据(字节流)</param>
        ///// <returns>解析后UInt16数据</returns>
        //public List<UInt16> RecvDataToUInt16WithFunNum(byte[] recdata)
        //{
        //    List<UInt16> data = new List<UInt16>(); //存储解析后UInt16数据

        //    funNum = recdata[7];      //提取功能码
        //    regDataLen = recdata[8];  //错：寄存器的个数 ////对：提取收到数据的长度（单位：字节）,注意；功能码不同，返回的数据长度不同


        //    //功能码不同返回的数据也不同，例如：
        //    //03功能码请求10个寄存器值,通讯管理机返回数据是20个byte
        //    //01功能码请求10个寄存器值,通讯管理机返回数据是2个byte（因为01功能码是1位表示1个寄存器值，
        //    //以高位开始依次表示第0个寄存器值，依次类推，剩余的6位无用）

        //    switch (funNum) //功能码
        //    {
        //        case 1:   //1位转化成一个UInt16数据用来表示一个寄存器的值
        //            int recdataLen = (int)Math.Ceiling(((double)regDataLen) / 8); //返回的表示寄存器数据的字节数=(int)Math.Ceiling(寄存器个数/ 8)
        //            for (int i = 0; i < recdataLen; ++i)
        //            {
        //                byte temp = recdata[9 + i];

        //                data.Add((UInt16)((temp & 0x80) >> 7)); //一个字节中的最高位,先加入高位data
        //                data.Add((UInt16)((temp & 0x40) >> 6));
        //                data.Add((UInt16)((temp & 0x20) >> 5));
        //                data.Add((UInt16)((temp & 0x10) >> 4));
        //                data.Add((UInt16)((temp & 0x08) >> 3));
        //                data.Add((UInt16)((temp & 0x04) >> 2));
        //                data.Add((UInt16)((temp & 0x02) >> 1));
        //                data.Add((UInt16)(temp & 0x01));        //一个字节中的最低位
        //            }


        //            break;

        //        case 3:   //2个字节转化成一个UInt16数据用来表示一个寄存器的值
        //            //countRes = regDataLen / 2;

        //            int countRes = regDataLen / 2;  //寄存器的个数（每个寄存器占两个字节）
        //            for (int i = 0; i < countRes; ++i)
        //            {
        //                UInt16 temp = (UInt16)((UInt16)recdata[2 * i + 9] * 256 + (UInt16)recdata[2 * i + 10]);
        //                data.Add(temp);
        //            }

        //            break;

        //        default:
        //            break;
        //    }

        //    return data;
        //}


        /// <summary>
        ///  将收到的数据根据功能码解析成UInt16数据
        ///  功能码决定了从字节流中提取和组装数据的方式
        /// </summary>
        /// <param name="recdata">收到的数据(字节流)</param>
        /// <param name="startResId">起始寄存器id</param>
        /// <param name="needReadRes">实际需要读取的寄存器个数</param>
        /// <returns>解析后UInt16数据，字典《寄存器id，寄存器值》</returns>
        public Dictionary<int, UInt16> RecvDataToUInt16WithFunNum(byte[] recdata, int startResId, int countReadRes)
        {
            /*----------------------------------------------------------------------------------
             * 本方法要保证返回值都是有效的，体现在：
             * 01功能码返回的数据recdata中可能有一部分是无效（并不表示寄存器的值），需要剔除
             ----------------------------------------------------------------------------------*/

            Dictionary<int, UInt16> dicResIdToData = new Dictionary<int, UInt16>();

            funNum = recdata[7];      //提取功能码
            regDataLen = recdata[8];  //提取收到表示寄存器值的数据的长度（单位：字节）,注意；功能码不同，请求相同的寄存器个数返回的数据长度不同
            //int countRes = 0;         //寄存器个数

            //功能码不同返回的数据也不同，例如：
            //03功能码请求10个寄存器值,通讯管理机返回数据是20个byte
            //01功能码请求10个寄存器值,通讯管理机返回数据是2个byte（因为01功能码是1位表示1个寄存器值，
            //以高位开始依次表示第0个寄存器值，依次类推，剩余的6位无用）

            switch (funNum) //功能码
            {
                case 1:  //0功能码(DI)
                    {
                        //1位转化成一个UInt16数据用来表示一个寄存器的值
                        byte temp = 0;
                        List<UInt16> totalRecvVal = new List<UInt16>();
                        for (int k = 0; k < regDataLen; ++k)
                        {
                            temp = recdata[9 + k];

                            //解析01功能码的顺序是从每个返回字节的低位到高位
                            totalRecvVal.Add((UInt16)(temp & 0x01));         //一个字节中的最低位
                            totalRecvVal.Add((UInt16)((temp & 0x02) >> 1));
                            totalRecvVal.Add((UInt16)((temp & 0x04) >> 2));
                            totalRecvVal.Add((UInt16)((temp & 0x08) >> 3));
                            totalRecvVal.Add((UInt16)((temp & 0x10) >> 4));
                            totalRecvVal.Add((UInt16)((temp & 0x20) >> 5));
                            totalRecvVal.Add((UInt16)((temp & 0x40) >> 6));
                            totalRecvVal.Add((UInt16)((temp & 0x80) >> 7));  //一个字节中的最高位,先加入高位data
                        }

                        //条件：i < countReadRes 是要求实际要读多少个寄存器的值，就存储多少个值
                        //totalRecvVal中可能存在不表示寄存器值的多余数据，通过条件：i < countReadRes，提取有效数据，剔除无效数据
                        for (int i = 0; i < totalRecvVal.Count && i < countReadRes; ++i)
                        {
                            //这样赋值的前提是：返回的值是对应的寄存器是连续的
                            dicResIdToData.Add(startResId++, totalRecvVal[i]);
                        }

                        break;
                    }
                case 3:   //03功能码(AI\ACC)：
                    {
                        //2个字节转化成一个UInt16数据用来表示一个寄存器的值
                        //03功能码(AI\ACC)：寄存器的个数（每个寄存器占两个字节）= 响应字节数 / 2
                        int countRes = regDataLen / 2;  //等价为（int countRes = countReadRes;）
                        UInt16 temp = 0;
                        for (int i = 0; i < countRes; ++i)
                        {
                            temp = (UInt16)((UInt16)recdata[2 * i + 9] * 256 + (UInt16)recdata[2 * i + 10]);
                            dicResIdToData.Add(startResId, temp);
                            ++startResId;
                        }

                        break;
                    }
                case 5:   //05功能码(DO):  recdata[8]和recdata[9]表示起始寄存器id，recdata[10]及其以后表示寄存器值
                    {
                        int countRes = countReadRes; //寄存器的个数
                        UInt16 temp = 0;

                        for (int i = 0; i < countRes; ++i)
                        {
                            
                            if (0xFF == recdata[2 * i + 10] && 0x00 == recdata[2 * i + 11])
                            {
                                temp = 1; //DO 开
                            }
                            else if (0x00 == recdata[2 * i + 10] && 0x00 == recdata[2 * i + 11])
                            {
                                temp = 0;  //DO 关
                            }

                            dicResIdToData.Add(startResId, temp);
                            ++startResId;
                        }

                        break;
                    }

                case 6:   //06功能码(AO):  recdata[8]和recdata[9]表示起始寄存器id，recdata[10]及其以后表示寄存器值
                    {
                        int countRes = countReadRes; //寄存器的个数
                        UInt16 temp = 0;
                        for (int i = 0; i < countRes; ++i)
                        {
                            temp = (UInt16)((UInt16)recdata[2 * i + 10] * 256 + (UInt16)recdata[2 * i + 11]);
                            dicResIdToData.Add(startResId, temp);
                            ++startResId;
                        }

                        break;
                    }

                default:
                    {
                        int countRes2 = regDataLen / 2;  //等价为（int countRes = countReadRes;）
                        UInt16 temp = 0;
                        for (int i = 0; i < countRes2; ++i)
                        {
                            temp = (UInt16)((UInt16)recdata[2 * i + 9] * 256 + (UInt16)recdata[2 * i + 10]);
                            dicResIdToData.Add(startResId, temp);
                            ++startResId;
                        }
                        break;
                    }
            }

            return dicResIdToData;
        }

    } //end of ModbusRecvDataStruct




} //end of nama space ModbusServer
