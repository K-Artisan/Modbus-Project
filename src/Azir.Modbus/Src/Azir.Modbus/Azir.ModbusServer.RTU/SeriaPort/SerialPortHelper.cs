using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.ModbusServer.RTU.Event;

namespace Azir.ModbusServer.RTU
{
    public class SerialPortHelper
    {
        private readonly object _objReadOrWriteLock = new object();   //读、写寄存器锁
        private SerialPort serialPort;

        public SerialPort SerialPort
        {
            get { return serialPort; }
            set { serialPort = value; }
        }

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


        #region 构造函数

        public SerialPortHelper(SerialPort serialPort)
        {
            this.serialPort = serialPort;

            if (this.serialPort.WriteTimeout == -1)
            {
                this.serialPort.ReadTimeout = 10 * 1000;
            }

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

                RaiseCurrentReceiveDataChangedEvent(recevideData.ToList());
            }
            catch (Exception ex)
            {
                //LoggingFactory.GetLogger().WriteDebugLogger("ModbusRTU“读”串口发生异常！" + ex.Message);
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
            succeed = TryOpenSerialPort();
            return succeed;
        }

        public bool TryOpenSerialPort()
        {
            bool succeed = true;

            try
            {
                if (!serialPort.IsOpen)
                {
                    this.serialPort.Open();
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
                //LoggingFactory.GetLogger().WriteDebugLogger("ModbusRTU“写”串口发生异常！" + ex.Message);
            }

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
