using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using NCS.Client.WPF.ViewModel;

namespace NCS.Client.WPF.View
{
    /// <summary>
    /// SerialPortConfigerView.xaml 的交互逻辑
    /// </summary>
    public partial class SerialPortConfigerView : UserControl
    {
        public SerialPortConfigerView()
        {
            InitializeComponent();

            InitializePortCombox();
            InitializeBaudRateCombox();
            InitializeParityCombox();
            InitializeDataBitsCombox();
            InitializeStopBitsCombox();

            this.DataContext = new SerialPortConfigerViewModel();

        }

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
            //List<string> stopBits = new List<string>() 
            //{ 
            //    "1", "2"
            //};

            string[] stopBitsTring = Enum.GetNames(typeof(StopBits));
            this.cbStopBit.ItemsSource = stopBitsTring;
            this.cbStopBit.SelectedIndex = 0;
        }

        #endregion
    }
}
