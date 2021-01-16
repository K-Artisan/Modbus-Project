using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;

using Modbus.Contract;
using Modbus.RTU;
using ModbusDriver.RTUModel.View;
using Modbus.Common;
using Modbus.Contract.RequestDataBase;

namespace ModbusDriver.RTUModel
{
    /// <summary>
    /// ModbusRTUView.xaml 的交互逻辑
    /// </summary>
    public partial class ModbusRTUView : UserControl
    {
        #region 成员变量

        private ModbusRTU modbusRTU;
        private SerialPort serialPort;
        private bool serialPortConnecting = false;
        private IFunctionNumView currentFunctionNumView;

        #endregion

        #region 构造函数

        public ModbusRTUView()
        {
            InitializeComponent();
            InitializePortCombox();
            InitializeBaudRateCombox();
            InitializeParityCombox();
            InitializeDataBitsCombox();
            InitializeStopBitsCombox();

            InitializeMenuTop();
        }

        #endregion

        #region 串口参数初始化

        /// <summary>
        /// 初始化串口
        /// </summary>
        private void InitializePortCombox()
        {
            List<string> ports = new List<string>();
            for (int i = 1; i < 41; i++)
            {
                ports.Add("COM" + Convert.ToString(i));
            }
            this.cbPorts.ItemsSource = ports;
            this.cbPorts.SelectedIndex = 0;
        }

        /// <summary>
        /// 初始化化波特率
        /// </summary>
        private void InitializeBaudRateCombox()
        {
            List<int> baudRates = new List<int>() 
            { 
               300, 600, 1200, 2400, 4800, 9600,
               19200, 38400, 43000, 56000, 57600,115200
            };

            this.cbBaudRate.ItemsSource = baudRates;
            this.cbBaudRate.SelectedIndex = 5;
        } 

        /// <summary>
        /// 初始化校验位
        /// </summary>
        private void InitializeParityCombox()
        {
            string[] paritiesString = Enum.GetNames(typeof(Parity));
            this.cbParity.ItemsSource = paritiesString;
            this.cbParity.SelectedIndex = 1;
        }

        /// <summary>
        /// 初始化数据位
        /// </summary>
        private void InitializeDataBitsCombox()
        {
            List<int> dataBits = new List<int>() 
            { 
                8, 7 , 6
            };

            this.cbDataBits.ItemsSource = dataBits;
            this.cbDataBits.SelectedIndex = 0;
        }

        /// <summary>
        /// 初始化停止位
        /// </summary>
        private void InitializeStopBitsCombox()
        {
            List<string> stopBits = new List<string>() 
            { 
                "1", "2"
            };

            this.cbStopBit.ItemsSource = stopBits;
            this.cbStopBit.SelectedIndex = 0;
        }

        #endregion

        #region 顶部菜单

        private void InitializeMenuTop()
        {
            MenuItem fuctionNumsMenuItem = new MenuItem();
            fuctionNumsMenuItem.Header = "功能码";

            string[] functionNumTypes = Enum.GetNames(typeof(FunctionNumType));
            foreach (var item in functionNumTypes)
            {
                FunctionNumType functionNumType = (FunctionNumType)Enum.Parse(typeof(FunctionNumType), item);
                currentFunctionNumView = FunctionNumViewManager.GetFunctionNumView(functionNumType);

                if (null != currentFunctionNumView)
                {
                    MenuItem menuSubItem = new MenuItem() { Header = item, Tag = functionNumType };
                    menuSubItem.Click += new RoutedEventHandler(menuSubItem_Click);

                    fuctionNumsMenuItem.Items.Add(menuSubItem);
                }

            }

            this.menuTop.Items.Add(fuctionNumsMenuItem);
        }

        private void menuSubItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem selectedFunNum = sender as MenuItem;
            FunctionNumType functionNumType = (FunctionNumType)selectedFunNum.Tag;

            DisplayFunctionNumView(functionNumType);
        }

        private void DisplayFunctionNumView(FunctionNumType functionNumType)
        {
            this.stLeftDown.Children.Clear();
            currentFunctionNumView = FunctionNumViewManager.GetFunctionNumView(functionNumType);
            if (null != currentFunctionNumView)
            {
                this.stLeftDown.Children.Add((UserControl)currentFunctionNumView);
            }
        }

