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
using Modbus.Contract.RequestDataBase;
using NCS.Client.WPF.ViewModel;

namespace NCS.Client.WPF.View
{
    /// <summary>
    /// ModbusConfigerView.xaml 的交互逻辑
    /// </summary>
    public partial class ModbusConfigerView : UserControl
    {
        public ModbusConfigerView()
        {
            InitializeComponent();
            this.DataContext = new ModbusConfigerViewModel();
        }
    }
}
