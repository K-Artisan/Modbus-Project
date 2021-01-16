using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using NCS.Infrastructure.Ioc;


using NCS.Service.Messaging.DataPointService;
using NCS.Service.Messaging.ModbusService;
using NCS.Service.Messaging.ModuleService;
using NCS.Service.ServiceInterface;
using NCS.Service.ViewModel.DataPoints;
using NCS.Service.ViewModel.Modules;
using NCS.Client.WPF.Mapping;

namespace NCS.Client.WPF.ViewModel
{
    public class DataPointMonitorViewModel : NotificationObject
    {
        #region 绑定界面的数据

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

        #endregion

        private System.Timers.Timer timerReadingDataPointRealTimeValue = null; //定时读取实时数据
        private readonly object objReadingDataPointRealTimeValueLock = new object();

        public DataPointMonitorViewModel()
        {
            InitializeData();
            InitializeCommads();
            InitializeTimer();
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
            IModbusService modbusService = IocContainerFactory.GetUnityContainer().Resolve<IModbusService>();

            if (null != this.SeleteItem)
            {
                DataPointViewModel dataPointViewModel = this.SeleteItem as DataPointViewModel;
                if (null != dataPointViewModel)
                {
                    SetDataPointValueRequest request = new SetDataPointValueRequest();
                    DataPointInfoView dataPointInfoView = dataPointViewModel.ConvertToDataPointInfoView();
                    request.DataPointsToSetValue.Add(dataPointInfoView);

                    modbusService.SetDataPointValue(request);
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

        private void InitializeData()
        {
            IModuleService moduleService = IocContainerFactory.GetUnityContainer().Resolve<IModuleService>();
            IDataPointService  dataPointService = IocContainerFactory.GetUnityContainer().Resolve<IDataPointService>();
            IModbusService modbusService = IocContainerFactory.GetUnityContainer().Resolve<IModbusService>();

            GetAllModuleRequest getAllModuleRequest = new GetAllModuleRequest();
            GetAllModuleResponse getAllModuleResponse = moduleService.GetAllModules(getAllModuleRequest);
            foreach (var moduleView in getAllModuleResponse.ModuleViews)
            {
                GetDataPointByModuleRequest getDataPointByModuleRequest = new GetDataPointByModuleRequest();
                getDataPointByModuleRequest.ModuleId = moduleView.ModuleId;

                GetDataPointByModuleResponse getDataPointByModuleResponse =
                    dataPointService.GetDataPointInfoByModule(getDataPointByModuleRequest);

                if (getDataPointByModuleResponse.ResponseSucceed)
                {
                    foreach (var dataPointInfoView in getDataPointByModuleResponse.DataPointInfoViews)
                    {
                        DataPointViewModel dataPointViewModel = dataPointInfoView.ConvertToDataPointViewModel();

                        dataPointViewModel.ModuleNumber = moduleView.Number;
                        dataPointViewModel.ModuleName = moduleView.Name;
                        dataPointViewModel.ModuleDescription = moduleView.Description;

                        this.DataPoints.Add(dataPointViewModel);
                    }
                }
            }
        }

        private void InitializeTimer()
        {
            timerReadingDataPointRealTimeValue = new System.Timers.Timer();
            timerReadingDataPointRealTimeValue.Interval = 1000;
            timerReadingDataPointRealTimeValue.Enabled = true;
            timerReadingDataPointRealTimeValue.AutoReset = true;
            timerReadingDataPointRealTimeValue.Elapsed += ReadingDataPointRealTimeValue;  //(object obj, EventArgs e)

            timerReadingDataPointRealTimeValue.Start();
        }

        public void ReadingDataPointRealTimeValue(object obj, EventArgs e)
        {
            IModbusService modbusService = IocContainerFactory.GetUnityContainer().Resolve<IModbusService>();
        
            lock (objReadingDataPointRealTimeValueLock)
            {
                 GetAllDataPointsRealTimeDataResponse response = null;
                 response = modbusService.GetAllDataPointsRealTimeData();

                 if (null != response && response.ResponseSucceed)
                 {
                     List<DataPointRealTimeDataView> dataPointRealTimeDataViews = response.AllDataPointsRealTimeData;
                     foreach (var dataPointRealTimeDataView in dataPointRealTimeDataViews)
                     {
                         DataPointViewModel dataPointViewModel = this.DataPoints.Find(p => p.Id == dataPointRealTimeDataView.DataPointId);
                         if (null != dataPointViewModel)
                         {
                             dataPointViewModel.RealTimeValue = dataPointRealTimeDataView.DataPointRealTimeValue;
                         }
                     }
                 }
            }
        }

    }
}
