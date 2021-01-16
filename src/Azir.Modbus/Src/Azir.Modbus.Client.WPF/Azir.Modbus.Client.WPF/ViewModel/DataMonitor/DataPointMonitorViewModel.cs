using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Azir.Infrastructure.Ioc;
using Azir.Modbus.Client.WPF.Mapping;
using Azir.Modbus.DataObject.DataPoint;
using Azir.Modbus.Service;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace Azir.Modbus.Client.WPF.ViewModel.DataMonitor
{
    public class DataPointMonitorViewModel : NotificationObject
    {
        public DelegateCommand SetValueToModusCommand { get; set; }
        public DelegateCommand FoldDateDetailesCommand { get; set; }

        private List<DataPointViewModel> dataPoints = new List<DataPointViewModel>();
        public List<DataPointViewModel> DataPoints
        {
            get { return dataPoints; }
            set { dataPoints = value; }
        }

        public object SeleteItem { get; set; }

        private bool expandFoldDateDetailes;
        public bool ExpandFoldDateDetailes
        {
            get { return expandFoldDateDetailes; }
            set
            {
                expandFoldDateDetailes = value;
                this.RaisePropertyChanged("ExpandFoldDateDetailes");
            }
        }

        private System.Timers.Timer timerReadingDataPointRealTimeValue = null; //定时读取实时数据
        private readonly object objReadingDataPointRealTimeValueLock = new object();

        public DataPointMonitorViewModel()
        {
            InitializeData();
            InitializeCommads();
            //InitializeTimer();
        }

        private void InitializeData()
        {
            List<DataPointDto> dataPointDtos = ModbusService.Instance.GetAllDataPoints();
            foreach (var dpDto in dataPointDtos)
            {
                DataPointViewModel dataPointViewModel = dpDto.ConvertToDataPointViewModel();
                this.DataPoints.Add(dataPointViewModel);
            }
            //注册事件
            ModbusService.Instance.OnDataPointRealValueChanged += DoOnDataPointRealValueChanged;
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadReadModbus), new object());    //参数可选
        }

        private static void ThreadReadModbus(object val)
        {
            ModbusService.Instance.Start();
        }

        private void DoOnDataPointRealValueChanged(object sender, DataPointRealValueEventArgs e)
        {
            List<DataPointRealValueDto> dataPointRealValues = e.DataPointRealValues;
            List<DataPointViewModel> dataPointRealValueVms = dataPointRealValues.ConvertToDataPointViewModelList();
            foreach (var dpVm in dataPointRealValueVms)
            {
                var oldDpVm = this.DataPoints.FirstOrDefault(p => p.Number == dpVm.Number);
                if (oldDpVm != null)
                {
                    oldDpVm.RealTimeValue = dpVm.RealTimeValue;
                }
            }
        }

        private void InitializeCommads()
        {
            this.SetValueToModusCommand =
                new DelegateCommand(new Action(this.SetValueToModusCommandExecute));

            this.FoldDateDetailesCommand =
                new DelegateCommand(new Action(this.FoldDateDetailesCommandExecute));
        }

        #region 用于绑定的命令

        public void SetValueToModusCommandExecute()
        {
            if (null != this.SeleteItem)
            {
                DataPointViewModel dataPointViewModel = this.SeleteItem as DataPointViewModel;
                if (null != dataPointViewModel)
                {
                    SetDataPointValueDto dto = new SetDataPointValueDto()
                    {
                        DataPointNumber = dataPointViewModel.Number,
                        ValueToSet = dataPointViewModel.ValueToSet
                    };
                    ModbusService.Instance.SetDataPointValue(dto);
                }
            }
            else
            {
                MessageBox.Show("先选中一行");
            }
        }

        private void FoldDateDetailesCommandExecute()
        {
            return;
        }

        #endregion


    }
}
