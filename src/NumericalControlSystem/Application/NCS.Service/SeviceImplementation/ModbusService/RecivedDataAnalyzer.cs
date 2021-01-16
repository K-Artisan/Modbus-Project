using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modbus.Contract.RequestDataBase;
using NCS.Infrastructure.Logging;

namespace NCS.Service.SeviceImplementation.ModbusService
{
    public static class RecivedDataAnalyzer
    {
        private static DataAnalyzeMode dataAnalyzeMode = DataAnalyzeMode.DataLowToHigh;

        #region 根据功能码将接受帧解析成一些列寄存器值

        public static AnalyzeRecivedDataReponse AnalyzeRecivedData(DataAnalyzeMode dataAnalyzeMode ,List<byte> requestByteData, List<byte> recevideByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            //TODO:暂时注释，调试完后取消注释
            if (!RecivedDataCorrespondToRequesData(requestByteData, recevideByteData))
            {
                reponse.ModbusReponseSuccess = false;
                return reponse;
            }

            if (recevideByteData.Count < 2 || requestByteData.Count < 2)
            {
                //无法获取设备地址、功能码
                string message = "请求帧或接受帧中不存在设备地址或功能码。";
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                reponse.ModbusReponseSuccess = false;
                return reponse;
            }

            if (requestByteData[0] != recevideByteData[0]
                || requestByteData[1] != recevideByteData[1])
            {
                string message = "请求帧与接受帧的设备地址或功能码不对应，" +
                                 "请求接受的数据不对应。";
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                reponse.AnalyzeRecivedDataSuccess = false;
                return reponse;
            }

            int funNum = recevideByteData[1];

            if (1 == funNum)
            {
                reponse = AnalyzeRecivedDataByFunNum01(dataAnalyzeMode, requestByteData, recevideByteData);
            }

            if (3 == funNum)
            {
                reponse = AnalyzeRecivedDataByFunNum03(dataAnalyzeMode, requestByteData, recevideByteData);
            }

            if (5 == funNum)
            {
                reponse = AnalyzeRecivedDataByFunNum05(dataAnalyzeMode, requestByteData, recevideByteData);
            }

            if (6 == funNum)
            {
                reponse = AnalyzeRecivedDataByFunNum06(dataAnalyzeMode, requestByteData, recevideByteData);
            }

            if (15 == funNum)
            {
                reponse = AnalyzeRecivedDataByFunNum015(dataAnalyzeMode, requestByteData, recevideByteData);
            }

            if (16 == funNum)
            {
                reponse = AnalyzeRecivedDataByFunNum16(dataAnalyzeMode, requestByteData, recevideByteData);
            }

            return reponse;
        }

