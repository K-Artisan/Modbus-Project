using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Xml;
using System.Timers;

using UpDataBase.RTWriteProxy;
using System.Diagnostics;

namespace ModbusServer
{
    /*---------------------------------------------------------------------------
     *CmeManager  --Communications Management Equipment Manager:通讯管理机管理器
     *
     * CmeManager与CMEquipment一一对应关系
     *--------------------------------------------------------------------------*/
    public class CmeManager :IDisposable
    {
        #region 字段
        CMEquipment _cmEquipment;  //通讯管理机
        internal CMEquipment CmEquipment
        {
            get { return _cmEquipment; }
            set { _cmEquipment = value; }
        }
        //internal CMEquipment CMEquipment
        //{
        //    get
        //    {
        //        throw new System.NotImplementedException();
        //    }
        //    set
        //    {
        //    }
        //}

        //ModbusSendDataStruct _sendData;    // 请求数据包 
        //internal ModbusSendDataStruct SendData
        //{
        //    get { return _sendData; }
        //    set { _sendData = value; }
        //}

        //ModbusRecvDataStruct _recData;    // 接收数据包
        //internal ModbusRecvDataStruct RecData
        //{
        //    get { return _recData; }
        //    set { _recData = value; }
        //}

        private System.Timers.Timer _timeSend = null;                                       //定时发送请求和接收读取寄存器的值
        public delegate void ReadCMEquipmentHandler(object obj, ReadCMEEventArgs newData);  //定义委托：CmeManager上传寄存器数据给的委托
        public event EventHandler<ReadCMEEventArgs> _OnReadCMEquipmentEvent = delegate { }; //定义事件：CmeManager上的寄存器数据变化的事件

        #region 内嵌类ReadCMEEventArgs
        /*--------------------------------------------------------------------------
         * ReadCMEEventArgs
         *-------------------------------------------------------------------------*/
        /// <summary>
        /// CmeManager作为发布类，Modbus作为订阅者类
        /// Modbus是发布类上传给订阅者类的数据
        /// </summary>
        public class ReadCMEEventArgs : System.EventArgs
        {
            private List<UpDataBase.RTWriteProxy.PointRTModel> _updata;      //ModbusUpDataBase需要上传的数据列表
            public List<UpDataBase.RTWriteProxy.PointRTModel> Updata
            {
                get { return _updata; }
                set { _updata = value; }
            }

            public ReadCMEEventArgs(List<UpDataBase.RTWriteProxy.PointRTModel> data)
            {
                _updata = data;
            }

        }//UpDataEventArgs
        #endregion

        #region 构造函数 释放资源
        public CmeManager(int equipId, string xmlPath)
        {
            _cmEquipment = new CMEquipment(equipId, xmlPath);
            SetEndResId();
            _timeSend = new System.Timers.Timer(); //定时发送请求和接收读取寄存器的值

        }

        /// <summary>
        /// 该方法实现了IDisposable的Dispose
        /// </summary>
        public void Dispose()
        {
            //调用实际执行的清理的、带参数的Dispose方法
            Dispose(true);
        }

        /// <summary>
        /// 调用该共用方法来代替Dispose；
        /// </summary>
        public void Close()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timeSend.Stop();
                _timeSend.Close();
            }

