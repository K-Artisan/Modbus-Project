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

using ModbusDriver.RTUModel;

namespace ModbusDriver
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.gdRoot.Children.Clear();
            this.gdRoot.Children.Add(new ModbusRTUView());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //double x = SystemParameters.WorkArea.Width;//得到屏幕工作区域宽度
            //double y = SystemParameters.WorkArea.Height;//得到屏幕工作区域高度
            ////double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            ////double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度
            //this.Width = x;//设置窗体宽度
            //this.Height = y;//设置窗体高度
        }
    }
}
