using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using NCS.Infrastructure.Ioc;
using NCS.Service.ServiceInterface;
using NCS.Service.SeviceImplementation.ModbusService;

namespace NCS.Client.WPF.ViewModel
{
    public class SerialPortConfigerViewModel : NotificationObject
    {
        private ModbusService modbusService = null;
        
        #region 界面绑定变量

        public DelegateCommand OpenOrCloseSerialPortCommand { get; set; }

        private bool serialPortConnecting = false;
        private BitmapImage serialPortConnectStatusImage = new BitmapImage(new Uri(@"/Resources/Images/unconnect.png", UriKind.Relative));
        private string serialPortConnectStatusTip = "串口已关闭";
        
        private string openOrCloseString = "打开串口";
        private string portName = "COM1";
        private int baudRate = 9600;
        private Parity parity = Parity.Odd;
        private int dataBits = 8;
        private StopBits stopBits = StopBits.One;
     
        public bool SerialPortConnecting
        {
            get { return serialPortConnecting; }
            set
            {
                serialPortConnecting = value;
                DoWhenSerialPortConnectingChanged();
                this.RaisePropertyChanged("SerialPortConnecting");
            }
        }
        public BitmapImage SerialPortConnectStatusImage
        {
            get { return serialPortConnectStatusImage; }
            set
            {
                serialPortConnectStatusImage = value;
                this.RaisePropertyChanged("SerialPortConnectStatusImage");
            }
        }
        public string SerialPortConnectStatusTip
        {
            get { return serialPortConnectStatusTip; }
            set
            {
                serialPortConnectStatusTip = value;
                this.RaisePropertyChanged("SerialPortConnectStatusTip");
            }
        }
        public string OpenOrCloseString
        {
            get { return openOrCloseString; }
            set
            {
                openOrCloseString = value;
                this.RaisePropertyChanged("OpenOrCloseString");
            }
        }
        public string PortName
        {
            get { return portName; }
            set
            {
                portName = value;
                this.RaisePropertyChanged("PortName");
            }
        }
        public int BaudRate
        {
            get { return baudRate; }
            set
            {
                baudRate = value;
                this.RaisePropertyChanged("BaudRate");
            }
        }
        public Parity Parity
        {
            get { return parity; }
            set
            {
                parity = value;
                this.RaisePropertyChanged("Parity");
            }
        }
        public int DataBits
        {
            get { return dataBits; }
            set
            {
                dataBits = value;
                this.RaisePropertyChanged("DataBits");
            }
        }
        public StopBits StopBits
        {
            get { return stopBits; }
            set
            {
                stopBits = value;
                this.RaisePropertyChanged("StopBits");
            }
        }



        #endregion

        public SerialPortConfigerViewModel()
        {
            this.modbusService = (ModbusService)IocContainerFactory.GetUnityContainer().Resolve<IModbusService>();



            this.OpenOrCloseSerialPortCommand = new DelegateCommand(new Action(this.OpenOrCloseSerialPortCommandExecute));
            
            SerialPort serialPort = this.modbusService.GetCurrentSerialPort();
            this.PortName = serialPort.PortName;
            this.BaudRate = serialPort.BaudRate;
            this.Parity = serialPort.Parity;
            this.DataBits = serialPort.DataBits;
            this.StopBits = serialPort.StopBits;
            this.SerialPortConnecting = serialPort.IsOpen;
        }

        private void OpenOrCloseSerialPortCommandExecute()
        {
            if (!this.SerialPortConnecting)
            {
                OpenSerialPort();
            }
            else
            {
                CloseSerialPort();
            }
        }

        private void OpenSerialPort()
        {
            SerialPort serialPort = GetSerialPort();
            if (this.modbusService.SetModbusRtuSerialPort(serialPort))
            {
                this.SerialPortConnecting = true;
            }
            else
            {
                this.SerialPortConnecting = false;
                MessageBox.Show("串口" + serialPort.PortName + "打开失败." + "可能的原因:\n\r" +
                                "1.该串口不存在\n\r" +
                                "2.已经被占用\n\r" +
                                "3.串口参数错误\n\r");
            }
        }


        private SerialPort GetSerialPort()
        {
            SerialPort serialPort = new SerialPort();

            serialPort.PortName = this.portName;
            serialPort.BaudRate = this.baudRate;
            serialPort.Parity = this.parity;
            serialPort.DataBits = this.dataBits;
            serialPort.StopBits = this.stopBits;

            return serialPort;
        }


        private void CloseSerialPort()
        {


            if (this.modbusService.CloseCurrentSerialPort())
            {
                this.SerialPortConnecting = false;
            }
            else
            {
                this.SerialPortConnecting = true;
                MessageBox.Show("串口" + this.modbusService.GetCurrentSerialPort().PortName + "关闭失败");
            }
        }

        private void DoWhenSerialPortConnectingChanged()
        {
            if (this.serialPortConnecting)
            {
                this.SerialPortConnectStatusImage =
                    new BitmapImage(new Uri(@"/Resources/Images/connect.png", UriKind.Relative));
                this.SerialPortConnectStatusTip = "串口已打开";
                this.OpenOrCloseString = "关闭串口";
            }
            else
            {
                this.SerialPortConnectStatusImage =
                    new BitmapImage(new Uri(@"/Resources/Images/unconnect.png", UriKind.Relative));
                this.SerialPortConnectStatusTip = "串口已关闭";
                this.OpenOrCloseString = "打开串口";
            }
        }
    }
}
