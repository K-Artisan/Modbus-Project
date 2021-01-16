using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Xml;
using Modbus.Contract;
using Modbus.Contract.RequestDataBase;
using Modbus.RTU;
using NCS.Infrastructure.Configuration;
using NCS.Infrastructure.Logging;
using NCS.Infrastructure.Querying;
using NCS.Infrastructure.UnitOfWork;



using NCS.Service.Helper;
using NCS.Service.Mapping;
using NCS.Service.Messaging.ModbusService;
using NCS.Service.ServiceInterface;
using NCS.Service.ViewModel.DataPoints;
using NCS.Model.Entity;
using NCS.Model.Repository;

namespace NCS.Service.SeviceImplementation.ModbusService
{
    public class ModbusService : IModbusService
    {
        #region 成员变量

        private readonly IDataPointRepository dataPointRepository;
        private readonly IDataPointHistoryDataRepository dataPointHistoryDataRepository;

        private ModbusRTU modbusRtu;
        private List<byte> currentRequestByteData = null;
        private List<byte> currentRecivedByteData = null;

        private List<DataPoint> allDataPoints;

        private bool startRunModbusService = true;
        private bool canSendNextRequestCommandBytes = true;     //用于保证：请求必须有响应数据后才能再发下一条请求
        private int serialPortNotResponseCount = 0;             //串口不响应数据的次数
        private const int SerialPortNotResponseGiveUpValue = 3; //串口无响应超过该值，放弃该次请求
        private WriteRegisterCommand CurrentWriteRegisterCommand = null;
        private ReadRegisterCommand CurrentReadRegisterCommand = null;
        private Queue<ReadRegisterCommand> allReadRegisterCommands = new Queue<ReadRegisterCommand>(); 
        private Queue<WriteRegisterCommand> allWriterRegisterCommands = new Queue<WriteRegisterCommand>();

        private Thread readAndWriteMobusThread = null;
        private System.Timers.Timer saveDataPointHistoryValueTimer = null;   //定时保存为历史数据
        private readonly object saveDataPointHistoryValueLock = new object();

        #endregion

        #region 构造函数

        public ModbusService(IDataPointRepository dataPointRepository,
            IDataPointHistoryDataRepository dataPointHistoryDataRepository)
        {
            this.dataPointRepository = dataPointRepository;
            this.dataPointHistoryDataRepository = dataPointHistoryDataRepository;
            InitializeTimer();

            Run();
        }

        #endregion

        #region 处理历史数据：定时保存

        private void InitializeTimer()
        {
            saveDataPointHistoryValueTimer = new System.Timers.Timer();
            saveDataPointHistoryValueTimer.Interval = DateTimeHelper.GetCurrentTimeUntilNextHourInterval();
            saveDataPointHistoryValueTimer.Enabled = true;
            saveDataPointHistoryValueTimer.AutoReset = true;
            saveDataPointHistoryValueTimer.Elapsed += SaveDataPointHistoryValue;
            saveDataPointHistoryValueTimer.Start();
        }

        private void SaveDataPointHistoryValue(object obj, EventArgs e)
        {
            lock (this.saveDataPointHistoryValueLock)
            {
                this.saveDataPointHistoryValueTimer.Interval = DateTimeHelper.GetCurrentTimeUntilNextHourInterval();
                SaveDataPointHistoryValue();
            }
        }

        private void SaveDataPointHistoryValue()
        {
            if (null != this.allDataPoints && null != this.dataPointHistoryDataRepository)
            {
                foreach (var dataPoint in this.allDataPoints)
                {
                    DataPointHistoryData dataPointHistoryData = new DataPointHistoryData();

                    //蛋疼的UintOfWork机制要求：插入前要Id！！解决办法:
                    //每添加一个,就commit一次 uwRepository.UnitOfWork.Commit();
                    dataPointHistoryData.Id = "123";//Guid.NewGuid().ToString("D");
                    dataPointHistoryData.DataPoint = dataPoint;
                    dataPointHistoryData.DateTime = DateTime.Now;
                    dataPointHistoryData.Value = dataPoint.RealTimeValue;

                    try
                    {
                        this.dataPointHistoryDataRepository.Add(dataPointHistoryData);
                        var uwRepository = this.dataPointHistoryDataRepository as IUnitOfWorkRepository;
                        if (uwRepository != null)
                        {
                            uwRepository.UnitOfWork.Commit();
                        }
                    }
                    catch (Exception)
                    {
                        //保证一个小时内在异常不出现后能将这小时内的数据保存到数据库
                        this.saveDataPointHistoryValueTimer.Interval = 1000 * 5;
                        return;
                    }
                }


            }
        }

