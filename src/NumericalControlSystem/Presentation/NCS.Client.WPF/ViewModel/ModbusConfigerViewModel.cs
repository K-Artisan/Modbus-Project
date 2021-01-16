using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using NCS.Infrastructure.Ioc;
using NCS.Service.Messaging.ModbusConfigService;
using NCS.Service.ServiceInterface;
using Modbus.Contract.RequestDataBase;

namespace NCS.Client.WPF.ViewModel
{
    public class ModbusConfigerViewModel : NotificationObject
    {
        #region 绑定界面的数据

        public DelegateCommand ImportModbusConfigToDataBaseCommand { get; set; }
        public DelegateCommand SaveDataAnalyzeModeCommand { get; set; }

        private DataAnalyzeMode oldDataAnalyzeMode = DataAnalyzeMode.DataLowToHigh;
        private DataAnalyzeMode currentDataAnalyzeMode = DataAnalyzeMode.DataLowToHigh;
        private List<string> dataAnalyzeModeItemSource = Enum.GetNames(typeof(DataAnalyzeMode)).ToList();

        public DataAnalyzeMode CurrentDataAnalyzeMode
        {
            get { return currentDataAnalyzeMode; }
            set
            {
                this.oldDataAnalyzeMode = this.currentDataAnalyzeMode;
                currentDataAnalyzeMode = value;
                this.RaisePropertyChanged("CurrentDataAnalyzeMode");
            }
        }

        public List<string> DataAnalyzeModeItemSource
        {
            get { return dataAnalyzeModeItemSource; }
            set { dataAnalyzeModeItemSource = value; }
        }

        #endregion

        public ModbusConfigerViewModel()
        {
            InitializeCommands();
            InitializeDataForView();

        }

        #region 命令绑定
        private void InitializeCommands()
        {
            this.ImportModbusConfigToDataBaseCommand = new DelegateCommand(new Action(ImportModbusConfigToDataBaseCommandExecute));
            this.SaveDataAnalyzeModeCommand = new DelegateCommand(new Action(SaveDataAnalyzeModeCommandExecute));
        }

        private void ImportModbusConfigToDataBaseCommandExecute()
        {
            IModbusConfigService modbusConfigService =
                IocContainerFactory.GetUnityContainer().Resolve<IModbusConfigService>();

            SetModbusConfigToDataBaseResponse response = modbusConfigService.SetModbusConfigToDataBase();

            if (response.ResponseSucceed)
            {
                MessageBox.Show("导入成功");
            }
            else
            {
                MessageBox.Show(response.Message);
            }

        }

        private void SaveDataAnalyzeModeCommandExecute()
        {
            IModbusConfigService modbusConfigService =
                IocContainerFactory.GetUnityContainer().Resolve<IModbusConfigService>();

            SetDataAnalyzeModeRequest request = new SetDataAnalyzeModeRequest();
            request.DataAnalyzeMode = this.CurrentDataAnalyzeMode;

            SetDataAnalyzeModeResponse response = modbusConfigService.SetDataAnalyzeMode(request);
            if (!response.ResponseSucceed)
            {
                this.CurrentDataAnalyzeMode = this.oldDataAnalyzeMode;
                MessageBox.Show("设置数据解析方式失败！");
            }
        }

        #endregion

        private void InitializeDataForView()
        {
            IModbusConfigService modbusConfigService =
                IocContainerFactory.GetUnityContainer().Resolve<IModbusConfigService>();
            GetDataAnalyzeModeResponse response = modbusConfigService.GetDataAnalyzeMode();

            if (response.ResponseSucceed)
            {
                this.CurrentDataAnalyzeMode = response.DataAnalyzeMode;
            }
        }
    }
}
