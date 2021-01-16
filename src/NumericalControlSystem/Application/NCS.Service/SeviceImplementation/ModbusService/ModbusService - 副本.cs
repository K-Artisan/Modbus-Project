using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using Modbus.Contract;
using Modbus.Contract.RequestDataBase;
using Modbus.RTU;
using NCS.Infrastructure.Logging;
using NCS.Model.DataPoints;
using NCS.Service.Helper;
using NCS.Service.Messaging.ModbusService;
using NCS.Service.ServiceInterface;

namespace NCS.Service.SeviceImplementation.ModbusService
{
    public class ModbusService : IModbusService
    {
        #region 成员变量

        private readonly IDataPointRepository dataPointRepository;

        private ModbusRTU modbusRtu;
        private SerialPort serialPort;
        private List<byte> currentRequestByteData = null;
        private List<byte> currentRecivedByteData = null;

        private List<DataPoint> allDataPoints;

        private bool stopRunModbusService = false;
        private Queue<ReadRegisterCommand> allReadRegisterCommands = new Queue<ReadRegisterCommand>(); 
        private Queue<WriteRegisterCommand> allWriterRegisterCommands = new Queue<WriteRegisterCommand>();
             

        /// <summary>
        /// key:DataPiont.Number
        /// value:DataPiont.RealTimeValue
        /// </summary>
        public static Dictionary<int, double> DataPiontRealTimeValueDictionary = new Dictionary<int, double>();

        #endregion

        #region 构造函数

        public ModbusService(IDataPointRepository dataPointRepository)
        {
            this.dataPointRepository = dataPointRepository;
        }

        #endregion

        #region 运行ModbusService

        public void Run()
        {
            InitializeRunningEnvironment();

            while (CanRun())
            {
                if (this.allWriterRegisterCommands.Count > 0)
                {
                    WriteRegister();
                }
                else
                {
                    ReadRegister();
                }
            }
        }

        private void InitializeRunningEnvironment()
        {
            InitializeData();
            InitializeAllReadRegisterCommands(this.allDataPoints);
            InitializeModbusRtu(this.serialPort);
        }

        private bool CanRun()
        {
            return !this.stopRunModbusService && this.serialPort.IsOpen;
        }

        private void WriteRegister()
        {
            WriteRegisterCommand writeRegisterCommand = this.allWriterRegisterCommands.Dequeue();
            this.modbusRtu.WriteSerialPort(writeRegisterCommand.WriteCommand.ToArray());
        }

        private void ReadRegister()
        {
            ReadRegisterCommand readRegisterCommand = this.allReadRegisterCommands.Dequeue();
            this.modbusRtu.WriteSerialPort(readRegisterCommand.ReadCommand.ToArray());

            this.allReadRegisterCommands.Enqueue(readRegisterCommand);
        }

        #endregion

        #region 初始化数据相关信息

        public void InitializeData()
        {
            this.allDataPoints = dataPointRepository.FindAll().ToList();
        }

        #endregion

        #region 初始化所有"读寄存器"的命令

        private void InitializeAllReadRegisterCommands(List<DataPoint> dataPoints)
        {
            if (null != dataPoints)
            {
                return;
            }

            List<List<DataPoint>> dataPointsGroupedForRead = DataPointGrouper.GroupingDataPointsForReadRegister(dataPoints);

            foreach (var dataPointsGroup in dataPointsGroupedForRead)
            {
                List<List<byte>> requestCommadBytes =
                    RequestCommandByteStreamCreater.CreateRequestCommandByteStreamForReadRegisterBy(dataPointsGroup);

                foreach (var requestCommadByte in requestCommadBytes)
                {
                    ReadRegisterCommand readRegisterCommand = new ReadRegisterCommand();
                    readRegisterCommand.ReadCommand = requestCommadByte;

                    //把所有的读命令保存在内存中，换取速度
                    this.allReadRegisterCommands.Enqueue(readRegisterCommand);
                }
            }
        }

        #endregion

        #region ModbusRTU 初始化

        public void InitializeModbusRtu(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            this.modbusRtu = new ModbusRTU(serialPort);

            this.modbusRtu.OnCurrentRequestDataChanged += new EventHandler<RequstDataEventArgs>(ModbusRtu_OnRequestDataChanged);
            this.modbusRtu.OnCurrentReceiveDataChanged += new EventHandler<ReceiveDataEventArgs>(ModbusRtu_OnReceiveDataChanged);
        }

        private void ModbusRtu_OnRequestDataChanged(object sender, RequstDataEventArgs e)
        {
            List<byte> requstData = e.RequstData;
            this.currentRequestByteData = requstData;
        }