        #endregion

        #region 运行ModbusService

        public void Run()
        {
            InitializeRunningEnvironment();
            InitializeReadAndWriteMobusThread();
        }

        public void RestartWhenModbusSerialPortChanged()
        {
            InitializeReadAndWriteMobusThread();
        }

        private void InitializeReadAndWriteMobusThread()
        {
            this.readAndWriteMobusThread = new Thread(new ThreadStart(ReadAndWriteMobus));
            this.readAndWriteMobusThread.IsBackground = true;
            this.readAndWriteMobusThread.Start();
        }

        private void ReadAndWriteMobus()
        {
            try
            {
                while (CanRun())
                {
                    if (this.allWriterRegisterCommands.Count > 0)
                    {
                        WriteRegister();
                    }
                    else if (this.allReadRegisterCommands.Count > 0)
                    {
                        ReadRegister();
                    }
                }
            }
            catch (Exception)
            {
                //异常会使得线程Stoped,处于Stoped状态的线程不能重启。
                //故需要重新初始化线程并启动。
                InitializeReadAndWriteMobusThread();
            }

        }

        private void InitializeRunningEnvironment()
        {
            InitializeData();
            InitializeAllReadRegisterCommands(this.allDataPoints);
            InitializeModbusRtuSerialPortFormConfigFile();
        }

        private bool CanRun()
        {
            return this.startRunModbusService
                && null != this.modbusRtu.GetCurrentSerialPort()
                && this.modbusRtu.GetCurrentSerialPort().IsOpen;
        }

        private void WriteRegister()
        {
            if (this.canSendNextRequestCommandBytes)
            {
                this.CurrentWriteRegisterCommand = this.allWriterRegisterCommands.Dequeue();
                if (null != this.CurrentWriteRegisterCommand )
                {
                    this.serialPortNotResponseCount = 0;

                    this.modbusRtu.WriteSerialPort(this.CurrentWriteRegisterCommand.WriteCommand.ToArray());
                    this.canSendNextRequestCommandBytes = false;
                }
            }
            else
            {
                if (SerialPortNotResponseGiveUpValue <= ++this.serialPortNotResponseCount)
                {
                    this.canSendNextRequestCommandBytes = true;
                }
                else
                {
                    //重发
                    if (null != this.CurrentWriteRegisterCommand)
                    {
                        this.modbusRtu.WriteSerialPort(this.CurrentWriteRegisterCommand.WriteCommand.ToArray());
                        this.canSendNextRequestCommandBytes = false;
                    } 
                }

                System.Threading.Thread.Sleep(2000);
            }
        }

        private void ReadRegister()
        {
            if (null == this.allReadRegisterCommands 
                || this.allReadRegisterCommands.Count < 1)
            {
                return;
            }

            if (canSendNextRequestCommandBytes)
            {
                this.CurrentReadRegisterCommand = this.allReadRegisterCommands.Dequeue();
                 if (null != this.CurrentReadRegisterCommand)
                {
                    this.serialPortNotResponseCount = 0;

                    //TODO:删除，测试用的
                    //List<byte> textBytes = new List<byte>();

                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x03);
                    //textBytes.Add((byte)0x07);
                    //textBytes.Add((byte)0xD4);
                    //textBytes.Add((byte)0x00);
                    //textBytes.Add((byte)0x01);
                    //textBytes.Add((byte)0x09);
                    //textBytes.Add((byte)0x09);

                    ////GUID
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);

                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);

                    //this.modbusRtu.WriteSerialPort(textBytes.ToArray());

                    this.modbusRtu.WriteSerialPort(this.CurrentReadRegisterCommand.ReadCommand.ToArray());

                    this.allReadRegisterCommands.Enqueue(this.CurrentReadRegisterCommand);