        /// <summary>
        /// 判读接受的数据是否对应请求,
        /// 根据GUID判断
        /// </summary>
        /// <returns></returns>
        private static bool RecivedDataCorrespondToRequesData(List<byte> requestByteData, List<byte> recevideByteData)
        {

            bool correspond = true;

            try
            {
                //请求或返回起码长度为4：1个字节设备地址、1个字节功能码、2个字节CRC校验位
                //16是16个字节的GUID。
                int byteDataMinLenth = 4 + 16;
                if (requestByteData.Count < byteDataMinLenth)
                {
                    correspond = false;
                }

                //数据溢出也表明响应的数据不对。
                List<byte> requestGuid = GetGuidBytesForm(requestByteData);
                List<byte> recieveGuid = GetGuidBytesForm(recevideByteData);

                for (int i = 0; i < 16; i++)
                {
                    if (requestGuid[i] != recieveGuid[i])
                    {
                        correspond = false;
                        break;
                    }
                }

            }
            catch (Exception)
            {
                correspond = false;
            }

            return correspond;
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
            List<byte> recevideByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            try
            {
                int deviceAddress = requestByteData[0];

                int startRegisterAddress = requestByteData[2] * 256 + requestByteData[3];
                int countOfRegisterToRead = requestByteData[4] * 256 + requestByteData[5];


                //接受侦recevideByteData中寄存器值的数据之前的字节数：
                //deviceAddress、funNum、recevideByteData[2]个占1个字节
                int byteCountBeforeDataRegion = 3;
                //数据区表示寄存器值的字节数
                int registerValueByteCount = recevideByteData[2];

                //避免数据返回不完整引发下标溢出
                if (recevideByteData.Count > (byteCountBeforeDataRegion + recevideByteData[2]))
                {
                    byte byteData = 0;
                    List<UInt16> totalRecvVal = new List<ushort>();
                    for (int k = 0; k < registerValueByteCount; k++)
                    {
                        byteData = recevideByteData[byteCountBeforeDataRegion + k];

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
            catch (Exception)
            {
                reponse.AnalyzeRecivedDataSuccess = false;
                LoggingFactory.GetLogger().WriteDebugLogger("解析Modbus接收帧时发生异常！");
                return reponse;
            }


            return reponse;
        }

        private static AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum03(
            DataAnalyzeMode dataAnalyzeMode,
            List<byte> requestByteData, 
            List<byte> recevideByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            try
            {
                //接受侦recevideByteData中寄存器值的数据之前的字节数：
                //deviceAddress、funNum、recevideByteData[2]个占1个字节
                int byteCountBeforeDataRegion = 3;

                //避免数据返回不完整引发下标溢出
                if (recevideByteData.Count > (byteCountBeforeDataRegion + recevideByteData[2]))
                {
                    int deviceAddress = requestByteData[0];
                    int startRegisterAddress = requestByteData[2]*256 + requestByteData[3];

                    //寄存器的个数
                    int countOfRegisterHadRead = recevideByteData[2] / 2;

                    for (int i = 0; i < countOfRegisterHadRead; i++)
                    {
                        Register register = new Register();
                        register.DeviceAddress = deviceAddress;
                        register.RegisterAddress = startRegisterAddress;

                        if (dataAnalyzeMode == DataAnalyzeMode.DataHighToLow)
                        {
                            //HL HL
                            register.RegisterValue =
                                 (ushort)(recevideByteData[2 * i + byteCountBeforeDataRegion] * 256 +
                                 recevideByteData[2 * i + byteCountBeforeDataRegion + 1]);

                            //先加入低位
                            register.LowToHighDataBytes.Add(recevideByteData[2 * i + byteCountBeforeDataRegion + 1]);
                            //后加入高位
                            register.LowToHighDataBytes.Add(recevideByteData[2 * i + byteCountBeforeDataRegion]);
                        }
                        else
                        {
                            //LH LH 
                            register.RegisterValue =
                                 (ushort)(recevideByteData[2 * i + byteCountBeforeDataRegion] +
                                 recevideByteData[2 * i + byteCountBeforeDataRegion + 1] * 256);

                            //先加入低位
                            register.LowToHighDataBytes.Add(recevideByteData[2 * i + byteCountBeforeDataRegion]);
                            //后加入高位 
                            register.LowToHighDataBytes.Add(recevideByteData[2 * i + byteCountBeforeDataRegion + 1]);
                        }

                        ;
                        reponse.Registers.Add(register);
                        ++startRegisterAddress;
                    }
                }
            }
            catch (Exception)
            {
                reponse.AnalyzeRecivedDataSuccess = false;
                LoggingFactory.GetLogger().WriteDebugLogger("解析Modbus接收帧时发生异常！");
                return reponse;
            }

            return reponse;
        }

        /// <summary>
        /// 写单个线圈
        /// </summary>
        /// <param name="requestByteData"></param>
        /// <param name="recevideByteData"></param>
        /// <returns></returns>
        private static AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum05(
            DataAnalyzeMode dataAnalyzeMode,
            List<byte> requestByteData, 
            List<byte> recevideByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            try
            {
                int deviceAddress = recevideByteData[0];
                int startRegisterAddress = requestByteData[2] * 256 + requestByteData[3];

                //接受侦recevideByteData中寄存器值的数据之前的字节数：
                //deviceAddress、funNum、ecevideByteData[2]个占1个字节
                int byteCountBeforeDataRegion = 4;
                int countOfRegisterToRead = 1;

                //避免数据返回不完整引发下标溢出
                if (recevideByteData.Count > (byteCountBeforeDataRegion + countOfRegisterToRead * 2) &&
                    requestByteData.Count > (byteCountBeforeDataRegion + countOfRegisterToRead * 2))
                {

                    if (requestByteData[byteCountBeforeDataRegion] == recevideByteData[byteCountBeforeDataRegion] &&
                        requestByteData[byteCountBeforeDataRegion + 1] == recevideByteData[byteCountBeforeDataRegion + 1])
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

                    byte fistByteInRecevide = recevideByteData[byteCountBeforeDataRegion];
                    byte secondByteInRecevide = recevideByteData[byteCountBeforeDataRegion + 1];

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
            catch (Exception)
            {
                reponse.AnalyzeRecivedDataSuccess = false;
                LoggingFactory.GetLogger().WriteDebugLogger("解析Modbus接收帧时发生异常！");
                return reponse;
            }

            return reponse;
        }

        /// <summary>
        /// 写单个寄存器
        /// </summary>
        /// <param name="requestByteData"></param>
        /// <param name="recevideByteData"></param>
        /// <returns></returns>
        private static AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum06(
            DataAnalyzeMode dataAnalyzeMode,
            List<byte> requestByteData, 
            List<byte> recevideByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            try
            {
                int deviceAddress = recevideByteData[0];

                //byte[] startRegisterByte = new byte[] { recevideByteData[2], recevideByteData[3] };
                //int startRegisterAddress = BitConverter.ToUInt16(startRegisterByte, 0);
                int startRegisterAddress = requestByteData[2] * 256 + requestByteData[3];

                //byte[] valueToSetByte = new byte[] { requestByteData[4], requestByteData[5] };
                //double valueToSet = BitConverter.ToDouble(valueToSetByte, 0);

                //接受侦recevideByteData中寄存器值的数据之前的字节数：
                //deviceAddress、funNum、ecevideByteData[2]个占1个字节
                int byteCountBeforeDataRegion = 4;
                int countOfRegisterToRead = 1;

                //避免数据返回不完整引发下标溢出
                if (recevideByteData.Count > (byteCountBeforeDataRegion + countOfRegisterToRead * 2)
                    && requestByteData.Count > (byteCountBeforeDataRegion + countOfRegisterToRead * 2))
                {
                    if (requestByteData[byteCountBeforeDataRegion] == recevideByteData[byteCountBeforeDataRegion] &&
                        requestByteData[byteCountBeforeDataRegion + 1] == recevideByteData[byteCountBeforeDataRegion + 1])
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
                    //         recevideByteData[byteCountBeforeDataRegion],
                    //         recevideByteData[byteCountBeforeDataRegion + 1],
                    //    };
                     
                    //register.RegisterValue = BitConverter.ToUInt16(bytevalues, 0);
                    if (dataAnalyzeMode == DataAnalyzeMode.DataHighToLow)
                    {
                        //HL HL
                        register.RegisterValue = (ushort)(recevideByteData[byteCountBeforeDataRegion] * 256 +
                                  recevideByteData[byteCountBeforeDataRegion + 1]);


                        register.LowToHighDataBytes.Add(recevideByteData[byteCountBeforeDataRegion + 1]); //低位
                        register.LowToHighDataBytes.Add(recevideByteData[byteCountBeforeDataRegion]);     //高位
                    }
                    else
                    {
                        //LH LH
                        register.RegisterValue = (ushort)(recevideByteData[byteCountBeforeDataRegion] +
                                  recevideByteData[byteCountBeforeDataRegion + 1] * 256);

                        register.LowToHighDataBytes.Add(recevideByteData[byteCountBeforeDataRegion]);     //低位
                        register.LowToHighDataBytes.Add(recevideByteData[byteCountBeforeDataRegion + 1]); //高位
                    }


                    reponse.Registers.Add(register);
                }

            }
            catch (Exception)
            {
                reponse.AnalyzeRecivedDataSuccess = false;
                LoggingFactory.GetLogger().WriteDebugLogger("解析Modbus接收帧时发生异常！");
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
        /// <param name="recevideByteData"></param>
        /// <returns></returns>
        private static AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum015(
            DataAnalyzeMode dataAnalyzeMode,
            List<byte> requestByteData, 
            List<byte> recevideByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            //根据Modbus协议，功能码15（0F），接收帧为
            //11 0F 0013 0009 
            //并不包含寄存器的值。
            //故返回一个空的List<Register>
            try
            {
                //接受侦recevideByteData中寄存器值的数据之前的字节数：
                //deviceAddress、funNum、ecevideByteData[2]个占1个字节
                int byteCountBeforeDataRegion = 4;

                //避免数据返回不完整引发下标溢出
                if (recevideByteData.Count > (byteCountBeforeDataRegion + 2)
                    && requestByteData.Count > (byteCountBeforeDataRegion + 2))
                {
                    if (requestByteData[byteCountBeforeDataRegion] == recevideByteData[byteCountBeforeDataRegion] &&
                        requestByteData[byteCountBeforeDataRegion + 1] == recevideByteData[byteCountBeforeDataRegion + 1])
                    {
                        reponse.ModbusReponseSuccess = true;
                    }
                    else
                    {
                        reponse.ModbusReponseSuccess = false;
                    }
                }
            }
            catch (Exception)
            {
                reponse.AnalyzeRecivedDataSuccess = false;
                LoggingFactory.GetLogger().WriteDebugLogger("解析Modbus接收帧时发生异常！");
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
        /// <param name="recevideByteData"></param>
        /// <returns></returns>
        private static AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum16(
            DataAnalyzeMode dataAnalyzeMode,
            List<byte> requestByteData, 
            List<byte> recevideByteData)
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
                if (recevideByteData.Count > (byteCountBeforeDataRegion + 2)
                    && requestByteData.Count > (byteCountBeforeDataRegion + 2))
                {
                    if (requestByteData[byteCountBeforeDataRegion] == recevideByteData[byteCountBeforeDataRegion] &&
                        requestByteData[byteCountBeforeDataRegion + 1] == recevideByteData[byteCountBeforeDataRegion + 1])
                    {
                        reponse.ModbusReponseSuccess = true;
                    }
                    else
                    {
                        reponse.ModbusReponseSuccess = false;
                    }
                }
            }
            catch (Exception)
            {
                reponse.AnalyzeRecivedDataSuccess = false;
                LoggingFactory.GetLogger().WriteDebugLogger("解析Modbus接收帧时发生异常！");
                return reponse;
            }

            return reponse;
        }

        #endregion
    }
}