        #endregion

        #region 设置串口连接状态对应的界面状态

        private void SetConnectionStatus()
        {
            if (this.serialPortConnecting)
            {
                this.imgConnectionStatus.Source = new BitmapImage(
                    new Uri("pack://application:,,,/ResourceLibrary;component/WPFResource/Image/connect.png"));
                this.imgConnectionStatus.ToolTip = "串口" + this.serialPort.PortName + "已经打开";

                this.btOpenSerialPort.Content = "关闭串口";
                this.cbPorts.IsEnabled = false;
                this.cbBaudRate.IsEnabled = false;
                this.cbParity.IsEnabled = false;
                this.cbDataBits.IsEnabled = false;
                this.cbStopBit.IsEnabled = false;
            }
            else
            {
                this.imgConnectionStatus.Source = new BitmapImage(
                    new Uri("pack://application:,,,/ResourceLibrary;component/WPFResource/Image/unconnect.png"));
                this.imgConnectionStatus.ToolTip = "串口" + this.serialPort.PortName + "已经关闭";

                this.btOpenSerialPort.Content = "打开串口";
                this.cbPorts.IsEnabled = true;
                this.cbBaudRate.IsEnabled = true;
                this.cbParity.IsEnabled = true;
                this.cbDataBits.IsEnabled = true;
                this.cbStopBit.IsEnabled = true;
            }
        }
   
        #endregion

        #region 开、关串口

        private void btOpenSerialPort_Click(object sender, RoutedEventArgs e)
        {
            if (!this.serialPortConnecting)
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
            InitializeModbusRTU();

            try
            {
                if (!this.serialPort.IsOpen)
                {
                    this.serialPortConnecting = true;
                    SetConnectionStatus();
                    this.serialPort.Open();
                }
                else
                {
                    MessageBox.Show("串口" + this.serialPort.PortName + "已经打开");
                }
            }
            catch (Exception)
            {
                this.serialPortConnecting = false;
                SetConnectionStatus();
                MessageBox.Show("串口" + this.serialPort.PortName + "打开失败.\n该串口可能不存在或已经被占用");
            }
        }