        private void ModbusRtu_OnReceiveDataChanged(object sender, ReceiveDataEventArgs e)
        {
            List<byte> recevideData = e.ReceiveData;

            if (null != recevideData)
            {
                this.currentRecivedByteData = recevideData;

                AnalyzeRecivedDataReponse reponse= AnalyzeRecivedData(this.currentRequestByteData, this.currentRequestByteData);

                ProcessRegisterValue(reponse.Registers, this.allDataPoints);

                //if (reponse.DataPointType == DataPointType.WriteAndReadByFunNum01
                //    || reponse.DataPointType == DataPointType.WriteAndReadByFunNum03)
                //{
                //    //TODO:分析写寄存器接受侦
                //}

            }
        }

        #endregion

        

        #region "写寄存器"的命令

        private void AddWriteRegisterCommands(List<DataPoint> dataPoints)
        {
             WriteRegisterCommand writeRegisterCommand =new WriteRegisterCommand();
             this.allWriterRegisterCommands.Enqueue(writeRegisterCommand);
        }

        #endregion

        #region 根据功能码将接受帧解析成一些列寄存器值

        private AnalyzeRecivedDataReponse AnalyzeRecivedData(List<byte> requestByteData, List<byte> recevideByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            if (recevideByteData.Count < 1 || requestByteData.Count < 1)
            {
                //无法获取设备地址、功能码
                string message = "请求帧或接受帧中不存在设备地址或功能码。";
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return reponse;
            }

            if (requestByteData[0] != recevideByteData[0]
                || requestByteData[1] != recevideByteData[1])
            {
                string message = "请求帧与接受帧的设备地址或功能码不对应，" +
                                 "请求接受的数据不对应。";
                LoggingFactory.GetLogger().WriteDebugLogger(message);
                return reponse;
            }

            int funNum = recevideByteData[1];

            if (1 == funNum)
            {
                reponse = AnalyzeRecivedDataByFunNum01(requestByteData, recevideByteData);
            }

            if (3 == funNum)
            {
                reponse = AnalyzeRecivedDataByFunNum03(requestByteData, recevideByteData);
            }

            if (5 == funNum)
            {
                reponse = AnalyzeRecivedDataByFunNum05(requestByteData, recevideByteData);
            }

            if (6 == funNum)
            {
                reponse = AnalyzeRecivedDataByFunNum06(requestByteData, recevideByteData);
            }

            if (15 == funNum)
            {
                reponse = AnalyzeRecivedDataByFunNum015(requestByteData, recevideByteData);
            }

            if (16 == funNum)
            {
                reponse = AnalyzeRecivedDataByFunNum16(requestByteData, recevideByteData);
            }

            return reponse;
        }