            GC.SuppressFinalize(this);
        }
        #endregion

        public void Start(bool autoRead)
        {
            if (autoRead)
            {
                _timeSend.Interval = 100;
                _timeSend.Enabled = true;
                _timeSend.Elapsed += StartReadCme;  //(object obj, EventArgs e)
            }
            else
            {
                ReadAndProcessResValueInTotal(_funNumAi, _typeAI, _startResIdAi, _endResIdAi);    //AI
                ReadAndProcessResValueInTotal(_funNumDi, _typeDI, _startResIdDi, _endResIdDi);    //DI
                ReadAndProcessResValueInTotal(_funNumAcc, _typACC, _startResIdAcc, _endResIdAcc); //ACC
            }
        }
        #endregion

        #region 读

        //AI
        byte _funNumAi = 3;
        UpDataBase.RTWriteProxy.MType _typeAI = UpDataBase.RTWriteProxy.MType.AI;
        int _startResIdAi = 0;
        int _endResIdAi = 0;

        //DI
        byte _funNumDi = 1;
        UpDataBase.RTWriteProxy.MType _typeDI = UpDataBase.RTWriteProxy.MType.DI;
        int _startResIdDi = 0;//8;
        int _endResIdDi = 0;

        //ACC
        byte _funNumAcc = 3;
        UpDataBase.RTWriteProxy.MType _typACC = UpDataBase.RTWriteProxy.MType.ACC;
        int _startResIdAcc = 6000;//6128;//6000;
        int _endResIdAcc = 0;


        private void SetEndResId()
        {
            _endResIdAi = ConvertXmlResIdToConmandResId(_cmEquipment.AiMaxResIdDataPoint.Type, _cmEquipment.XmlEndResAi);
            _endResIdDi = ConvertXmlResIdToConmandResId(_cmEquipment.DiMaxResIdDataPoint.Type, _cmEquipment.XmlEndResDi);
            _endResIdAcc = ConvertXmlResIdToConmandResId(_cmEquipment.AccMaxResIdDataPoint.Type, _cmEquipment.XmlEndResAcc);
        }


        private bool _sendOverFlag = false;         
        private readonly object _objReadOrWriteLock = new object();   //读、写寄存器锁

        //private bool _readingCmeFlag = true;     //读标识：正在读为ture

        //public bool ReadingCmeFlag
        //{
        //    get { return _readingCmeFlag; }
        //    set 
        //    {
        //        Debug.WriteLine(value.ToString() + "||" + DateTime.Now.ToString());
        //        _readingCmeFlag = value; 
        //    }
        //}
        //private bool _writingCmeFlag = false;    //写标识：正在写为true

        //public bool WritingCmeFlag
        //{
        //    get { return _writingCmeFlag; }
        //    set
        //    {
        //        Debug.WriteLine(value.ToString() + "||" + DateTime.Now.ToString());
        //        _writingCmeFlag = value; 
        //    }
        //}

        //---------------------------------------------- 读---------------------------------------------------------
        
        public void StartReadCme(object obj, EventArgs e)
        {
            if (!_sendOverFlag)
            {
                Debug.WriteLine("ReadCmeInTotal。。。{0}",DateTime.Now.TimeOfDay);
                _sendOverFlag = true;
                ReadAndProcessResValueInTotal(_funNumAi, _typeAI, _startResIdAi, _endResIdAi);    //AI
                ReadAndProcessResValueInTotal(_funNumDi, _typeDI, _startResIdDi, _endResIdDi);    //DI
                ReadAndProcessResValueInTotal(_funNumAcc, _typACC, _startResIdAcc, _endResIdAcc); //ACC
                _sendOverFlag = false;
            }
        }
        /// <summary>
        /// 本方法的前提是：已经假设配置的寄存器id：startResId到endResId都是连续的
        /// </summary>
        /// <param name="funNum"></param>
        /// <param name="type"></param>
        /// <param name="startResId"></param>
        /// <param name="endResId"></param>
        public void ReadAndProcessResValueInTotal(byte funNum, UpDataBase.RTWriteProxy.MType type, int startResId, int endResId)
        {
            /*-------------------------------------------------------------------------------------------------------------
             * 本方法的前提是：已经假设配置的寄存器id都是连续的
             * ------------------------------------------------------------------------------------------------------------*/
            List<ModbusSendDataStruct> sendDataList = null;   //分解请求包
            Dictionary<int, UInt16> temp = new Dictionary<int, UInt16>();

            //生产请求包（分包成多个）
            sendDataList = SplitSendDataPacket2(funNum, type, startResId, endResId);

            if (null != sendDataList)   //安全检索
            {
                for (int i = 0; i < sendDataList.Count; ++i)
                {

                    //if (!WritingCmeFlag)
                    //{
                    //    ReadingCmeFlag = true;
                    lock (_objReadOrWriteLock)
                    {
                        //一个请求包返回的有效（不表示寄存器值的不会返回）寄存器值
                        temp = ReadResValueFromCmeInCell(funNum, sendDataList[i].StartReg, sendDataList[i].RegCount);

                        if (temp != null)
                        {
                            //处理寄存器值
                            ProcessResValueInCell(temp, funNum, type, startResId, sendDataList[i].RegCount);
                        }
                    }
                    //    ReadingCmeFlag = false;
                    //}
                    //else
                    //{
                    //    ReadingCmeFlag = false;
                    //}
                }
            }
        }

        private void ProcessResValueInCell(Dictionary<int, UInt16> dicU16Data, byte funNum, UpDataBase.RTWriteProxy.MType type, int startResId, int countRes)
        {
            /*-------------------------------------------------------------------------------------
             *1.将寄存器值放入对应的DataPoint.PointValue；
             *2.找出DataPoint.PointValue发生改变的DataPoint并封装成UpDataBase.RTWriteProxy.PointRTModel统一放入一个列表changdataList中
             *3.上传第2步的changdataList列表中的数据
            -------------------------------------------------------------------------------------*/

            List<UpDataBase.RTWriteProxy.PointRTModel> listUploadData = null;

            //设置和提取数据发生改变的DataPiont（已经封装成PointRTModel结构）
            listUploadData = SetDataPointVauleInCell(dicU16Data, funNum, type, startResId, countRes);

            //上传数据
            if (null != listUploadData && listUploadData.Count > 0)
            {

                UploadPointRTModelListInCell(listUploadData);
            }
        }
        /// <summary>
        /// 1.设置_cmEquipment.DicResIdToDataPoint[currResId].PointValue
        /// 2.提取_cmEquipment.DicResIdToDataPoint[currResId].PointValue发生改变的DataPoint并封装成上传列表
        /// </summary>
        /// <param name="dicU16Data">《寄存器id，寄存器值》</param>
        /// <param name="startResId">起始寄存器</param>
        /// <param name="countRes">所读取的寄存器个数</param>
        /// <returns>上传数据列表</returns>
        private List<UpDataBase.RTWriteProxy.PointRTModel> SetDataPointVauleInCell(Dictionary<int, UInt16> dicU16Data, byte funNum, UpDataBase.RTWriteProxy.MType type, int startResId, int countRes)
        {
            List<UpDataBase.RTWriteProxy.PointRTModel> listUploadData = new List<UpDataBase.RTWriteProxy.PointRTModel>();  //上传数据列表
            int currResId;
            UInt16 currResValue;
            int currXmlResId;

            DataPoint curDataPoint;
            Type tmpType;
            double curPointval;
           
            foreach (KeyValuePair<int, UInt16> item in dicU16Data)  //寄存器值字典
            {
                currResId = item.Key;       //
                currResValue = item.Value;  //
                currXmlResId = ConvertConmandResIdToXmlResId(type, currResId); //currResId所对应的xml文件中的寄存器id

                if (_cmEquipment.DicResIdToDataPoint.ContainsKey(currXmlResId))  //存在DataPiont.ResId == item.Key的DataPiont
                {
                    //1.判断是类型：U16、S26、U32、S32、F32
                    //2.如果是U32、S32、F32，要判断dicU16Data[resId + 1]存在！
                    //3.如果DataPoint.PointValue改变，封装成PointRTModel类型添加到上传列表listUploadData

                    curDataPoint = _cmEquipment.DicResIdToDataPoint[currXmlResId]; //DataPoint.ResId == currResId 的DataPoint
                    tmpType = curDataPoint.DataType;         //类型
                    curPointval = curDataPoint.PointValue == null ? 0 : (double)curDataPoint.PointValue; //当前的值（读取新值前的值）

                    if (tmpType == typeof(UInt16) || tmpType == typeof(Int16) || tmpType == typeof(Boolean)) //DataPoint.DataType是U16\S16\Bit   
                    {
                        double newPointVal = UInt16ToDouble(tmpType, currResValue);  //将寄存器的值转化为DataPoint.PointValue

                        if (newPointVal != curPointval)   //DataPiont.DataPiontValue 改变了
                        {
                            _cmEquipment.DicResIdToDataPoint[currXmlResId].PointValue = newPointVal; //赋新值

                            //添加到上传数据列表
                            listUploadData.Add(new UpDataBase.RTWriteProxy.PointRTModel()
                                                        {
                                                            ID = new UpDataBase.RTWriteProxy.IDModel()
                                                            {
                                                                DevID = curDataPoint.DevId,
                                                                Type = curDataPoint.Type,
                                                                PointID = curDataPoint.PointId,
                                                            },
                                                            Value = newPointVal,
                                                            Time = DateTime.Now,
                                                        });
                        }
                    } //if（|| ||）
                    else if (tmpType == typeof(UInt32) || tmpType == typeof(Int32) || tmpType == typeof(Single))  //DataPoint.DataType是U32\S32\F32   
                    {
                        if (dicU16Data.ContainsKey(currResId + 1)) //检查寄存器号为(currResId + 1)存在
                        {
                            double newPointVal = UInt16ToDouble(tmpType, currResValue, dicU16Data[(currResId + 1)]);  //将寄存器的值转化为DataPoint.PointValue
                            if (newPointVal != curPointval)   //DataPiont.DataPiontValue 改变了
                            {
                                _cmEquipment.DicResIdToDataPoint[currXmlResId].PointValue = newPointVal; //赋新值
                                //添加到上传数据列表
                                listUploadData.Add(new UpDataBase.RTWriteProxy.PointRTModel()
                                                            {
                                                                ID = new UpDataBase.RTWriteProxy.IDModel()
                                                                {
                                                                    DevID = curDataPoint.DevId,
                                                                    Type = curDataPoint.Type,
                                                                    PointID = curDataPoint.PointId,
                                                                },
                                                                Value = newPointVal,
                                                                Time = DateTime.Now,
                                                            });
                            }
                        }
                    }//else if

                }//if
            } //foreach

            return listUploadData;
        }
        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="listUploadData">所要上传的数据</param>
        private void UploadPointRTModelListInCell(List<UpDataBase.RTWriteProxy.PointRTModel> listUploadData)
        {
            //上传：值发生改变的并封装成PointRTModel类型的DataPoint
            if (null != _OnReadCMEquipmentEvent) //确认有方法可以执行
            {
                //封装ModbusUpDataBase需要上传的数据列表
                ReadCMEEventArgs upData = new ReadCMEEventArgs(listUploadData);

                //触发事件，并上传数据,
                //foreach是为了处理来自订阅者的异常
                foreach (EventHandler<ReadCMEEventArgs> hanlder in _OnReadCMEquipmentEvent.GetInvocationList())
                {
                    //_OnReadCMEquipmentEvent(this, upData);
                    hanlder(this, upData);
                }
            }
        }
        /// <summary>
        /// 从通讯管理机读取数据,并根据功能码转换成 Dictionary<int, UInt16>
        /// </summary>
        /// <param name="funNum">功能码</param>
        /// <param name="startR">起始寄存器id，从0开始,注意：当时功能码3时，注意与40001的转换</param>
        /// <param name="countR">所读取的寄存器的个数 ,取值范围是：当时功能码01时：0-1000；
        ///                                                      当时功能码03时：0-120。
        ///</param>
        /// <returns>字典《寄存器id，寄存器值（U16）》</returns>
        private Dictionary<int, UInt16> ReadResValueFromCmeInCell(byte funNum, int startR, int countR) 
        {
            TcpClient tcpClient = null;  
            NetworkStream netIOStream = null; 
            ModbusSendDataStruct sendData = null;
            ModbusRecvDataStruct recData = null;
            Dictionary<int, UInt16> dicU16Data = null;
            tcpClient = new TcpClient();

            try
            {
                try   
                {
                    //连接通讯管理机
                    int port = _cmEquipment.EquipPort;
                    IPAddress ipAddr = IPAddress.Parse(_cmEquipment.EquipIP);
                    tcpClient = new TcpClient();
                    tcpClient.Connect(ipAddr, port);

                    //tcpClient.Connect("192.168.0.83", 502);
                    // tcpClient.Connect("192.168.0.240", 502);

                    Debug.WriteLine("Read-->Connect...{0}", DateTime.Now.TimeOfDay);
                }
                catch (Exception e1)
                {
                    Debug.WriteLine("Read-->Connect...error:\n" + e1.Message);
                }

                try
                {
                    //获取网络传输流
                    netIOStream = tcpClient.GetStream();
                    netIOStream.ReadTimeout = 500;
                    netIOStream.WriteTimeout = 500;
                }
                catch (Exception e2)
                {
                    Debug.WriteLine("Read-->tcpClient error:{0}" + e2.Message, DateTime.Now);
                }

                if (null != netIOStream)
                {
                    //发送请求包
                    if (netIOStream.CanWrite)
                    {
                        try
                        {
                            sendData = new ModbusSendDataStruct();
                            sendData.FunNum = funNum;
                            sendData.StartReg = (ushort)startR;
                            sendData.RegCount = (ushort)countR;

                            byte[] tmp = sendData.ToArray();
                            netIOStream.Write(tmp, 0, tmp.Length);

                        }
                        catch (Exception ew)
                        {
                            Debug.WriteLine("Read-->tcpClient error:{0}" + ew.Message, DateTime.Now);
                        }
                    }

                    if (netIOStream.CanRead)
                    {
                        byte[] recvdata = new byte[1026];

                        try
                        {
                            int recvLen = netIOStream.Read(recvdata, 0, recvdata.Length);
                        }
                        catch (Exception ex3)
                        {
                            Debug.WriteLine("Read-->tcpClient error:{0}" + ex3.Message, DateTime.Now);
                        }

                        //recvdata[7]的值不等于funNum，
                        //说明请求命令包出问题比如：请求了不存在的寄存器号。
                        byte recvfunNum = recvdata[7];  //提取功能码 
                        if (recvfunNum == funNum)  //请求时功能码与从返回数据中提取的功能码一致，表明请求命令正确
                        {
                            recData = new ModbusRecvDataStruct();
                            dicU16Data = recData.RecvDataToUInt16WithFunNum(recvdata, startR, countR);
                        }
                        else  //返回了错误的数据
                        {
                            dicU16Data = new Dictionary<int, UInt16>();  //空值
                        }
                    } //if (netIOStream.CanRead)
                }
            }
            finally
            {
                //netIOStream.Close();
                //tcpClient.Close();
                CloseNetIOStream(netIOStream);
                CloseTcpClient(tcpClient);

                tcpClient = null;
                netIOStream = null;
                sendData = null;
                recData = null;
            }
            
            return dicU16Data;
        }
        #endregion 读

        #region 写
        /*---------------------------------------------- 写------------------------------------------------------------*/
        /// <summary>
        /// 向通讯管理机写数据
        /// </summary>
        /// <param name="funNum"></param>
        /// <param name="startR"></param>
        /// <param name="countR"></param>
        /// <returns>返回通讯管理机的响应数据</returns>
        public double WriteDataToCmeAndGetReturnValInCell(DataPoint dataPiont)
        {
            double returVal = -1;  //响应的数据（修改后的寄存器值），double类型

            Dictionary<int, UInt16> dicU16Data = new Dictionary<int, UInt16>(); //响应的数据（修改后的寄存器值），存储在字典中

            UpDataBase.RTWriteProxy.MType type = dataPiont.Type;
            byte funNum = GetFunNumByType(type);                 //功能码
            int startResId = dataPiont.ResId;
            Type dataType = dataPiont.DataType;
            int resCount = GetReadResCountByDataType(dataType);
            double pointVal = dataPiont.PointValue == null ? 0 : (double)dataPiont.PointValue;

            int endResId = startResId + resCount - 1;

            int cmdStartResId = ConvertXmlResIdToConmandResId(type, startResId); 
            int cmdEndResId = cmdStartResId + resCount - 1;                
            
            //检测所要写的起始和结束寄存器id是否合法
            int statResIdRright = VerifyLegitimacyOfStartResId(funNum, type, cmdStartResId, _cmEquipment.DicResIdToDataPoint);
            int endResIdRright = VerifyLegitimacyOfEndResId(funNum, type, cmdEndResId, _cmEquipment.DicResIdToDataPoint);

            if (statResIdRright == 1 && endResIdRright == 1)//起始和结束寄存器id合法
            {
                lock (_objReadOrWriteLock)
                {
                    dicU16Data = WriteResValueToCmeInCell(funNum, cmdStartResId, resCount, pointVal);
                }
                returVal = ProcessWriteCmeReturnDataInCell(dicU16Data, type);

                return returVal;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 写寄存器
        /// </summary>
        /// <param name="funNum">功能码</param>
        /// <param name="startR">起始寄存器</param>
        /// <param name="countR">寄存器个数</param>
        /// <param name="pointVal">所要写入的值
        /// 05功能码（DO）开：pointVal=1；关pointVal=0；
        /// </param>
        /// <returns>返回写寄存器后的寄存器值</returns>
        private Dictionary<int, UInt16> WriteResValueToCmeInCell(byte funNum, int startR, int countR, double pointVal)
        {
            TcpClient tcpClient = null;
            NetworkStream netIOStream = null;
            ModbusSendDataStruct sendData = null;
            ModbusRecvDataStruct recData = null;

            Dictionary<int, UInt16> dicU16Data = null;

            tcpClient = new TcpClient();


            try
            {
                //连接通讯管理机
                try
                {
                    int port = _cmEquipment.EquipPort;
                    IPAddress ipAddr = IPAddress.Parse(_cmEquipment.EquipIP);
                    tcpClient = new TcpClient();
                    tcpClient.Connect(ipAddr, port);

                    //tcpClient.Connect("192.168.0.83", 502);
                    // tcpClient.Connect("192.168.0.240", 502);

                    Debug.WriteLine("Write-->Connect...---------------------------{0}", DateTime.Now.TimeOfDay);
                }
                catch (Exception e1)
                {
                    Debug.WriteLine("Write-->Connect......------------------------{error:" + e1.Message);
                    //CloseTcpClient(tcpClient);
                }

                //获取网络传输流
                try
                {
                    netIOStream = tcpClient.GetStream();
                    netIOStream.ReadTimeout = 500;
                    netIOStream.WriteTimeout = 500;
                }
                catch (Exception e2)
                {
                    Debug.WriteLine("Write-->tcpClient error...-------------------{:" + e2.Message);
                    //CloseNetIOStream(netIOStream);
                }

                if (null != netIOStream)
                {
                    //发送请求包
                    if (netIOStream.CanWrite)
                    {
                        sendData = new ModbusSendDataStruct();
                        sendData.FunNum = funNum;
                        sendData.StartReg = (ushort)startR;
                        sendData.RegCount = (ushort)pointVal;

                        byte[] tmp = sendData.ToArray();
                        netIOStream.Write(tmp, 0, tmp.Length);
                    }

                    if (netIOStream.CanRead)
                    {
                        byte[] recvdata = new byte[1026];

                        try
                        {
                            int recvLen = netIOStream.Read(recvdata, 0, recvdata.Length);
                        }
                        catch (Exception ex3)
                        {
                            Debug.WriteLine("Write-->tcpClient...------------------{ error:" + ex3.Message);
                        }


                        //recvdata[7]的值不是1，说明请求命令包出问题比如：请求了不存在的寄存器号。
                        byte recvfunNum = recvdata[7];  //提取功能码 
                        if (recvfunNum == funNum)  //请求时输入的功能码与从返回数据中提取的功能码一致，表明请求命令正确
                        {
                            recData = new ModbusRecvDataStruct();
                            dicU16Data = recData.RecvDataToUInt16WithFunNum(recvdata, startR, countR);
                        }
                        else  //返回了错误的数据
                        {
                            dicU16Data = new Dictionary<int, UInt16>();  //空值
                        }
                    }
                }
            }
            finally
            {
                //netIOStream.Close();
                //tcpClient.Close();
                CloseNetIOStream(netIOStream);
                CloseTcpClient(tcpClient);

                 tcpClient = null;
                 netIOStream = null;
                 sendData = null;
                 recData = null;
            }
            
            return dicU16Data;
        }

        /// <summary>
        /// 解析写通讯管理机寄存器后通讯管理机返回的数据（字典）成double类型
        /// </summary>
        /// <param name="dicU16Data">通讯管理机返回的数据（字典）</param>
        /// <param name="type">写的类型：AO、DO</param>
        /// <returns>解析后的double</returns>
        private double ProcessWriteCmeReturnDataInCell(Dictionary<int, UInt16> dicU16Data, UpDataBase.RTWriteProxy.MType type)
        {
            double returnPointVal = -1;
            int currResId;
            UInt16 currResValue ;
            int currXmlResId;

            foreach (KeyValuePair<int, UInt16> item in dicU16Data)  //寄存器值字典
            {
                currResId = item.Key;       //
                currResValue = item.Value;  //
                currXmlResId = ConvertConmandResIdToXmlResId(type, currResId); //currResId所对应的xml文件中的寄存器id
                DataPoint curDataPoint = null;
                Type tmpType = null;

                if (_cmEquipment.DicResIdToDataPoint.ContainsKey(currXmlResId))  //检查DataPiont.ResId == item.Key的DataPiont存在于xml文件中
                {
                    //1.判断是类型：U16、S26、U32、S32、F32
                    //2.如果是U32、S32、F32，要判断dicU16Data[resId + 1]存在！
                    //3.求DataPoint.PointValue

                    curDataPoint = _cmEquipment.DicResIdToDataPoint[currXmlResId]; //DataPoint.ResId == currResId 的DataPoint
                    tmpType = curDataPoint.DataType;         //类型

                    if (tmpType == typeof(UInt16) || tmpType == typeof(Int16) || tmpType == typeof(Boolean)) //DataPoint.DataType是U16\S16\Bit   
                    {
                        returnPointVal = UInt16ToDouble(tmpType, currResValue);  //将寄存器的值转化为DataPoint.PointValue

                    } //if（|| ||）
                    else if (tmpType == typeof(UInt32) || tmpType == typeof(Int32) || tmpType == typeof(Single))  //DataPoint.DataType是U32\S32\F32   
                    {
                        if (dicU16Data.ContainsKey(currResId + 1)) //检查通讯管理机返回的数据中存在寄存器号为(currResId + 1)的值
                        {
                            returnPointVal = UInt16ToDouble(tmpType, currResValue, dicU16Data[(currResId + 1)]);  //将寄存器的值转化为DataPoint.PointValue
                        }
                    }//else if

                }//if
            } //foreach


            return returnPointVal;
        }
        #endregion

        #region 读、写公用
        //---------------------------------------------- 读、写公用------------------------------------------
       
        private const int  RESNUMINONESENDPACKET1000 = 1000;   //每个请求包要求读取的寄存器个数 
        private const int  RESNUMINONESENDPACKET120 = 120;     //每个请求包要求读取的寄存器个数
        /// <summary>
        /// 根据起始地址id和结束寄存器id，产生相应数量的发送包（ModbusSendDataStruct）
        /// </summary>
        /// <param name="funNum">功能码
        /// （不同的功能码，可一次读取寄存器的最大个数不同，
        /// 01功能码建议书1200个寄存器（每个寄存器占1个比特），考虑到01功能码对应DI，一个DI最多占1个bit，所以diDataPointList以1200个为一个包分包
        /// 03功能码建议是120个寄存器（每个寄存器占2个字节），考虑到03功能码对应AI，一个AI最多占120个寄存器（4个字节），所以diDataPointList以60（即：120/4个）围一个包分包
        /// </param>
        /// <param name="startR">起始寄存器id</param>
        /// <param name="endResId">结束寄存器id</param> 
        private List<ModbusSendDataStruct> SplitSendDataPacket(byte funNum, UpDataBase.RTWriteProxy.MType type, int startResId, int endResId)
        {
            /*-------------------------------------------------------------------------------------------------------------
             * 1.本方法的前提是：【1】已经假设AI DI ACC 对应配置的寄存器id都是连续的
             *                  （因为请求了0-119寄存器id的数据，当0-119中有个寄存器id不存在，将返回错误的响应数据）
             *                  【2】eachLen 和 eachLen+1 （即：第n个请求包最后一个寄存器和第n+1个请求包第一个请求包）刚好不表示同一个DataPiont
             * 2.本方法的算法是：保证endResId或者（endResId+1）存在
             *                  (1).保证endResId及封包目标：
             *                  (2).保证（endResId+1）及封包目标：
             *                     endResId 的情况是DataPiont类型是Bit、U16、S16
             *                    （endResId+1）的情况是DataPiont类型是U32、S36、F32
             * 
             * ------------------------------------------------------------------------------------------------------------*/


            List<ModbusSendDataStruct> sendDataList = new List<ModbusSendDataStruct>();  //各个请求包

            //检测所要读取的起始和结束寄存器id是否合法
            int statResIdRright = VerifyLegitimacyOfStartResId(funNum, type, startResId, _cmEquipment.DicResIdToDataPoint);
            int endResIdRright = VerifyLegitimacyOfEndResId(funNum, type, endResId, _cmEquipment.DicResIdToDataPoint);

            if (statResIdRright == 1 && endResIdRright == 1)//起始和结束寄存器id合法
            {
                int eachLen = RESNUMINONESENDPACKET120;    //每个请求包可读取寄存器的最大个数
                int resCount = (endResId - startResId) + 1;   //从startResId 到 endResId 寄存器的个数
                int countPacket = 0;  //请求包个数

                //if (resCount < 0)
                //    return null;

                switch (funNum)
                {
                    case 1:   //01功能码
                        eachLen = RESNUMINONESENDPACKET1000;

                        break;
                    case 3:    //03功能码
                        eachLen = RESNUMINONESENDPACKET120;

                        break;
                    default:
                        eachLen = RESNUMINONESENDPACKET120;
                        break;
                }

                countPacket = (int)Math.Ceiling((double)resCount / (double)eachLen); //需要分成包的个数

                if (1 == countPacket)
                {
                    sendDataList.Add(new ModbusSendDataStruct()
                                                                {
                                                                    FunNum = funNum,
                                                                    StartReg = (ushort)startResId,
                                                                    RegCount = (ushort)resCount,
                                                                }
                                      );

                }//(1 == countPacket)
                else if (1 < countPacket)   //需要分成包的个数>=2
                {
                    for (int i = 0; i <= (countPacket - 1); ++i)
                    {
                        //注意：寄存器id小先放入sendDataList列表中

                        if (i < countPacket - 1)    //第一个包到倒数第二个包
                        {
                            sendDataList.Add(new ModbusSendDataStruct()
                                                                        {
                                                                            FunNum = funNum,
                                                                            StartReg = (ushort)(startResId + eachLen * i),
                                                                            RegCount = (ushort)eachLen,
                                                                        }
                                            );

                        }
                        else if (i == countPacket - 1)  //倒数第一个包
                        {
                            int retainRegCount = resCount % eachLen;  //最后一个包的寄存器个数；
                            sendDataList.Add(new ModbusSendDataStruct()
                                                                        {
                                                                            FunNum = funNum,
                                                                            StartReg = (ushort)(startResId + eachLen * i),
                                                                            RegCount = (ushort)(retainRegCount),
                                                                        }
                                              );
                        }
                    } //for
                }//else if (1 < countPacket)   //需要分成包的个数>=2
            }

            return sendDataList;
        }
        private List<ModbusSendDataStruct> SplitSendDataPacket2(byte funNum, UpDataBase.RTWriteProxy.MType type, int startResId, int endResId)
        {
            /*-------------------------------------------------------------------------------------------------------------
             * 1.本方法的前提是：【1】已经假设AI DI ACC 对应配置的寄存器id都是连续的
             *                  （因为请求了0-119寄存器id的数据，当0-119中有个寄存器id不存在，将返回错误的响应数据）
             *                  
             * 2.本方法的算法是：保证endResId或者（endResId+1）存在
             *                  (1).保证endResId及封包目标：
             *                  (2).保证（endResId+1）及封包目标：
             *                     endResId 的情况是DataPiont类型是Bit、U16、S16
             *                    （endResId+1）的情况是DataPiont类型是U32、S36、F32
             *                 (3).第n个请求包最后一个寄存器和第n+1个请求包第一个请求包刚好表示同一个DataPiont的处理方式是:
             *                      第n个请求包往后多读一个寄存器，第n+1个请求包往前多读一个寄存器
             * ------------------------------------------------------------------------------------------------------------*/


            List<ModbusSendDataStruct> sendDataList = new List<ModbusSendDataStruct>();  //各个请求包

            //检测所要读取的起始和结束寄存器id是否合法
            int statResIdRright = VerifyLegitimacyOfStartResId(funNum, type, startResId, _cmEquipment.DicResIdToDataPoint);
            int endResIdRright = VerifyLegitimacyOfEndResId(funNum, type, endResId, _cmEquipment.DicResIdToDataPoint);

            if (statResIdRright == 1&& endResIdRright ==1 )//起始和结束寄存器id合法
            {
                int eachLen = RESNUMINONESENDPACKET120;       //每个请求包可读取寄存器的最大个数
                int resCount = (endResId - startResId) + 1;   //从startResId 到 endResId 寄存器的个数
                int countPacket = 0;  //请求包个数

                if (resCount < 0)
                    return null;

                switch (funNum)
                {
                    case 1:   //01功能码
                        eachLen = RESNUMINONESENDPACKET1000;

                        break;
                    case 3:    //03功能码
                        eachLen = RESNUMINONESENDPACKET120;

                        break;
                    default:
                        eachLen = RESNUMINONESENDPACKET120;
                        break;
                }

                countPacket = (int)Math.Ceiling((double)resCount / (double)eachLen); //需要分成包的个数

                if (1 == countPacket)
                {
                    sendDataList.Add(new ModbusSendDataStruct()
                    {
                        FunNum = funNum,
                        StartReg = (ushort)startResId,
                        RegCount = (ushort)resCount,
                    }
                                      );

                }//(1 == countPacket)
                else if (1 < countPacket)   //需要分成包的个数>=2
                {
                    int  tmpStarResId =0;
                    int  tmpResCount =0;
                    int tmpEndResId = 0;
                    
                    for (int i = 0; i <= (countPacket - 1); ++i)
                    {
                        //注意：寄存器id小先放入sendDataList列表中 
                        if (i < countPacket - 1)    //第一个包到倒数第二个包
                        {
                            //本应如此
                            tmpStarResId = startResId + eachLen * i;
                            tmpResCount = eachLen;
                            tmpEndResId = tmpStarResId + tmpResCount - 1;

                            //以防万一
                            //起始寄存器是否需要往前再取一个
                            if (1 != VerifyLegitimacyOfStartResId(funNum, type, tmpStarResId, _cmEquipment.DicResIdToDataPoint))  
                            {
                                //请求包的起始寄存器tmpStarResId不存在于 _cmEquipment.DicResIdToDataPoint中
                                //说明该寄存器id是和它的前一个寄存器共同表示一个DataPoint
                                //如果以该寄存器id作为请求包的起始id,将返回错误的数据，所以：
                                //将该寄存器id的前一个寄存器id作为请求包的起始id

                                tmpStarResId -= 1;   //将该寄存器id的前一个寄存器id作为该请求包的寄存器起始id
                                tmpResCount += 1;    //要读取的寄存器个数相应加1；
                            }

                            //结束寄存器是否需要往后加一个
                            if (0 == VerifyLegitimacyOfEndResId(funNum, type, tmpEndResId, _cmEquipment.DicResIdToDataPoint))
                            {
                                //每个寄存器请求包的结束必定寄存器不存在于 _cmEquipment.DicResIdToDataPoint中
                                //说明该寄存器id有两种可能：
                                //（1）和它的前一个寄存器共同表示一个DataPoint
                                //（2）压根不存在
                                //如果以压根不存在的寄存器id作为请求包的结束id,将返回错误的数据，所以：
                                //如果是“（1）和它的前一个寄存器共同表示一个DataPoint”，该请求包往后追加一个寄存器

                                tmpEndResId += 1;    //将该寄存器id的下一个寄存器id作为该请求包的结束寄存器id
                                tmpResCount += 1;    //要读取的寄存器个数相应加1；

                            }

                            sendDataList.Add(new ModbusSendDataStruct()
                            {
                                FunNum = funNum,
                                StartReg =  (ushort)tmpStarResId,//(ushort)(startResId + eachLen * i),
                                RegCount = (ushort)tmpResCount,
                            }
                                            );

                        }
                        else if (i == countPacket - 1)  //倒数第一个包
                        {
                            int retainRegCount = resCount % eachLen;  //最后一个包的寄存器个数；

                            //本应如此
                            tmpStarResId = startResId + eachLen * i;
                            tmpResCount = retainRegCount;

                            //以防万一
                            //起始寄存器是否需要往前再取一个
                            if (1 != VerifyLegitimacyOfStartResId(funNum, type, tmpStarResId, _cmEquipment.DicResIdToDataPoint))
                            {
                                //请求包的起始寄存器tmpStarResId不存在于 _cmEquipment.DicResIdToDataPoint中
                                //说明该寄存器id是和它的前一个寄存器共同表示一个DataPoint
                                //如果以该寄存器id作为请求包的起始id,将返回错误的数据，所以：
                                //将该寄存器id的前一个寄存器id作为请求包的起始id

                                tmpStarResId -= 1;   //将该寄存器id的前一个寄存器id作为该请求包的寄存器起始id
                                tmpResCount += 1;    //要读取的寄存器个数相应的加1；
                            }

                            sendDataList.Add(new ModbusSendDataStruct()
                            {
                                FunNum = funNum,
                                StartReg = (ushort)tmpStarResId,   //(startResId + eachLen * i),
                                RegCount = (ushort)tmpResCount,    //(retainRegCount),
                            }
                                              );
                        }
                    } //for
                }//else if (1 < countPacket)   //需要分成包的个数>=2
            }

            return sendDataList;
        }

        /// <summary>
        /// 检查起始寄存器的设置是否正确
        /// </summary>
        /// <param name="funNum"></param>
        /// <param name="type"></param>
        /// <param name="startResId">命令中的startResId</param>
        /// <param name="dicResIdToDataPoint"></param>
        /// <returns>1：起始寄存器设置正确
        ///          否则：起始寄存器设置错误       
        /// </returns>
        private int VerifyLegitimacyOfStartResId(byte funNum, UpDataBase.RTWriteProxy.MType type, int startResId, Dictionary<int, DataPoint> dicResIdToDataPoint)
        {
           /*该算法的前提是：
            * dicResIdToDataPoint 中存在的寄存器id都是每个DataPoint的起始寄存器id，而dicResIdToDataPoint中永远不存在任何DataPoint的结束寄存器id，
            * 要检查每个DataPoint的结束寄存器id还要结合每个DataPoint的Type字段（U16?S16?U32?S32?F32?BOOL）
            */

            int xmlStartResId = ConvertConmandResIdToXmlResId(type, startResId);

            //根据功能码funNum判断startResId的合法性！！？？？
            //场景：AO和ACC都对应03功能码：如果读AO而请求的寄存器Id不对应AO而是对应ACC，就将ACC的值当作AO的值了。
            //。。。。。。

            if (dicResIdToDataPoint.ContainsKey(xmlStartResId))
            {
                return 1;
            }

            return 0;
        }
       /// <summary>
        /// 检查结束寄存器的设置是否正确
       /// </summary>
       /// <param name="funNum"></param>
       /// <param name="type"></param>
       /// <param name="endResId"></param>
       /// <param name="dicResIdToDataPoint"></param>
        /// <returns>1：表示结束寄存器的设置正确
        ///          否则：结束寄存器的设置错误--其中：0表示设置的结束寄存器的下一个寄存器（如果存在的话）也应该配置
        ///                                          -1表示设置的结束寄存器压根就不存在
        /// </returns>
        private int VerifyLegitimacyOfEndResId(byte funNum, UpDataBase.RTWriteProxy.MType type, int endResId, Dictionary<int, DataPoint> dicResIdToDataPoint)
        {
            /*该算法的前提是：
             * dicResIdToDataPoint 中存在的寄存器id都是每个DataPoint的起始寄存器id，而永远不存在DataPoint的结束寄存器id，
             * 要检查每个DataPoint的结束寄存器id还要结合每个DataPoint的Type字段（U16?S16?U32?S32?F32?BOOL）
             */

            int xmlEndResId = ConvertConmandResIdToXmlResId(type, endResId);

            switch (funNum)
            {
                case 1:  //01功能码 ：  typeof(Boolean)  DI
                case 5:  //05功能码     typeof(Boolean)  DO
                    if (dicResIdToDataPoint != null && dicResIdToDataPoint.ContainsKey(xmlEndResId))
                    {
                        return 1;
                    }
                    break;

                case 3: //03功能码   AI\ACC
                case 6: //06功能码   AO

                    if (dicResIdToDataPoint != null &&  dicResIdToDataPoint.ContainsKey(xmlEndResId)) //存在xmlEndResId（是某个Datapoint的起始寄存器），往下追溯
                    {

                        Type dataType = dicResIdToDataPoint[xmlEndResId].DataType;
                        if (dataType == typeof(UInt16) ||
                            dataType == typeof(Int16))
                        {
                            return 1;   //结束寄存器配置正确：是某个Datapoint的起始寄存器，也是该Datapoint的结束寄存器，
                        }
                        else
                        {
                            Debug.WriteLine("往下追溯,结束寄存器id={0}-->{1}设置错误，该寄存器的下一个寄存器没有在命令中", endResId, xmlEndResId);
                            return 0;   //结束寄存器配置错误：是某个Datapoint的起始寄存器，但是该Datapoint的结束寄存器没有取到
                        }
                    }
                    else if (dicResIdToDataPoint != null && !dicResIdToDataPoint.ContainsKey(xmlEndResId)) //不存在xmlEndResId（不是某个Datapoint的起始寄存器）,往上追溯
                    {
                        if (dicResIdToDataPoint.ContainsKey(xmlEndResId - 1))
                        {
                            Type dataType = dicResIdToDataPoint[xmlEndResId - 1].DataType;

                            if (dataType == typeof(UInt32) || dataType == typeof(Int32) || dataType == typeof(Single))
                            {
                                return 1;  //结束寄存器配置正确：不是某个Datapoint的起始寄存器，却是某个Datapoint的结束寄存器，
                            }
                        }
                        else
                        {
                            Debug.WriteLine("往上追溯:结束寄存器id={0}-->{1}设置错误，该寄存器号压根就不存在！", endResId, xmlEndResId);
                            return -1;  //结束寄存器配置错误：既不是某个Datapoint的起始寄存器，也不是某个Datapoint的结束寄存器
                        }
                    }
                    break;
            } //switch

            return 0;
        }

        private int ConvertXmlResIdToConmandResId(UpDataBase.RTWriteProxy.MType type, int xmlResId)
        {
            int cmdResId = -1;

            switch (type)
            {
                case UpDataBase.RTWriteProxy.MType.AI:
                    cmdResId = xmlResId - CMEquipment._XMLRESIGERIDAI;
                    break;

                case UpDataBase.RTWriteProxy.MType.ACC:
                    cmdResId = xmlResId - CMEquipment._XMLRESIGERIDACC;
                    break;

                case UpDataBase.RTWriteProxy.MType.DI:
                    cmdResId = xmlResId - CMEquipment._XMLRESIGERIDDI;
                    break;

                case UpDataBase.RTWriteProxy.MType.AO:
                    cmdResId = xmlResId - CMEquipment._XMLRESIGERIDAO;
                    break;

                case UpDataBase.RTWriteProxy.MType.DO:
                    cmdResId = xmlResId - CMEquipment._XMLRESIGERIDDO;
                    break;

                default:
                    break;
            }
            return cmdResId;
        }
        private int ConvertConmandResIdToXmlResId(UpDataBase.RTWriteProxy.MType type, int cmdResId)
        {
            int xmlResId = 0;

            switch (type)
            {
                case UpDataBase.RTWriteProxy.MType.AI:
                    xmlResId = cmdResId + CMEquipment._XMLRESIGERIDAI;
                    break;

                case UpDataBase.RTWriteProxy.MType.ACC:
                    xmlResId = cmdResId + CMEquipment._XMLRESIGERIDACC;
                    break;

                case UpDataBase.RTWriteProxy.MType.DI:
                    xmlResId = cmdResId + CMEquipment._XMLRESIGERIDDI;
                    break;

                case UpDataBase.RTWriteProxy.MType.AO:
                    xmlResId = cmdResId + CMEquipment._XMLRESIGERIDAO;
                    break;

                case UpDataBase.RTWriteProxy.MType.DO:
                    xmlResId = cmdResId + CMEquipment._XMLRESIGERIDDO;
                    break;

                default:
                    break;
            }
            return xmlResId;
        }

        private byte GetFunNumByType(UpDataBase.RTWriteProxy.MType type)
        {
            byte funNum = 0;

            switch (type)
            {
                case UpDataBase.RTWriteProxy.MType.DI:
                    funNum = 1;
                    break;

                case UpDataBase.RTWriteProxy.MType.AI:
                case UpDataBase.RTWriteProxy.MType.ACC:
                    funNum = 3;
                    break;

                case UpDataBase.RTWriteProxy.MType.DO:
                    funNum = 5;
                    break;

                case UpDataBase.RTWriteProxy.MType.AO:
                    funNum = 6;
                    break;

                default:
                    funNum = 0;  
                    break;
            }

            return funNum;
        }
        private int GetReadResCountByDataType(Type dataType)
        {
            int resCount = -1;

            if (typeof(UInt16) == dataType || typeof(Int16) == dataType || typeof(Boolean) == dataType)
            {
                resCount = 1;
            }
            else if (typeof(UInt32) == dataType || typeof(Int32) == dataType || typeof(Single) == dataType)
            {
                resCount = 2;
            }
            else
            {
                resCount = -1;
            }

            return resCount;
        }

        /// <summary>
        ///  从data中offset位置提取一个UInt16元素合成UInt16，并返回对应的值（double类型）
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">数据中的位置</param>
        /// <returns></returns>
        private double UInt16ToUInt16(List<UInt16> data, int offset)
        {
            //return data[offset];

            double val = -1;
            try
            {
                val = data[offset];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                val = -2;
            }
            return val;

        }
        private double UInt16ToUInt16(UInt16 data)
        {
            double val = data;
            return val;

        }

        /// <summary>
        ///  从data中offset位置提取一个UInt16元素合成Int16，并返回对应的值（double类型）
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">数据中的位置</param>
        /// <returns></returns>
        private double UInt16ToInt16(List<UInt16> data, int offset)
        {
            //return BitConverter.ToInt16(BitConverter.GetBytes(data[offset]), 0);

            double val = -1;
            try
            {
                val = BitConverter.ToInt16(BitConverter.GetBytes(data[offset]), 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                val = -2;
            }
            return val;
        }
        private double UInt16ToInt16(UInt16 data)
        {
            double val = BitConverter.ToInt16(BitConverter.GetBytes(data), 0);
            return val;
        }

        /// <summary>
        ///  从data中offset位置开始提取两个UInt16元素合成UInt32，并返回对应的值（double类型）
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">数据中的位置</param>
        /// <returns></returns>
        private double UInt16ToUInt32(List<UInt16> data, int offset)
        {
            double val = -1;

            try
            {
                val = (UInt32)((UInt32)data[offset] + (UInt32)data[offset + 1] * 65536u);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                val = -2;
            }
            return val;
        }
        private double UInt16ToUInt32(UInt16 firData, UInt16 secData)
        {
            double val = -1;

            try
            {
                val = (UInt32)((UInt32)firData + (UInt32)secData * 65536u);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                val = -2;
            }
            return val;
        }

        /// <summary>
        ///  从data中offset位置开始提取两个UInt16元素合成Int32，并返回对应的值（double类型）
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">数据中的位置</param>
        /// <returns></returns>
        private double UInt16ToInt32(List<UInt16> data, int offset)
        {
            double val = -1;
            try
            {
                val = BitConverter.ToInt32(BitConverter.GetBytes((UInt32)((UInt32)data[offset] + (UInt32)data[offset + 1] * 65536u)), 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                val = -2;
            }
            return val;
        }
        private double UInt16ToInt32(UInt16 firData, UInt16 secData)
        {
            double val = -1;
            try
            {
                val = BitConverter.ToInt32(BitConverter.GetBytes((UInt32)((UInt32)firData + (UInt32)secData * 65536u)), 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                val = -2;
            }
            return val;
        }

        /// <summary>
        ///  从data中offset位置开始提取两个UInt16元素合成Single（即float），并返回对应的值（double类型）
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">数据中的位置</param>
        /// <returns></returns>
        private double UInt16ToSingle(List<UInt16> data, int offset)
        {
            double val = -1;
            try
            {
                val = BitConverter.ToSingle
                        (BitConverter.GetBytes((UInt32)((UInt32)data[offset] + (UInt32)data[offset + 1] * 65536u)), 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                val = -2;
            }
            return val;
        }
        private double UInt16ToSingle(UInt16 firData, UInt16 secData)
        {
            double val = -1;
            try
            {
                val = BitConverter.ToSingle
                        (BitConverter.GetBytes((UInt32)((UInt32)firData + (UInt32)secData * 65536u)), 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                val = -2;
            }
            return val;
        }

        /// <summary>
        /// 根据DataPoint.Type合成DataPoint.PointValue
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private double UInt16ToDouble(List<UInt16> data, int offset, Type type)
        {
            double doubleData = -1;

            //offset -= 40000;

            if (offset > data.Count)
            {
                return -1;
            }

            if (typeof(UInt16) == type)
            {
                return UInt16ToUInt16(data, offset);
            }
            else if (typeof(Int16) == type)
            {
                return UInt16ToInt16(data, offset);
            }
            else if (typeof(UInt32) == type)
            {
                return UInt16ToUInt32(data, offset);
            }
            else if (typeof(Int32) == type)
            {
                return UInt16ToInt32(data, offset);
            }
            else if (typeof(Single) == type)
            {
                return UInt16ToSingle(data, offset);
            }

            return doubleData;
        }
        private double UInt16ToDouble(Type type, UInt16 first)
        {
            double doubleData = -1;

            if (typeof(UInt16) == type || typeof(Int16) == type || typeof(Boolean) == type)
            {
                return UInt16ToUInt16(first);
            }

            return doubleData;
        }
        private double UInt16ToDouble(Type type, UInt16 first, UInt16 second)
        {
            //double doubleData = -1;

            if (typeof(UInt32) == type)
            {
                return UInt16ToUInt32(first, second);
            }
            else if (typeof(Int32) == type)
            {
                return UInt16ToInt32(first, second);
            }
            else if (typeof(Single) == type)
            {
                return UInt16ToSingle(first, second);
            }
            else
            {
                return UInt16ToUInt16(first);
            }

            //return doubleData;
        }

        private void CloseTcpClient(TcpClient tcpClient)
        {
            if (null != tcpClient)
            {
                try
                {
                    tcpClient.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message + "{0}", DateTime.Now);
                }
            }
        }
        private void CloseNetIOStream(NetworkStream netIOStream)
        {
            if (null != netIOStream)
            {
                try
                {
                    netIOStream.Close();
                    netIOStream.Dispose();

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message + "{0}", DateTime.Now);
                }
            }
        }
       
        #endregion

        #region 废弃暂且保留的代码
        //private List<UpDataBase.RTWriteProxy.PointRTModel> ProcessU16DataToPointRTModelWithFunNum(List<UInt16> ui16Data, byte funNum, int startR, int countR)
        //{
        //    switch (funNum)
        //    {
        //        case 1:
        //            //DI
        //            SetDIDataPointValue(ui16Data, startR, countR);                    //填充DataPoit.PointValue
        //            return SetDataPointToPointRTModel(_cmEquipment.DiDataPointList);//第三步：将DataPoint封装成UpDataBase.RTWriteProxy.PointRTModel类型

        //            break;
        //        case 3:
        //            if (startR < CMEquipment.FIRSTACCREGISTERID)
        //            {
        //                //AI
        //                SetAIDataPointValue(ui16Data, startR, countR); //填充DataPoit.PointValue
        //                return SetDataPointToPointRTModel(_cmEquipment.AiDataPointList);//第三步：将DataPoint封装成UpDataBase.RTWriteProxy.PointRTModel类型

        //            }
        //            else if (startR >= CMEquipment.FIRSTACCREGISTERID)
        //            {
        //                //ACC
        //                SetACCDataPointValue(ui16Data, startR, countR); //填充DataPoit.PointValue
        //                return SetDataPointToPointRTModel(_cmEquipment.AccDataPointList);//第三步：将DataPoint封装成UpDataBase.RTWriteProxy.PointRTModel类型
        //            }




        //            break;
        //    } //switch (funNum)

        //    return null;
        //} //ProcessDataWithFunNum

        ///// <summary>
        ///// 将数据放入对应的DIDataPoint.dataPointValue
        ///// </summary>
        ///// <param name="ui16DataList">数据</param>
        ///// <param name="startR">起始寄存器id，从0开始,注意：当时功能码3时，注意与40001的转换</param>
        ///// <param name="countR">所读取的寄存器个数</param>
        //private void SetDIDataPointValue(List<UInt16> ui16DataList, int startR, int countR)
        //{
        //    //-------------------------------------------------------
        //    //该方法处理前提是：假设DiDataPointList中的寄存器id连续的
        //    //若设寄存器id不连续，不能用此方法？？？？？？？？？？
        //    //-------------------------------------------------------

        //    int count = _cmEquipment.DiDataPointList.Count;

        //    int offStartPoint = 0;   //ResId是startR的DataPointList在DiDataPointList中的位置
        //    for (int i = 0; i < count; ++i)
        //    {
        //        if (startR == (_cmEquipment.DiDataPointList[i].ResId - 1))
        //        {
        //            offStartPoint = i;
        //        }
        //    }

        //    for (int i = offStartPoint; i < count; ++i)
        //    {
        //        int startRes = _cmEquipment.DiDataPointList[i].ResId - 1; //寄存器号转化
        //        Type dataType = _cmEquipment.DiDataPointList[i].DataType;

        //        int offRegister = offStartPoint * 1; //前offStartPoint个DataPoint占地寄存器个数

        //        double dataPointValue = 0.0;
        //        if (startRes > ui16DataList.Count)
        //            dataPointValue = -1;
        //        else
        //            dataPointValue = ui16DataList[startRes - offStartPoint];

        //        _cmEquipment.DiDataPointList[i].PointValue = dataPointValue;
        //    }
        //}

        ///// <summary>
        ///// 将数据放入对应的AIDataPoint.dataPointValue
        ///// </summary>
        ///// <param name="ui16DataList">数据</param>
        ///// <param name="startR">起始寄存器id，从0开始,注意：当时功能码3时，注意与40001的转换</param>
        ///// <param name="countR">所读取的寄存器个数</param>
        //private void SetAIDataPointValue(List<UInt16> ui16DataList, int startR, int countR)
        //{
        //    //-------------------------------------------------------
        //    //该方法处理前提是：假设AiDataPointList中的寄存器id连续的
        //    //若设寄存器id不连续，不能用此方法？？？？？？？？
        //    //-------------------------------------------------------

        //    int count = _cmEquipment.AiDataPointList.Count;  //AI类型DataPoint的个数

        //    int offStartPoint = 0;   //ResId是startR的DataPointList在AiDataPointList中的位置

        //    //error: 如果startR不等于_cmEquipment.AiDataPointList[i].ResId，
        //    //offStartPoint进被赋值为零。明显有问题
        //    for (int i = 0; i < count; ++i)
        //    {
        //        if (startR == (_cmEquipment.AiDataPointList[i].ResId - 40001))
        //        {
        //            offStartPoint = i;
        //        }
        //    }


        //    //从AiDataPointList中的位置为offStartPoint的AiDataPoint开始对号入座
        //    for (int i = offStartPoint; i < count; ++i)
        //    {
        //        int startRes = _cmEquipment.AiDataPointList[i].ResId - 40001; //起始寄存器id转化
        //        Type dataType = _cmEquipment.AiDataPointList[i].DataType;     //每个数据点类型不同

        //        //(startRes - offStartPoint)不对！！！！！！
        //        double dataPointValue = UInt16ToDouble(ui16DataList, (startRes - offStartPoint), dataType);

        //        _cmEquipment.AiDataPointList[i].PointValue = dataPointValue;
        //    }

        //}

        //private void SetDODataPointValue()
        //{ }

        //private void SetAODataPointValue()
        //{ }

        //private void SetACCDataPointValue(List<UInt16> ui16DataList, int startR, int countR)
        //{
        //    //-------------------------------------------------------
        //    //该方法处理前提是：假设AiDataPointList中的寄存器id连续的
        //        //若设寄存器id不连续，不能用此方法？？？？？？？？
        //    //-------------------------------------------------------

        //    int count = _cmEquipment.AccDataPointList.Count;  //AI类型DataPoint的个数

        //    int offStartPoint = 0;   //ResId是startR的DataPointList在AccDataPointList中的位置

        //    //error: 如果startR不等于_cmEquipment.AccDataPointList[i].ResId，
        //    //offStartPoint进被赋值为零。明显有问题
        //    for (int i = 0; i < count; ++i)
        //    {
        //        if (startR == (_cmEquipment.AccDataPointList[i].ResId - 40001))
        //        {
        //            offStartPoint = i;
        //        }
        //    }


        //    //从AccDataPointList中的位置为offStartPoint的AccDataPointList开始对号入座
        //    for (int i = offStartPoint; i < count; ++i)
        //    {
        //        int startRes = _cmEquipment.AccDataPointList[i].ResId - 48001; //起始寄存器id转化,startRes是指在ui16DataList中相对位置（不是绝对位置）
        //        Type dataType = _cmEquipment.AccDataPointList[i].DataType;     //每个数据点类型不同

        //        int offRegister = offStartPoint * 2; //每个是S32（占2个寄存器）---粗糙处理，其实不对，
        //        //(startRes - offStartPoint)不对！！！！！！
        //        double dataPointValue = UInt16ToDouble(ui16DataList, (startRes - offRegister), dataType);

        //        _cmEquipment.AccDataPointList[i].PointValue = dataPointValue;
        //    }

        //}

        /// <summary>
        /// 将DataPoint封装成UpDataBase.RTWriteProxy.PointRTModel类型
        /// </summary>
        /// <param name="dataPointList"></param>
        /// <returns></returns>
        //private List<UpDataBase.RTWriteProxy.PointRTModel> SetDataPointToPointRTModel(List<DataPoint> dataPointList)
        //{
        //    int count = dataPointList.Count;
        //    List<UpDataBase.RTWriteProxy.PointRTModel> idModelList = new List<UpDataBase.RTWriteProxy.PointRTModel>();

        //    for (int i = 0; i < count; ++i)
        //    {
        //        idModelList.Add(new UpDataBase.RTWriteProxy.PointRTModel()
        //                            {
        //                                ID = new UpDataBase.RTWriteProxy.IDModel()
        //                                {
        //                                    DevID = dataPointList[i].DevId,
        //                                    Type = dataPointList[i].Type,
        //                                    PointID = dataPointList[i].PointId,
        //                                },
        //                                Value = dataPointList[i].PointValue,
        //                                Time = DateTime.Now,
        //                            }
        //                        );
        //    }

        //    return idModelList;
        //}
        #endregion

    } //CmeManager
}//namespace ModbusServer
