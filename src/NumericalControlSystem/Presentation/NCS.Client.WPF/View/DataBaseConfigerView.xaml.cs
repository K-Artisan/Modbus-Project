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
using NCS.Client.WPF.ViewModel;

namespace NCS.Client.WPF.View
{
    /// <summary>
    /// DataBaseConfigerView.xaml 的交互逻辑
    /// </summary>
    public partial class DataBaseConfigerView : UserControl
    {
        private DataBaseConfigerViewModel viewModel =null;

        public DataBaseConfigerView()
        {
            InitializeComponent();
            this.viewModel = new DataBaseConfigerViewModel();
            this.DataContext =this.viewModel;
        }

        private void PbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.viewModel.DataBasePassword = this.PbDataBasePassword.Password;
        }

    }
}