                     this.canSendNextRequestCommandBytes = false;
                }
            }
            else
            {
                if (SerialPortNotResponseGiveUpValue < ++this.serialPortNotResponseCount)
                {
                    //如果在modbus没有响应，超过SerialPortNotResponseGiveUpValue指定
                    //的次数，就可以发生下一个请求。
                    this.canSendNextRequestCommandBytes = true;
                }

                System.Threading.Thread.Sleep(2000);
            }
        }

        #endregion

        #region 开启、停止ModbusService

        public void Start()
        {
            this.startRunModbusService = true;
            if (this.readAndWriteMobusThread != null)
            {
                ThreadState threadState = this.readAndWriteMobusThread.ThreadState;

                //TODO:修改串口和modubRTU时线程就变成Stopped状态
                //TODO:目前使用该方法（InitializeReadAndWriteMobusThread），
                //TODO:希望以后用更好的改进方法
                if (threadState == ThreadState.Stopped)
                {
                    InitializeReadAndWriteMobusThread();
                }
            }
            
        }

        public void Stop()
        {
            this.startRunModbusService = false;
        }

        #endregion

        #region 初始化数据相关信息

        public void InitializeData()
        {
            this.allDataPoints = dataPointRepository.FindAll().ToList();
        }

        #endregion

        #region 初始化所有"读寄存器"的命令

        public void InitializeAllReadRegisterCommands(List<DataPoint> dataPoints)
        {
            if (null == dataPoints)
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

                    //请求唯一性，用guid标示唯一性
                    List<byte> guidBytes = Guid.NewGuid().ToByteArray().ToList();
                    requestCommadByte.AddRange(guidBytes);

                    readRegisterCommand.ReadCommand = requestCommadByte;

                    //把所有的读命令保存在内存中，换取速度
                    this.allReadRegisterCommands.Enqueue(readRegisterCommand);
                }
            }
        }

        #endregion

        #region 串口初始化、获取

        public SerialPort GetCurrentSerialPort()
        {
            return this.modbusRtu.SerialPort;
        }

        public bool OpenCurrentSerialPort()
        {
            return this.modbusRtu.OpenCurrentSerialPort();
        }

        public bool CloseCurrentSerialPort()
        {
            return this.modbusRtu.CloseCurrentSerialPort();
        }

        private bool InitializeModbusRtuSerialPortFormConfigFile()
        {
            bool success = true;
            try
            {
                Stop();
                SerialPort serialPortFromConfigFile 
                    = SerialPorConfigerHelper.GetSerialPortFormConfigFile();
                InitializeModbusRtu(serialPortFromConfigFile);
            }
            catch (Exception)
            {
                success = false;
            }
            Start();
            return success;
        }

        public bool SetModbusRtuSerialPort(SerialPort serialPort)
        {
            bool success = true;
            try
            {
                if (this.modbusRtu.TryOpenSerialPort(serialPort))
                {
                    Stop();
                    InitializeModbusRtu(serialPort);
                    SerialPorConfigerHelper.SaveSerialPortToConfigFile(serialPort);
                    Start();
                }
                else
                {
                    success = false;
                }
            }
            catch (Exception)
            {
                success = false;
            }

            Start();

            return success; 
        }

        #endregion

        #region ModbusRTU 初始化

        private void InitializeModbusRtu(SerialPort serialPort)
        {
            this.modbusRtu = new ModbusRTU(serialPort);
            this.modbusRtu.OpenCurrentSerialPort();

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
            
            this.canSendNextRequestCommandBytes = true;

            if (null != recevideData)
            {
                this.currentRecivedByteData = recevideData;

                DataAnalyzeMode dataAnalyzeMode = ModbusConfigService.GetCurrentDataAnalyzeMode();
                AnalyzeRecivedDataReponse reponse =
                    RecivedDataAnalyzer.AnalyzeRecivedData(dataAnalyzeMode, this.currentRequestByteData, this.currentRecivedByteData);

                if (reponse.ModbusReponseSuccess && reponse.AnalyzeRecivedDataSuccess)
                {
                    ProcessRegisterValue(reponse.Registers, this.allDataPoints);
                }

                //if (reponse.DataPointType == DataPointType.WriteAndReadByFunNum01
                //    || reponse.DataPointType == DataPointType.WriteAndReadByFunNum03)
                //{
                //    //TODO:分析写寄存器接受侦
                //}

            }
        }

        #endregion

        #region 添加"写寄存器"的命令

        public void AddWriteRegisterCommands(List<DataPoint> dataPoints)
        {
            if (null == dataPoints || dataPoints.Count < 1)
            {
                return;
            }

            DataAnalyzeMode dataAnalyzeMode = ModbusConfigService.GetCurrentDataAnalyzeMode();

            List<List<DataPoint>> dataPointsGroupedForWrite 
                = DataPointGrouper.GroupingDataPointsForWriteRegister(dataPoints);

            foreach (var dataPointsGroup in dataPointsGroupedForWrite)
            {
                List<List<byte>> requestCommadBytes =
                    RequestCommandByteStreamCreater.CreateRequestCommandByteStreamForWriteRegisterBy(dataAnalyzeMode, dataPointsGroup);

                foreach (var requestCommadByte in requestCommadBytes)
                {
                    WriteRegisterCommand writeRegisterCommand = new WriteRegisterCommand();

                    //请求唯一性，用guid标示唯一性
                    List<byte> guidBytes = Guid.NewGuid().ToByteArray().ToList();
                    requestCommadByte.AddRange(guidBytes);
                    writeRegisterCommand.WriteCommand = requestCommadByte;

                    //TODO:删除，测试用的
                    //List<byte> textBytes = new List<byte>();

                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x10);
                    //textBytes.Add((byte)0x00);
                    //textBytes.Add((byte)0x72);
                    //textBytes.Add((byte)0x04);
                    //textBytes.Add((byte)0x01);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x03);
                    //textBytes.Add((byte)0x04);
                    //textBytes.Add((byte)0x09);
                    //textBytes.Add((byte)0x09);

                    //GUID
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);

                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);
                    //textBytes.Add((byte)0x02);

                    //writeRegisterCommand.WriteCommand = textBytes;

                    this.allWriterRegisterCommands.Enqueue(writeRegisterCommand);
                }
            }

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

        public GetDataPointRealTimeDataResponse GetDataPointRealTimeData(GetDataPointRealTimeDataRequest request)
        {
            GetDataPointRealTimeDataResponse response = new GetDataPointRealTimeDataResponse();

            DataPoint dataPoint = this.allDataPoints.Find(p => p.Id == request.DataPointId);
            if (null != dataPoint)
            {
                DataPointRealTimeDataView dataPointRealTimeDataView = new DataPointRealTimeDataView();
                dataPointRealTimeDataView.DataPointId = dataPoint.Id;
                dataPointRealTimeDataView.DataPointRealTimeValue = dataPoint.RealTimeValue = 1;

                response.DataPiontRealTimeData = dataPointRealTimeDataView;
            }
            else
            {
                response.ResponseSucceed = false;
            }

            return response;
        }

        public GetAllDataPointsRealTimeDataResponse GetAllDataPointsRealTimeData()
        {
            GetAllDataPointsRealTimeDataResponse response = new GetAllDataPointsRealTimeDataResponse();

            foreach (var dataPoint in this.allDataPoints)
            {
                DataPointRealTimeDataView dataPointRealTimeDataView = new DataPointRealTimeDataView();
                dataPointRealTimeDataView.DataPointId = dataPoint.Id;

                //TODO:模拟数据；要删除模式代码
                //Random random = new Random();
                //= random.NextDouble()
                dataPointRealTimeDataView.DataPointRealTimeValue = dataPoint.RealTimeValue;

                response.AllDataPointsRealTimeData.Add(dataPointRealTimeDataView);
            }

            return response;
        }

        public void SetDataPointValue(SetDataPointValueRequest request)
        {
            List<DataPoint> dataPointToSetValues 
                = request.DataPointsToSetValue.ConverToDataPoints().ToList();

            foreach (var dataPointToSetValue in dataPointToSetValues)
            {
                DataPoint dataPoint = this.allDataPoints.Find(p => p.Number == dataPointToSetValue.Number);
                if (null != dataPoint)
                {
                    dataPoint.ValueToSet = dataPointToSetValue.ValueToSet;
                }
            }

            AddWriteRegisterCommands(dataPointToSetValues);
        }

        #endregion 
    }
}
