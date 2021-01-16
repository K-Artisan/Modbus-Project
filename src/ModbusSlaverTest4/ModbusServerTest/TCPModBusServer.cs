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
using System.Diagnostics;

namespace ModbusServer
{
    /*-----------------------------------------------------------------
     * ModbusRegister
     *----------------------------------------------------------------*/
    /// <summary>
   /// ModBus寄存器
   /// </summary>
    class ModbusRegister
    {
        int equId;              //ModBus寄存器所属ModbusId（即TCPModBusServer中字段equIp）
        public int EquId
        {
            get { return equId; }
            set { equId = value; }
        }

        int resId;              //ModBus寄存器id 
        public int ResId
        {
            get { return resId; }
            set { resId = value; }
        }

        int devId;               //ModBus寄存器所属ModbusId（数据库中的标识，与TCPModBusServer中字段equIp--Modbus设备Id没有任何关系）
        public int DevId
        {
            get { return devId; }
            set { devId = value; }
        }


        double resValue;        //寄存器的值
        public double ResValue
        {
            get { return resValue; }
            set { resValue = value; }
        }

        int pointId;            //寄存器对应的点
        public int PointId
        {
            get { return pointId; }
            set { pointId = value; }
        }

        UpDataBase.RTWriteProxy.MType type; //类型 
        public UpDataBase.RTWriteProxy.MType Type
        {
            get { return type; }
            set { type = value; }
        }

        DateTime dateTime;      //时间(上传数据的时间)
        public DateTime DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }

        public ModbusRegister()
        {
            equId = 0;
            resId = 0;
            devId = 0;
            resValue = 0;
            pointId = 0;
            type = UpDataBase.RTWriteProxy.MType.AI;
            dateTime = DateTime.Now;
        }

