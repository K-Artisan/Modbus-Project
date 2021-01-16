using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.FuncitonNum;
using Azir.Modbus.Protocol.FuncitonNum.ModbusRequest;

namespace Azir.Modbus.Protocol.DataReponse
{
    /// <summary>
    /// Modbus响应数据解析器
    /// </summary>
    public class ModbusRecivedDataAnalyzer
    {
        #region 根据功能码将接受帧解析成一些列寄存器值

        /// <summary>
        /// 解析返回的数据
        /// </summary>
        /// <param name="dataAnalyzeMode"> 解析数据方式 </param>
        /// <param name="requestByteData">
        /// 请求数据字节流,
        /// 注意：要求第一个字节位是:单元标识符（从机地址位），
        /// 即：如果是TCP请求帧，要求去掉TCP报头所剩下的字节。
        /// 例如03功能码，要求请求数据字节流第一个字节位<see cref="FunNum03RequestDataBase.DeviceAddress"/>

        /// </param>
        /// <param name="receviceByteData">
        /// 响应数据字节流
        /// 注意：要求第一个字节位是:单元标识符（从机地址位），
        /// 即：如果是TCP响应帧，要求去掉TCP报头所剩下的字节。
        /// 例如03功能码，要求响应数据字节流第一个字节位<see cref="FunNum03RequestDataBase.DeviceAddress"/>
        /// </param>
        /// <returns></returns>
        public static AnalyzeRecivedDataReponse AnalyzeRecivedData(DataAnalyzeMode dataAnalyzeMode, List<byte> requestByteData, List<byte> receviceByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();
            reponse = RecivedDataCorrespondToRequesData(requestByteData, receviceByteData);

            if (!(reponse.ModbusReponseSuccess && reponse.AnalyzeRecivedDataSuccess))
            {
                reponse.ModbusReponseSuccess = false;
                return reponse;
            }

            byte funNumHex = receviceByteData[1]; //功能码的16进制值
            string funNmDecimal = Convert.ToString(funNumHex, 10); //功能码的10进制值字符串

            FunctionNumType functionNumType = 0;
            Enum.TryParse(funNmDecimal, true, out functionNumType);

            switch (functionNumType)
            {
                case FunctionNumType.FunctionNum01:
                    reponse = AnalyzeRecivedDataByFunNum01(dataAnalyzeMode, requestByteData, receviceByteData);
                    break;
                case FunctionNumType.FunctionNum02:
                    break;
                case FunctionNumType.FunctionNum03:
                    reponse = AnalyzeRecivedDataByFunNum03(dataAnalyzeMode, requestByteData, receviceByteData);
                    break;
                case FunctionNumType.FunctionNum04:
                    break;
                case FunctionNumType.FunctionNum05:
                    reponse = AnalyzeRecivedDataByFunNum05(dataAnalyzeMode, requestByteData, receviceByteData);
                    break;
                case FunctionNumType.FunctionNum06:
                    reponse = AnalyzeRecivedDataByFunNum06(dataAnalyzeMode, requestByteData, receviceByteData);
                    break;
                case FunctionNumType.FunctionNum07:
                    break;
                case FunctionNumType.FunctionNum08:
                    break;
                case FunctionNumType.FunctionNum09:
                    break;
                case FunctionNumType.FunctionNum10:
                    break;
                case FunctionNumType.FunctionNum11:
                    break;
                case FunctionNumType.FunctionNum12:
                    break;
                case FunctionNumType.FunctionNum13:
                    break;
                case FunctionNumType.FunctionNum14:
                    break;
                case FunctionNumType.FunctionNum15:
                    reponse = AnalyzeRecivedDataByFunNum015(dataAnalyzeMode, requestByteData, receviceByteData);
                    break;
                case FunctionNumType.FunctionNum16:
                    reponse = AnalyzeRecivedDataByFunNum16(dataAnalyzeMode, requestByteData, receviceByteData);
                    break;
                case FunctionNumType.FunctionNum17:
                    break;
                case FunctionNumType.FunctionNum18:
                    break;
                case FunctionNumType.FunctionNum19:
                    break;
                case FunctionNumType.FunctionNum20:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return reponse;
        }

        /// <summary>
        /// 判读接受的数据是否对应请求
        /// </summary>
        /// <returns></returns>
        private static AnalyzeRecivedDataReponse RecivedDataCorrespondToRequesData(List<byte> requestByteData, List<byte> receviceByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            #region 基本的规则校验

            if (receviceByteData.Count < 2 || requestByteData.Count < 2)
            {
                //无法获取设备地址、功能码
                string message = "请求帧或接受帧中不存在设备地址或功能码。";
                //LoggingFactory.GetLogger().WriteDebugLogger(message);
                reponse.ErrorMsg = message;
                reponse.ModbusReponseSuccess = false;
                return reponse;
            }

            if (requestByteData[0] != receviceByteData[0])
            {
                string message = "请求帧与接受帧的设备地址不对应，响应数据有误。";
                //LoggingFactory.GetLogger().WriteDebugLogger(message);
                reponse.ErrorMsg = message;
                reponse.AnalyzeRecivedDataSuccess = false;
                return reponse;
            }


            if (requestByteData[1] != receviceByteData[1])
            {
                string message = "请求帧与接受帧的功能码不对应，响应数据有误。";
                //LoggingFactory.GetLogger().WriteDebugLogger(message);
                reponse.ErrorMsg = message;
                reponse.AnalyzeRecivedDataSuccess = false;
                return reponse;
            }

            #endregion


            #region TODO：数据校验

            #endregion

            #region  返回的数据根据是否带GUID判断是否是对应的数据
            //try
            //{
            //    //请求或返回起码长度为4：1个字节设备地址、1个字节功能码、2个字节CRC校验位
            //    //16是16个字节的GUID。
            //    int byteDataMinLenth = 4 + 16;
            //    if (requestByteData.Count < byteDataMinLenth)
            //    {
            //        correspond = false;
            //    }

            //    //数据溢出也表明响应的数据不对。
            //    List<byte> requestGuid = GetGuidBytesForm(requestByteData);
            //    List<byte> recieveGuid = GetGuidBytesForm(receviceByteData);

            //    for (int i = 0; i < 16; i++)
            //    {
            //        if (requestGuid[i] != recieveGuid[i])
            //        {
            //            correspond = false;
            //            break;
            //        }
            //    }

            //}
            //catch (Exception)
            //{
            //    correspond = false;
            //} 
            #endregion

            reponse.ModbusReponseSuccess = true;
            return reponse;
        }

        private static List<byte> GetGuidBytesForm(List<byte> dataBytes)
        {
            List<byte> guidBytes = new List<byte>();

            for (int i = dataBytes.Count - 16; i < dataBytes.Count; i++)
            {
                guidBytes.Add(dataBytes[i]);
            }

            return guidBytes;
        }

        private static AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum01(
            DataAnalyzeMode dataAnalyzeMode,
            List<byte> requestByteData,
            List<byte> receviceByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            try
            {
                int deviceAddress = requestByteData[0];

                int startRegisterAddress = requestByteData[2] * 256 + requestByteData[3];
                int countOfRegisterToRead = requestByteData[4] * 256 + requestByteData[5];


                //接受侦receviceByteData中寄存器值的数据之前的字节数：
                //deviceAddress、funNum、receviceByteData[2]个占1个字节
                int byteCountBeforeDataRegion = 3;
                //数据区表示寄存器值的字节数
                int registerValueByteCount = receviceByteData[2];

                //避免数据返回不完整引发下标溢出
                if (receviceByteData.Count > (byteCountBeforeDataRegion + receviceByteData[2]))
                {
                    byte byteData = 0;
                    List<UInt16> totalRecvVal = new List<ushort>();
                    for (int k = 0; k < registerValueByteCount; k++)
                    {
                        byteData = receviceByteData[byteCountBeforeDataRegion + k];

                        //解析01功能码的顺序是从每个返回字节的低位到高位
                        totalRecvVal.Add((UInt16)(byteData & 0x01));         //一个字节中的最低位
                        totalRecvVal.Add((UInt16)((byteData & 0x02) >> 1));
                        totalRecvVal.Add((UInt16)((byteData & 0x04) >> 2));
                        totalRecvVal.Add((UInt16)((byteData & 0x08) >> 3));
                        totalRecvVal.Add((UInt16)((byteData & 0x10) >> 4));
                        totalRecvVal.Add((UInt16)((byteData & 0x20) >> 5));
                        totalRecvVal.Add((UInt16)((byteData & 0x40) >> 6));
                        totalRecvVal.Add((UInt16)((byteData & 0x80) >> 7));  //一个字节中的最高位,先加入高位data
                    }

                    //条件：i < countOfRegisterToRead 是要求实际要读多少个寄存器的值，就存储多少个值
                    //totalRecvVal中可能存在不表示寄存器值的多余数据，
                    //通过条件：i < countOfRegisterToRead，提取有效数据，剔除无效数据
                    for (int i = 0; i < totalRecvVal.Count && i < countOfRegisterToRead; i++)
                    {
                        Register register = new Register();
                        register.DeviceAddress = deviceAddress;
                        register.RegisterAddress = startRegisterAddress;
                        register.RegisterValue = totalRecvVal[i];

                        reponse.Registers.Add(register);
                        ++startRegisterAddress;
                    }
                }
            }
            catch (Exception ex)
            {
                reponse.AnalyzeRecivedDataSuccess = false;
                //LoggingFactory.GetLogger().WriteDebugLogger("解析Modbus接收帧时发生异常！");
                reponse.ErrorMsg = "AnalyzeRecivedDataByFunNum01->解析Modbus接收帧时发生异常！" + ex.Message;
                return reponse;
            }

            return reponse;
        }

        private static AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum03(
            DataAnalyzeMode dataAnalyzeMode,
            List<byte> requestByteData,
            List<byte> receviceByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            try
            {
                //接受帧receviceByteData中寄存器值的数据之前的字节数：
                //deviceAddress、funNum、receviceByteData[2]各占1个字节
                int byteCountBeforeDataRegion = 3;

                //避免数据返回不完整引发下标溢出
                if (receviceByteData.Count > (byteCountBeforeDataRegion + receviceByteData[2]))
                {
                    int deviceAddress = requestByteData[0];
                    int startRegisterAddress = requestByteData[2] * 256 + requestByteData[3];

                    //寄存器的个数
                    int countOfRegisterHadRead = receviceByteData[2] / 2;

                    for (int i = 0; i < countOfRegisterHadRead; i++)
                    {
                        Register register = new Register();
                        register.DeviceAddress = deviceAddress;
                        register.RegisterAddress = startRegisterAddress;

                        if (dataAnalyzeMode == DataAnalyzeMode.DataHighToLow)
                        {
                            //HL HL
                            register.RegisterValue =
                                 (ushort)(receviceByteData[2 * i + byteCountBeforeDataRegion] * 256 +
                                 receviceByteData[2 * i + byteCountBeforeDataRegion + 1]);

                            //先加入低位
                            register.LowToHighDataBytes.Add(receviceByteData[2 * i + byteCountBeforeDataRegion + 1]);
                            //后加入高位
                            register.LowToHighDataBytes.Add(receviceByteData[2 * i + byteCountBeforeDataRegion]);
                        }
                        else
                        {
                            //LH LH 
                            register.RegisterValue =
                                 (ushort)(receviceByteData[2 * i + byteCountBeforeDataRegion] +
                                 receviceByteData[2 * i + byteCountBeforeDataRegion + 1] * 256);

                            //先加入低位
                            register.LowToHighDataBytes.Add(receviceByteData[2 * i + byteCountBeforeDataRegion]);
                            //后加入高位 
                            register.LowToHighDataBytes.Add(receviceByteData[2 * i + byteCountBeforeDataRegion + 1]);
                        }

                        reponse.Registers.Add(register);
                        ++startRegisterAddress;
                    }
                }
            }
            catch (Exception ex)
            {
                reponse.AnalyzeRecivedDataSuccess = false;
                //LoggingFactory.GetLogger().WriteDebugLogger("解析Modbus接收帧时发生异常！");
                reponse.ErrorMsg = "AnalyzeRecivedDataByFunNum03->解析Modbus接收帧时发生异常！" + ex.Message;
                return reponse;
            }

            return reponse;
        }

        /// <summary>
        /// 写单个线圈
        /// </summary>
        /// <param name="requestByteData"></param>
        /// <param name="receviceByteData"></param>
        /// <returns></returns>
        private static AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum05(
            DataAnalyzeMode dataAnalyzeMode,
            List<byte> requestByteData,
            List<byte> receviceByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            try
            {
                int deviceAddress = receviceByteData[0];
                int startRegisterAddress = requestByteData[2] * 256 + requestByteData[3];

                //接受侦receviceByteData中寄存器值的数据之前的字节数：
                //deviceAddress、funNum、ecevideByteData[2]个占1个字节
                int byteCountBeforeDataRegion = 4;
                int countOfRegisterToRead = 1;

                //避免数据返回不完整引发下标溢出
                if (receviceByteData.Count > (byteCountBeforeDataRegion + countOfRegisterToRead * 2) &&
                    requestByteData.Count > (byteCountBeforeDataRegion + countOfRegisterToRead * 2))
                {

                    if (requestByteData[byteCountBeforeDataRegion] == receviceByteData[byteCountBeforeDataRegion] &&
                        requestByteData[byteCountBeforeDataRegion + 1] == receviceByteData[byteCountBeforeDataRegion + 1])
                    {
                        reponse.ModbusReponseSuccess = true;
                    }
                    else
                    {
                        reponse.ModbusReponseSuccess = false;
                    }

                    Register register = new Register();
                    register.DeviceAddress = deviceAddress;
                    register.RegisterAddress = startRegisterAddress;

                    byte fistByteInRecevide = receviceByteData[byteCountBeforeDataRegion];
                    byte secondByteInRecevide = receviceByteData[byteCountBeforeDataRegion + 1];

                    byte fistByteInRequest = requestByteData[byteCountBeforeDataRegion];
                    byte secondByteInRequest = requestByteData[byteCountBeforeDataRegion + 1];

                    if (0xFF == fistByteInRecevide && 0x00 == secondByteInRecevide)
                    {
                        register.RegisterValue = 1; //开：FF00
                    }
                    else if (0x00 == fistByteInRecevide && 0x00 == secondByteInRecevide)
                    {
                        register.RegisterValue = 0; //关：0000
                    }

                    reponse.Registers.Add(register);
                }
            }
            catch (Exception ex)
            {
                reponse.AnalyzeRecivedDataSuccess = false;
                //LoggingFactory.GetLogger().WriteDebugLogger("解析Modbus接收帧时发生异常！");
                reponse.ErrorMsg = "AnalyzeRecivedDataByFunNum05->解析Modbus接收帧时发生异常！" + ex.Message;

                return reponse;
            }

            return reponse;
        }

        /// <summary>
        /// 写单个寄存器
        /// </summary>
        /// <param name="requestByteData"></param>
        /// <param name="receviceByteData"></param>
        /// <returns></returns>
        private static AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum06(
            DataAnalyzeMode dataAnalyzeMode,
            List<byte> requestByteData,
            List<byte> receviceByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            try
            {
                int deviceAddress = receviceByteData[0];

                //byte[] startRegisterByte = new byte[] { receviceByteData[2], receviceByteData[3] };
                //int startRegisterAddress = BitConverter.ToUInt16(startRegisterByte, 0);
                int startRegisterAddress = requestByteData[2] * 256 + requestByteData[3];

                //byte[] valueToSetByte = new byte[] { requestByteData[4], requestByteData[5] };
                //double valueToSet = BitConverter.ToDouble(valueToSetByte, 0);

                //接受侦receviceByteData中寄存器值的数据之前的字节数：
                //deviceAddress、funNum、ecevideByteData[2]个占1个字节
                int byteCountBeforeDataRegion = 4;
                int countOfRegisterToRead = 1;

                //避免数据返回不完整引发下标溢出
                if (receviceByteData.Count > (byteCountBeforeDataRegion + countOfRegisterToRead * 2)
                    && requestByteData.Count > (byteCountBeforeDataRegion + countOfRegisterToRead * 2))
                {
                    if (requestByteData[byteCountBeforeDataRegion] == receviceByteData[byteCountBeforeDataRegion] &&
                        requestByteData[byteCountBeforeDataRegion + 1] == receviceByteData[byteCountBeforeDataRegion + 1])
                    {
                        reponse.ModbusReponseSuccess = true;
                    }
                    else
                    {
                        reponse.ModbusReponseSuccess = false;
                    }

                    Register register = new Register();
                    register.DeviceAddress = deviceAddress;
                    register.RegisterAddress = startRegisterAddress;

                    //byte[] bytevalues = new byte[]
                    //    {
                    //         receviceByteData[byteCountBeforeDataRegion],
                    //         receviceByteData[byteCountBeforeDataRegion + 1],
                    //    };

                    //register.RegisterValue = BitConverter.ToUInt16(bytevalues, 0);
                    if (dataAnalyzeMode == DataAnalyzeMode.DataHighToLow)
                    {
                        //HL HL
                        register.RegisterValue = (ushort)(receviceByteData[byteCountBeforeDataRegion] * 256 +
                                  receviceByteData[byteCountBeforeDataRegion + 1]);


                        register.LowToHighDataBytes.Add(receviceByteData[byteCountBeforeDataRegion + 1]); //低位
                        register.LowToHighDataBytes.Add(receviceByteData[byteCountBeforeDataRegion]);     //高位
                    }
                    else
                    {
                        //LH LH
                        register.RegisterValue = (ushort)(receviceByteData[byteCountBeforeDataRegion] +
                                  receviceByteData[byteCountBeforeDataRegion + 1] * 256);

                        register.LowToHighDataBytes.Add(receviceByteData[byteCountBeforeDataRegion]);     //低位
                        register.LowToHighDataBytes.Add(receviceByteData[byteCountBeforeDataRegion + 1]); //高位
                    }


                    reponse.Registers.Add(register);
                }

            }
            catch (Exception ex)
            {
                reponse.AnalyzeRecivedDataSuccess = false;
                //LoggingFactory.GetLogger().WriteDebugLogger("解析Modbus接收帧时发生异常！");
                reponse.ErrorMsg = "AnalyzeRecivedDataByFunNum06->解析Modbus接收帧时发生异常！" + ex.Message;

                return reponse;
            }

            return reponse;
        }

        /// <summary>
        /// 设置多个线圈的值
        /// 根据Modbus协议，功能码15（0F），接收帧为
        /// 11 0F 0013 0009 
        /// 并不包含寄存器的值。
        /// </summary>
        /// <param name="requestByteData"></param>
        /// <param name="receviceByteData"></param>
        /// <returns></returns>
        private static AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum015(
            DataAnalyzeMode dataAnalyzeMode,
            List<byte> requestByteData,
            List<byte> receviceByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            //根据Modbus协议，功能码15（0F），接收帧为
            //11 0F 0013 0009 
            //并不包含寄存器的值。
            //故返回一个空的List<Register>
            try
            {
                //接受侦receviceByteData中寄存器值的数据之前的字节数：
                //deviceAddress、funNum、ecevideByteData[2]个占1个字节
                int byteCountBeforeDataRegion = 4;

                //避免数据返回不完整引发下标溢出
                if (receviceByteData.Count > (byteCountBeforeDataRegion + 2)
                    && requestByteData.Count > (byteCountBeforeDataRegion + 2))
                {
                    if (requestByteData[byteCountBeforeDataRegion] == receviceByteData[byteCountBeforeDataRegion] &&
                        requestByteData[byteCountBeforeDataRegion + 1] == receviceByteData[byteCountBeforeDataRegion + 1])
                    {
                        reponse.ModbusReponseSuccess = true;
                    }
                    else
                    {
                        reponse.ModbusReponseSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                reponse.AnalyzeRecivedDataSuccess = false;
                //LoggingFactory.GetLogger().WriteDebugLogger("解析Modbus接收帧时发生异常！");
                reponse.ErrorMsg = "AnalyzeRecivedDataByFunNum15->解析Modbus接收帧时发生异常！" + ex.Message;

                return reponse;
            }

            return reponse;
        }

        /// <summary>
        /// 设置多个寄存器的值
        ///根据Modbus协议，功能码16（10H），接收帧为
        ///11 0F 0013 0009 
        ///并不包含寄存器的值。
        /// </summary>
        /// <param name="requestByteData"></param>
        /// <param name="receviceByteData"></param>
        /// <returns></returns>
        private static AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum16(
            DataAnalyzeMode dataAnalyzeMode,
            List<byte> requestByteData,
            List<byte> receviceByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            //根据Modbus协议，功能码16（10H），接收帧为
            //11 0F 0013 0009 
            //并不包含寄存器的值。
            //故返回一个空的List<Register>

            try
            {
                int byteCountBeforeDataRegion = 4;

                //避免数据返回不完整引发下标溢出
                if (receviceByteData.Count > (byteCountBeforeDataRegion + 2)
                    && requestByteData.Count > (byteCountBeforeDataRegion + 2))
                {
                    if (requestByteData[byteCountBeforeDataRegion] == receviceByteData[byteCountBeforeDataRegion] &&
                        requestByteData[byteCountBeforeDataRegion + 1] == receviceByteData[byteCountBeforeDataRegion + 1])
                    {
                        reponse.ModbusReponseSuccess = true;
                    }
                    else
                    {
                        reponse.ModbusReponseSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                reponse.AnalyzeRecivedDataSuccess = false;
                //LoggingFactory.GetLogger().WriteDebugLogger("解析Modbus接收帧时发生异常！");
                reponse.ErrorMsg = "AnalyzeRecivedDataByFunNum16->解析Modbus接收帧时发生异常！" + ex.Message;

                return reponse;
            }

            return reponse;
        }

        #endregion
    }
}
