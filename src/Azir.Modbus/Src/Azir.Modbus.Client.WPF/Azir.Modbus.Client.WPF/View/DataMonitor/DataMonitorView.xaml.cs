using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Azir.Modbus.Client.WPF.ViewModel.DataMonitor;

namespace Azir.Modbus.Client.WPF.View.DataMonitor
{
    /// <summary>
    /// DataMonitorView.xaml 的交互逻辑
    /// </summary>
    public partial class DataMonitorView : UserControl
    {
        DataPointMonitorViewModel viewModel = null;

        public DataMonitorView()
        {
            InitializeComponent();

            this.DgDataPiontInformation.ColumnWidth = DataGridLength.SizeToCells;
            this.viewModel = new DataPointMonitorViewModel();
            this.DataContext = this.viewModel;
        }

        private void SetValueToModus_OnClick(object sender, RoutedEventArgs e)
        {
            this.viewModel.SetValueToModusCommandExecute();
        }
    }
}