        private void InitializeModbusRTU()
        {
            this.serialPort = new SerialPort();
            this.serialPort.PortName = this.cbPorts.Text as string;//this.serialPort.PortName = this.cbPorts.SelectedItem as string;
            this.serialPort.BaudRate = (int)this.cbBaudRate.SelectedItem;
            this.serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), (string)this.cbParity.SelectedItem, false);
            this.serialPort.DataBits = (int)this.cbDataBits.SelectedItem;
            this.serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), (string)this.cbStopBit.SelectedItem);

            this.modbusRTU = new ModbusRTU(serialPort);
            this.modbusRTU.OnCurrentRequestDataChanged += new EventHandler<RequstDataEventArgs>(modbusRTU_OnRequestDataChanged);
            this.modbusRTU.OnCurrentReceiveDataChanged += new EventHandler<ReceiveDataEventArgs>(modbusRTU_OnReceiveDataChanged);
        }

        private void CloseSerialPort()
        {
            try
            {
                if (this.serialPort.IsOpen)
                {
                    this.serialPortConnecting = false;
                    SetConnectionStatus();
                    this.serialPort.Close();
                }
                else
                {
                    MessageBox.Show("串口" + this.serialPort.PortName + "已经关闭");
                }
            }
            catch (Exception)
            {
                this.serialPortConnecting = true;
                SetConnectionStatus();
                MessageBox.Show("串口" + this.serialPort.PortName + "关闭失败");
            }
        }

        #endregion

        #region 发送请求帧

        private void btSendRequestion_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckSerialPortCanUse())
            {
                MessageBox.Show("串口还没打开，请先打开串口");
                return;
            }

            if (null != this.currentFunctionNumView)
            {
                //IRequestInfo requestInfo = this.currentFunctionNumView.GetRequestInfo();
               // if (null != requestInfo)
                {
                    if (null != this.modbusRTU && null != this.modbusRTU.SerialPort)
                    {
                        //this.modbusRTU.OperateFunctionNum(requestInfo);

                        //RequestInfo requestInfo2 = (RequestInfo)requestInfo;

                        ///TODO:可删，测试01功能码                        
                        //FunNum01CustomerRequestData customerRequestData = new FunNum01CustomerRequestData();
                        //customerRequestData.DeviceAddress = requestInfo2.DeviceAddress;
                        //customerRequestData.FunctionNum = FunctionNumType.FunctionNum01;
                        //customerRequestData.StartingRegister = requestInfo2.TheByte3and4;
                        //customerRequestData.NumOfRegisterToRead = requestInfo2.TheByte5and6;

                        ///TODO:可删，测试03功能码
                        //FunNum03CustomerRequestData customerRequestData = new FunNum03CustomerRequestData();
                        //customerRequestData.DeviceAddress = requestInfo2.DeviceAddress;
                        //customerRequestData.FunctionNum = requestInfo2.FunctionNum;
                        //customerRequestData.StartingRegister = requestInfo2.TheByte3and4;
                        //customerRequestData.NumOfRegisterToRead = requestInfo2.TheByte5and6;

                        ///TODO:可删，测试05功能码                        
                        //FunNum05CustomerRequestData customerRequestData = new FunNum05CustomerRequestData();
                        //customerRequestData.DeviceAddress = requestInfo2.DeviceAddress;
                        //customerRequestData.FunctionNum = FunctionNumType.FunctionNum05;
                        //customerRequestData.CoilAddress = requestInfo2.TheByte3and4;
                        //customerRequestData.ON = true;

                        ///TODO:可删，测试06功能码                        
                        //FunNum06CustomerRequestData<ushort> customerRequestData = new FunNum06CustomerRequestData<ushort>();
                        //customerRequestData.DeviceAddress = requestInfo2.DeviceAddress;
                        //customerRequestData.FunctionNum = FunctionNumType.FunctionNum06;
                        //customerRequestData.RegisterAddress = requestInfo2.TheByte3and4;
                        //customerRequestData.PresetData = 32768;

                        ///TODO:可删，测试16功能码                        
                        FunNum16CustomerRequestData<int> customerRequestData = new FunNum16CustomerRequestData<int>();
                        //customerRequestData.DeviceAddress = requestInfo2.DeviceAddress;
                        //customerRequestData.FunctionNum = FunctionNumType.FunctionNum16;
                        //customerRequestData.StartingRegisterAddress = requestInfo2.TheByte3and4;
                        //customerRequestData.PresetData = new List<int>()
                        //{ 
                        //    0, 1, 2, 3, 4 , 5 , 6 , 7, 8, 9 , 10 , 11,
                        //    12, 13, 14 , 15 , 16 , 17, 18 , 19 , 20, 21, 22

                        //};

                        this.modbusRTU.OperateFunctionNum<int>(customerRequestData);
                    }
                }
            }
        }

        private bool CheckSerialPortCanUse()
        {
            bool canUse = true;

            if (this.serialPort == null)
            {
                return false;
            }
            if (this.serialPort != null)
            {
                if (!this.serialPort.IsOpen)
                {
                    return false;                    
                }
            }

            return canUse;
        }

        #endregion

        #region 接收请求帧

        private void modbusRTU_OnRequestDataChanged(object sender, RequstDataEventArgs e)
        {
            List<byte> requstData = e.RequstData;
            string recevideDataHexString = null;

            foreach (byte item in requstData)
            {
                //recevideDataHexString += HexString.ByteArrayToHexString(item.ToArray()) + "\n";
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                this.tbRequestData.Text += recevideDataHexString + "\n";
            }));
        }

        #endregion

        #region 接收响应帧

        private void modbusRTU_OnReceiveDataChanged(object sender, ReceiveDataEventArgs e)
        {
            byte[] recevideData = e.ReceiveData.ToArray();
            string recevideDataHexString = HexString.ByteArrayToHexString(recevideData);

            this.Dispatcher.Invoke(new Action(() =>
            {
                this.tbrecevideData.Text += recevideDataHexString + "\n";
            }));
        }

        #endregion

        #region 清空显示区

        private void btClearRequestData_Click(object sender, RoutedEventArgs e)
        {
            this.tbRequestData.Text = null;
        }

        private void btClearRecevideData_Click(object sender, RoutedEventArgs e)
        {
            this.tbrecevideData.Text = null;
        }

        #endregion

    }
}