        public ModbusRegister(int equiId,int registerId, int ponId, string strType)
        {
            equId = equiId;
            resId = registerId;
            devId = 0;
            resValue = 0;
            pointId = ponId;
            switch (strType)
            {
                case  "AI":
                {   
                    type = UpDataBase.RTWriteProxy.MType.AI;
                    break;
                }

                case "DI":
                {
                    type = UpDataBase.RTWriteProxy.MType.DI;
                    break;
                }
                case "AO":
                {
                    type = UpDataBase.RTWriteProxy.MType.AO;
                    break;
                }
                case "DO":
                {
                    type = UpDataBase.RTWriteProxy.MType.DO;
                    break;
                }
                case "ACC":
                {
                    type = UpDataBase.RTWriteProxy.MType.ACC;
                    break;
                }     
            }
            dateTime = DateTime.Now;
        }
    }// end of ModbusRegister


    /*--------------------------------------------------------------------------
     * TCPModBusServer
     *-------------------------------------------------------------------------*/
    /// <summary>
    /// ModBusTCP客户端
    /// </summary>
    class TCPModBusServer  //TCP客户端
    {
        //private Thread requestThread;              // 请求线程
        private TcpClient tcpClient;                 // Tcpclient
        public TcpClient TcpClient
        {
            get { return tcpClient; }
            set { tcpClient = value; }
        }  

        private string modbusIP;                     // ModbusIp
        public string ModbusIP
        {
            get { return modbusIP; }
            set { modbusIP = value; }
        }

        //private string hostname;                   // 服务器DNS

        private string serverIP = "192.168.0.83";    // 服务器Ip
        public string ServerIP
        {
            get { return serverIP; }
            set { serverIP = value; }
        }

        private int serverPort = 502;                // 服务器端口
        public int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }
       
        private NetworkStream netIOStream;           //网络传输流
        public NetworkStream NetIOStream
        {
            get { return netIOStream; }
            set { netIOStream = value; }
        }
        //// 请求数据包  
        //private byte[] arrMsgReq = new byte[]{00, 00, 00, 00, 
        //                                        00, 
        //                                        06,             //内容长度
        //                                        01,             //地址域
        //                                        03,             //功能码
        //                                        00, 10,         //开始地址
        //                                        00, 20};        //寄存器个数 
        ModbusSendDataStruct sendData;    // 请求数据包 
        internal ModbusSendDataStruct SendData
        {
            get { return sendData; }
            set { sendData = value; }
        }

        ModbusRecvDataStruct recData;    // 接收数据包
        internal ModbusRecvDataStruct RecData
        {
            get { return recData; }
            set { recData = value; }
        }

        //ModbusSendRecvData sendRecvData; //请求与其对应的接收包

        int equIp;  //Modbus设备Id
        public int EquIp
        {
            get { return equIp; }
            set { equIp = value; }
        }

        List<ModbusRegister> registerList = new List<ModbusRegister>();  //Modbus上的所有寄存器
        internal List<ModbusRegister> RegisterList
        {
            get { return registerList; }
            set { registerList = value; }
        }


        public TCPModBusServer()
        {
            equIp = 0;
        }

        
        public TCPModBusServer(int modbusId,string xmlPath)
        {  
            InitModbusServerWithXml(modbusId, xmlPath);
        }

        /// <summary>
        /// 用xml文件初始化Modbus参数（包括其内部的寄存器）
        /// </summary>
        /// <param name="xmlPath">Modbus设备Id</param>
        /// <param name="modbusId">xml文件路径</param>
        public void InitModbusServerWithXml(int modbusId, string xmlPath)
        {
            System.Xml.XmlDataDocument xmlDoc = new System.Xml.XmlDataDocument();
            xmlDoc.Load(xmlPath);

            foreach (XmlNode node1 in xmlDoc.ChildNodes)
            {
                foreach (XmlNode node2 in node1.ChildNodes)
                {
                    //设置ModbusId
                    equIp = modbusId;

                    //设置ModbusIP
                    if (node2.Name == "Equip" && node2.Attributes["ID"].Value == Convert.ToString(modbusId))
                    {
                        modbusIP = node2.Attributes["IP"].Value;
                    }

                    //设置所有寄存器
                    if (node2.Name == "Point" && node2.Attributes["MachineID"].Value == Convert.ToString(modbusId))
                    {
                        int resId = Convert.ToInt32(node2.Attributes["RegID"].Value);
                        int devId = Convert.ToInt32(node2.Attributes["DevID"].Value);
                        int pointId = Convert.ToInt32(node2.Attributes["PointID"].Value);
                        string strType = node2.Attributes["Type"].Value.Trim();

                        registerList.Add(new ModbusRegister(modbusId, resId, pointId, strType) 
                                             {
                                                 DevId = devId,
                                             }
                                         );
                    }
                }
            }
        }

        private System.Timers.Timer timeSend = new System.Timers.Timer(); //定时发送请求和接收读取寄存器的值

        //定义委托：TCPModbusServer上传寄存器数据给的委托
        public delegate void RegisterValueChangeHandler(object obj, UpDataEventArgs newData);

        //定义事件：TCPModbusServer上的寄存器数据变化的事件
        //public event RegisterValueChangeHandler OnRegisterValueChangeEvent = delegate { };
        public event EventHandler<UpDataEventArgs> OnRegisterValueChangeEvent;

        public void StartModbusServer()  
        {
            //tcpClient = new TcpClient();

            ////连接服务器
            //try
            //{
            //    tcpClient.Connect(serverIP, serverPort);
            //    Debug.WriteLine("Connect...");
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine("Connect...error:" + e.Message);
            //}

            ////获取网络传输流
            //try
            //{
            //    netIOStream = tcpClient.GetStream();
            //    netIOStream.ReadTimeout = 500;
            //    netIOStream.WriteTimeout = 500;
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine("tcpClient error:" + e.Message);
            //}


            //定时发送请求和接收读取寄存器的值
            //timeSend = new System.Timers.Timer();
            //timeSend.Elapsed += RequsAndRecvRegisterValue;
            //timeSend.Interval = 1000;
            //timeSend.Enabled = true;

            RequsAndRecvRegisterValue();
            ///////////////////////////////////////////////////////////////////
        } //end of StartModbusServer

        /// <summary>
        /// 发送请求和接收读取寄存器的值
        /// </summary>
        /// 
        //public void RequsAndRecvRegisterValue(object obj, EventArgs e)
        public void RequsAndRecvRegisterValue()
        {
            tcpClient = new TcpClient();

            //连接服务器
            try
            {
                tcpClient.Connect(serverIP, serverPort);
                Debug.WriteLine("Connect...");
            }
            catch (Exception e1)
            {
                Debug.WriteLine("Connect...error:" + e1.Message);
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
                Debug.WriteLine("tcpClient error:" + e2.Message);
            }

            if (netIOStream.CanWrite)
                {
                    //发送请求包
                    sendData = new ModbusSendDataStruct();
                    //sendData.RegCount = 20;
                    SetSendDataFormat(0, 6);    //该方法需要改造，根据xml设定请求数据包
                    sendData.FunNum = 3;
                    byte[] temp = sendData.ToArray();
                    netIOStream.Write(temp, 0, temp.Length); 

                    netIOStream.Flush();                
                }

                if (netIOStream.CanRead)
                {
                    byte[] recvdata = new byte[1026];
                    int recvlen = netIOStream.Read(recvdata, 0, recvdata.Length); //接收回应包

                    ///////////////////////////////////////////////
                    recData = new ModbusRecvDataStruct(); //考虑到接收到的数据只是暂时保持，因此读后要清空或重新分配内,这里重新分配内存

                    //recData.RecvDataToUInt16WithFunNum(recvdata);
                    //recData.ParseRecvResData(recvdata);   //解析byte[]，并保存在recData.recData.RecvResData中
                    
                    SetRegisterValueWithRecResData();     //将收到的值保存到寄存器（列表）中
                    /////////////////////////////////////////////

                    List<UpDataBase.RTWriteProxy.PointRTModel> updataList = new List<UpDataBase.RTWriteProxy.PointRTModel>(); //ModbusUpDataBase需要上传的数据列表

                    // 从ModBus中的寄存器列表列表registerList中提取数据并封装成UpDataBase.RTWriteProxy.PointRTModel类型
                    for (int indexRes = 0; indexRes < registerList.Count; ++indexRes)
                    {
                        updataList.Add(new UpDataBase.RTWriteProxy.PointRTModel()
                                            {
                                                ID = new UpDataBase.RTWriteProxy.IDModel()
                                                {
                                                    DevID = registerList[indexRes].DevId,
                                                    Type = registerList[indexRes].Type,
                                                    PointID = registerList[indexRes].PointId,
                                                },
                                                Value = registerList[indexRes].ResValue,
                                                Time = DateTime.Now,
                                            }
                                        ); //Add

                        Debug.WriteLine("[{0}]: {1}", indexRes, registerList[indexRes].ResValue);
 
                    }//for

                    if (null != OnRegisterValueChangeEvent) //确认有方法可以执行
                    {
                        //封装ModbusUpDataBase需要上传的数据列表
                        UpDataEventArgs upData = new UpDataEventArgs(updataList);
                        //触发事件，并上传数据
                        OnRegisterValueChangeEvent(this, upData); 
                    }
                }//if
              
                netIOStream.Close();
                tcpClient.Close();
        } //end of StartRequestfu


        /// <summary>
        /// 检查modbus中是否存在对应的寄存器
        /// </summary>
        /// <param name="startR">起始寄存器id</param>
        /// <param name="countR">从起始寄存器id开始连续往后的寄存器个数</param>
        public void SetSendDataFormat(int startR, int countR)
        {
            for (int i = 0; i < countR; ++i)
            {
                //检查modbus中是否存在对应的寄存器
                if (!registerList.Exists(list => list.ResId == (startR + i)))
                {
                    Debug.WriteLine("SetSendDataFormat设置的寄存器id={0}不存在！", startR + i);
                    return;
                }
            }
            sendData.StartReg = (ushort)startR;
            sendData.RegCount = (ushort)countR;
        }

        /// <summary>
        /// 通过接收到的解析后的数据设置寄存器的值：
        ///     将接收到的寄存器的值对号入座（存入对应的寄存器中）
        ///     根据解析后的寄存器值列表
        /// </summary>！
        public void SetRegisterValueWithRecResData()
        {
            //需要安全检查registerList[i + sendData.StartReg]不存在怎么办，以后补上！
            //。。。。。。
            //第一方案：在此方法检测  --不好
            //第二方案：封装下发送数据包，在那里检查modbus中是否存在对应的寄存器  --好

            int countResData = RecData.RecvResData.Count;  //寄存器值的个数
            int startRes     = (int)sendData.StartReg;     //起始寄存器id
            for (int i = 0; i < countResData; ++i)
            {
                registerList[i + startRes].ResValue = RecData.RecvResData[i];
            }
         }




        //-------------------------
        //重构代码2011.10.08
        //-------------------------

        /// <summary>
        ///  从data中offset位置提取一个UInt16元素合成UInt16，并返回对应的值（double类型）
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">数据中的位置</param>
        /// <returns></returns>
        public double UInt16ToUInt16(List<UInt16> data, int offset)
        {
            return data[offset];
        }

        /// <summary>
        ///  从data中offset位置提取一个UInt16元素合成Int16，并返回对应的值（double类型）
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">数据中的位置</param>
        /// <returns></returns>
        public double UInt16ToInt16(List<UInt16> data, int offset)
        {
            return BitConverter.ToInt16(BitConverter.GetBytes(data[offset]), 0);
        }

        /// <summary>
        ///  从data中offset位置开始提取两个UInt16元素合成UInt32，并返回对应的值（double类型）
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">数据中的位置</param>
        /// <returns></returns>
        public double UInt16ToUInt32(List<UInt16> data, int offset)
        {
            double val = 0;

            try
            {
                val = (UInt32)((UInt32)data[offset] + (UInt32)data[offset + 1] * 65536u);
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex.Message);
                val = 0;
            }
            return val;
        }

        /// <summary>
        ///  从data中offset位置开始提取两个UInt16元素合成Int32，并返回对应的值（double类型）
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">数据中的位置</param>
        /// <returns></returns>
        public double UInt16ToInt32(List<UInt16> data, int offset)
        {
            double val = 0;
            try
            {
                val = BitConverter.ToInt32(BitConverter.GetBytes((UInt32)((UInt32)data[offset] + (UInt32)data[offset + 1] * 65536u)), 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                val = 0;
            }
            return val;
        }

        /// <summary>
        ///  从data中offset位置开始提取两个UInt16元素合成Single（即float），并返回对应的值（double类型）
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">数据中的位置</param>
        /// <returns></returns>
        public double UInt16ToSingle(List<UInt16> data, int offset)
        {
            double val = 0;
            try
            {
                val = BitConverter.ToSingle
                        (BitConverter.GetBytes((UInt32)((UInt32)data[offset] + (UInt32)data[offset + 1] * 65536u)), 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                val = 0;
            }
            return val;
        }







    }//end of class TCPModBusServer


    /*--------------------------------------------------------------------------
     * UpDataEventArgs
     *-------------------------------------------------------------------------*/
    /// <summary>
    /// TCPModbusServer作为发布类，ModbusUpDataBase作为订阅者类
    /// UpDataEventArgs是发布类上传给订阅者类的数据
    /// </summary>
    public class UpDataEventArgs : System.EventArgs
    {
        private List<UpDataBase.RTWriteProxy.PointRTModel> updata;      //ModbusUpDataBase需要上传的数据列表
        public List<UpDataBase.RTWriteProxy.PointRTModel> Updata
        {
            get { return updata; }
            set { updata = value; }
        }

        public UpDataEventArgs(List<UpDataBase.RTWriteProxy.PointRTModel> data)
        {
            updata = data;
        }

    }//UpDataEventArgs
}
