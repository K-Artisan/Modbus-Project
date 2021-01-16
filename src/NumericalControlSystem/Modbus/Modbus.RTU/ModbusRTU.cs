using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

using Modbus.Contract;
using System.Threading;
using Modbus.Contract.RequestDataBase;
using NCS.Infrastructure.Logging;

namespace Modbus.RTU
{
    public class ModbusRTU : IModbusProtocol
    {
        #region 字段和属性

        private readonly object _objReadOrWriteLock = new object();   //读、写寄存器锁
        private SerialPort serialPort;
        
        public SerialPort SerialPort
        {
            get { return serialPort; }
            set { serialPort = value; }
        }

        #endregion

        #region 构造函数

        public ModbusRTU(SerialPort serialPort)
        {
            this.serialPort = serialPort;

            //if (this.serialPort.WriteTimeout == -1)
            //{
            //    this.serialPort.ReadTimeout = 10 * 1000;
            //}

            this.serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
        }

        #endregion 

        #region 串口

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                byte[] recevideData = new byte[this.serialPort.BytesToRead];
                this.serialPort.Read(recevideData, 0, recevideData.Length);
                this.CurrentReceiveData = recevideData.ToList();

                RaiseCurrentReceiveDataChangedEvent(this.CurrentReceiveData);
            }
            catch (Exception ex)
            {
                LoggingFactory.GetLogger().WriteDebugLogger("ModbusRTU“读”串口发生异常！" + ex.Message);
            }
        }

        public void SetCurrentSerialPort(SerialPort serialPort)
        {
            this.serialPort = serialPort;
        }

        public SerialPort GetCurrentSerialPort()
        {
            return this.serialPort;
        }

        public bool OpenCurrentSerialPort()
        {
            bool succeed = true;
            succeed = TryOpenSerialPort(this.serialPort);
            return succeed;
        }

        public bool TryOpenSerialPort(SerialPort serialPort)
        {
            bool succeed = true;

            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }
            }
            catch (Exception)
            {
                succeed = false;
            }

            return succeed;
        }


        public bool CloseCurrentSerialPort()
        {
            bool succeed = true;
            succeed = TryCloseSerialPort(this.serialPort);
            return succeed;
        }

        public bool TryCloseSerialPort(SerialPort serialPort)
        {
            bool succeed = true;

            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
            catch (Exception)
            {
                succeed = false;
            }

            return succeed;
        }

        public void WriteSerialPort(byte[] toWriteData)
        {
            try
            {
                lock (_objReadOrWriteLock)
                {
                    this.serialPort.Write(toWriteData, 0, toWriteData.Length);
                    RaiseCurrentRequstDataChangedEvent(toWriteData.ToList());
                }
            }
            catch (Exception ex)
            {
                LoggingFactory.GetLogger().WriteDebugLogger("ModbusRTU“写”串口发生异常！"+ ex.Message);
            }

        }

        #endregion

        #region  IModbusProtocol 成员

        public List<byte> CurrentRequestData
        {
            get;
            set;
        }

        public List<byte> CurrentReceiveData
        {
            get;
            set;
        }

        public delegate void RequstDataChangedEventHandler(object obj, RequstDataEventArgs requstData);
        public event EventHandler<RequstDataEventArgs> OnCurrentRequestDataChanged = delegate { };

        public delegate void ReceiveDataEventChangedHandler(object obj, ReceiveDataEventArgs receiveData);
        public event EventHandler<ReceiveDataEventArgs> OnCurrentReceiveDataChanged = delegate { };

        /// <summary>
        /// 根据功能码，执行相应的操作
        /// (暂时是ModbusTCP、UDP公用接口)
        /// 
        /// 特别注意：
        /// T类型只是针对如下功能码:
        /// 06\16
        /// 其他功能码可以设置为任何类型,因为没用到类型T，
        /// 只是为了统一接口，而使用了泛型函数。
        /// 
        /// </summary>
        /// <typeparam name="T">设置的值的类型，只能是如下类型:
        ///  double\ float\ int \long \short\ uint\ ulong\ ushort
        /// </typeparam>
        /// <param name="customerRequestData">客户端的请求</param>
        /// <returns></returns>
        public bool OperateFunctionNum<T>(ICustomerRequestData customerRequestData)
        {
            bool succeed = true;

            try
            {
                List<List<byte>> rtuRequesCmdByteStreams = RTURequesCommandCreator.CreateRequestCommandByteStream<T>(customerRequestData);

                foreach (var item in rtuRequesCmdByteStreams)
                {
                    WriteSerialPort(item.ToArray());
                }

            }
            catch (Exception)
            {
                succeed = false;
            }
            return succeed;
        }

        private void RaiseCurrentRequstDataChangedEvent(List<byte> currentRequestData)
        {
            if (null != currentRequestData)
            {
                if (null != OnCurrentRequestDataChanged)
                {
                    RequstDataEventArgs requstDataEventArgs = new RequstDataEventArgs(currentRequestData);
                    foreach (EventHandler<RequstDataEventArgs> hanlder in OnCurrentRequestDataChanged.GetInvocationList())
                    {
                        hanlder(this, requstDataEventArgs);
                    }
                }
            }

        }

        private void RaiseCurrentReceiveDataChangedEvent(List<byte> receiveData)
        {
            if (null != receiveData)
            {
                if (null != OnCurrentRequestDataChanged)
                {
                    ReceiveDataEventArgs requstDataEventArgs = new ReceiveDataEventArgs(receiveData);
                    foreach (EventHandler<ReceiveDataEventArgs> hanlder in OnCurrentReceiveDataChanged.GetInvocationList())
                    {
                        hanlder(this, requstDataEventArgs);
                    }
                }
            }
        }


        #endregion

    }
}