        private AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum01(List<byte> requestByteData, List<byte> recevideByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            try
            {
                int deviceAddress = requestByteData[0];

                byte[] startRegisterByte = new byte[] { requestByteData[2], requestByteData[3] };
                int startRegisterAddress = BitConverter.ToInt16(startRegisterByte, 0);

                byte[] readRegisterCountByte = new byte[] { requestByteData[4], requestByteData[5] };
                int countOfRegisterToRead = BitConverter.ToInt16(readRegisterCountByte, 0);

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

        private AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum03(List<byte> requestByteData, List<byte> recevideByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            try
            {
                int deviceAddress = requestByteData[0];

                byte[] startRegisterByte = new byte[] { requestByteData[2], requestByteData[3] };
                int startRegisterAddress = BitConverter.ToInt16(startRegisterByte, 0);

                //接受侦recevideByteData中寄存器值的数据之前的字节数：
                //deviceAddress、funNum、recevideByteData[2]个占1个字节
                int byteCountBeforeDataRegion = 3;
                //寄存器的个数
                int countOfRegisterHadRead = recevideByteData[2] / 2;

                //避免数据返回不完整引发下标溢出
                if (recevideByteData.Count > (byteCountBeforeDataRegion + recevideByteData[2]))
                {
                    for (int i = 0; i < countOfRegisterHadRead; i++)
                    {
                        Register register = new Register();
                        register.DeviceAddress = deviceAddress;
                        register.RegisterAddress = startRegisterAddress;

                        byte[] bytevalues = new byte[]
                        {
                             recevideByteData[2 * i + byteCountBeforeDataRegion],
                             recevideByteData[2 * i + byteCountBeforeDataRegion + 1],
                        };

                        register.RegisterValue = BitConverter.ToUInt16(bytevalues, 0);

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
        private AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum05(List<byte> requestByteData, List<byte> recevideByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            try
            {
                int deviceAddress = recevideByteData[0];

                byte[] startRegisterByte = new byte[] { recevideByteData[2], recevideByteData[3] };
                int startRegisterAddress = BitConverter.ToInt16(startRegisterByte, 0);

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
        private AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum06(List<byte> requestByteData, List<byte> recevideByteData)
        {
            AnalyzeRecivedDataReponse reponse = new AnalyzeRecivedDataReponse();

            try
            {
                int deviceAddress = recevideByteData[0];

                byte[] startRegisterByte = new byte[] { recevideByteData[2], recevideByteData[3] };
                int startRegisterAddress = BitConverter.ToInt16(startRegisterByte, 0);

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
                    if ( requestByteData[byteCountBeforeDataRegion] == recevideByteData[byteCountBeforeDataRegion] &&
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

                    byte[] bytevalues = new byte[]
                        {
                             recevideByteData[byteCountBeforeDataRegion],
                             recevideByteData[byteCountBeforeDataRegion + 1],
                        };

                    register.RegisterValue = BitConverter.ToUInt16(bytevalues, 0);
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
        private AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum015(List<byte> requestByteData, List<byte> recevideByteData)
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
        private AnalyzeRecivedDataReponse AnalyzeRecivedDataByFunNum16(List<byte> requestByteData, List<byte> recevideByteData)
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

        #region 设置DataPiont的实时数据

        private void ProcessRegisterValue(List<Register> registers, List<DataPoint> dataPoints)
        {
            if (null == registers || null == dataPoints)
            {
                return;
            }

            List<DataPoint> dataPointsWhoseRealTimeDataChanged = new List<DataPoint>();

            for (int i = 0; i < registers.Count; i++)
            {
                DataPoint dataPoint = dataPoints.Find(
                    p => p.DeviceAddress == registers[i].DeviceAddress 
                        && p.StartRegisterAddress== registers[i].RegisterAddress);

                if (null != dataPoint)
                {
                    switch (dataPoint.DataType)
                    {
                        case DataType.S16:
                            {
                                byte[] byteValues = BitConverter.GetBytes(registers[i].RegisterValue);
                                double realTimeValue = BitConverter.ToInt16(byteValues, 0);

                                if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                {
                                    dataPoint.RealTimeValue = realTimeValue;
                                    dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                }
                                break;

                            }
                        case DataType.U16:
                            {
                                byte[] byteValues = BitConverter.GetBytes(registers[i].RegisterValue);
                                double realTimeValue = BitConverter.ToUInt16(byteValues, 0);

                                if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                {
                                    dataPoint.RealTimeValue = realTimeValue;
                                    dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                }

                                break;
                            }
                        case DataType.S32:
                            {
                                if (i + 1 < registers.Count)
                                {
                                    byte[] byteValuesLow = BitConverter.GetBytes(registers[i].RegisterValue);
                                    byte[] byteValuesHigh = BitConverter.GetBytes(registers[i + 1].RegisterValue);
                                    byte[] byteValues = new byte[]
                                    {
                                        byteValuesLow[0],
                                        byteValuesLow[1],
                                        byteValuesHigh[0],
                                        byteValuesHigh[1]

                                    };

                                    double realTimeValue = BitConverter.ToInt32(byteValues, 0);

                                    if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                    {
                                        dataPoint.RealTimeValue = realTimeValue;
                                        dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                    }
                                }
                                break;
                            }
                        case DataType.U32:
                            {
                                if (i + 1 < registers.Count)
                                {
                                    byte[] byteValuesLow = BitConverter.GetBytes(registers[i].RegisterValue);
                                    byte[] byteValuesHigh = BitConverter.GetBytes(registers[i + 1].RegisterValue);
                                    byte[] byteValues = new byte[]
                                    {
                                        byteValuesLow[0],
                                        byteValuesLow[1],
                                        byteValuesHigh[0],
                                        byteValuesHigh[1]

                                    };

                                    double realTimeValue = BitConverter.ToUInt32(byteValues, 0);

                                    if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                    {
                                        dataPoint.RealTimeValue = realTimeValue;
                                        dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                    }
                                }

                                break;
                            }

                        case DataType.S64:
                            {
                                if (i + 3 < registers.Count)
                                {
                                    byte[] byteValues01 = BitConverter.GetBytes(registers[i].RegisterValue);
                                    byte[] byteValues02 = BitConverter.GetBytes(registers[i + 1].RegisterValue);
                                    byte[] byteValues03 = BitConverter.GetBytes(registers[i + 2].RegisterValue);
                                    byte[] byteValues04 = BitConverter.GetBytes(registers[i + 3].RegisterValue);

                                    byte[] byteValues = new byte[]
                                    {
                                        byteValues01[0],
                                        byteValues01[1],
                                        byteValues02[0],
                                        byteValues02[1],
                                        byteValues03[0],
                                        byteValues03[1],
                                        byteValues04[0],
                                        byteValues04[1]
                                    };

                                    double realTimeValue = BitConverter.ToInt64(byteValues, 0);

                                    if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                    {
                                        dataPoint.RealTimeValue = realTimeValue;
                                        dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                    }
                                }

                                break;
                            }
                        case DataType.U64:
                            {
                                if (i + 3 < registers.Count)
                                {
                                    byte[] byteValues01 = BitConverter.GetBytes(registers[i].RegisterValue);
                                    byte[] byteValues02 = BitConverter.GetBytes(registers[i + 1].RegisterValue);
                                    byte[] byteValues03 = BitConverter.GetBytes(registers[i + 2].RegisterValue);
                                    byte[] byteValues04 = BitConverter.GetBytes(registers[i + 3].RegisterValue);

                                    byte[] byteValues = new byte[]
                                    {
                                        byteValues01[0],
                                        byteValues01[1],
                                        byteValues02[0],
                                        byteValues02[1],
                                        byteValues03[0],
                                        byteValues03[1],
                                        byteValues04[0],
                                        byteValues04[1]
                                    };

                                    double realTimeValue = BitConverter.ToUInt64(byteValues, 0);

                                    if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                    {
                                        dataPoint.RealTimeValue = realTimeValue;
                                        dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                    }
                                }

                                break;
                            }
                        case DataType.F32:
                            {
                                if (i + 1 < registers.Count)
                                {
                                    byte[] byteValuesLow = BitConverter.GetBytes(registers[i].RegisterValue);
                                    byte[] byteValuesHigh = BitConverter.GetBytes(registers[i + 1].RegisterValue);
                                    byte[] byteValues = new byte[]
                                    {
                                        byteValuesLow[0],
                                        byteValuesLow[1],
                                        byteValuesHigh[0],
                                        byteValuesHigh[1]

                                    };

                                    double realTimeValue = BitConverter.ToSingle(byteValues, 0);

                                    if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                    {
                                        dataPoint.RealTimeValue = realTimeValue;
                                        dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                    }
                                }

                                break;
                            }
                        case DataType.D64:
                            {
                                if (i + 3 < registers.Count)
                                {
                                    byte[] byteValues01 = BitConverter.GetBytes(registers[i].RegisterValue);
                                    byte[] byteValues02 = BitConverter.GetBytes(registers[i + 1].RegisterValue);
                                    byte[] byteValues03 = BitConverter.GetBytes(registers[i + 2].RegisterValue);
                                    byte[] byteValues04 = BitConverter.GetBytes(registers[i + 3].RegisterValue);

                                    byte[] byteValues = new byte[]
                                    {
                                        byteValues01[0],
                                        byteValues01[1],
                                        byteValues02[0],
                                        byteValues02[1],
                                        byteValues03[0],
                                        byteValues03[1],
                                        byteValues04[0],
                                        byteValues04[1]
                                    };

                                    double realTimeValue = BitConverter.ToDouble(byteValues, 0);

                                    if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                    {
                                        dataPoint.RealTimeValue = realTimeValue;
                                        dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                    }
                                }

                                break;
                            }
                        case DataType.Bit:
                            {
                                double realTimeValue = registers[i].RegisterValue;

                                if (Math.Abs(realTimeValue - dataPoint.RealTimeValue) > 0)
                                {
                                    dataPoint.RealTimeValue = realTimeValue;
                                    dataPointsWhoseRealTimeDataChanged.Add(dataPoint);
                                }
                                break;
                            }
                        default:
                            throw new ArgumentOutOfRangeException();
                           
                    }
                }
            }

            //TODO:上传：dataPointsWhoseRealTimeDataChanged
            //TODO:将值发生数据点筛选出来，以事件的形式发布数据变化的值。
        }

        #endregion

        #region  IModbusService members

        public GetDataPointRealTimeDataResponse GetDataPointRealTimeData(Messaging.ModbusService.GetDataPointRealTimeDataRequest request)
        {
            GetDataPointRealTimeDataResponse response = new GetDataPointRealTimeDataResponse();
            return response;
        }

        public GetAllDataPointsRealTimeDataResponse GetAllDataPointsRealTimeData()
        {
            GetAllDataPointsRealTimeDataResponse response = new GetAllDataPointsRealTimeDataResponse();


            return response;
        }

        public void SetDataPointValue(SetDataPointValueRequest request)
        {
              
        }

        #endregion 
    }
}
