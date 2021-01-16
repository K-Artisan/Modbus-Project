using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using NCS.Infrastructure.Configuration;
using NCS.Infrastructure.Ioc;
using NCS.Service.Messaging.DataBaseConfigService;
using NCS.Service.ServiceInterface;

namespace NCS.Client.WPF.ViewModel
{
    public class DataBaseConfigerViewModel : NotificationObject
    {
        #region 绑定到界面的数据

        public DelegateCommand ConnectDataBaseCommand { get; set; }
        public DelegateCommand InitializeDataBaseCommand { get; set; }
 

        private string dataBaseIp = "127.0.0.1";
        private string dataBaseAccount = "root";
        private string dataBasePassword = string.Empty;

        private bool dataBaseConnectSuccess = false;
        private BitmapImage dataBaseConnectStatusImage = new BitmapImage(new Uri(@"/Resources/Images/unconnect.png", UriKind.Relative));
        private string dataBaseConnectStatusTip;

        private bool dataBaseInitSuccess = false;
        private BitmapImage dataBaseInitStatusImage = new BitmapImage(new Uri(@"/Resources/Images/unconnect.png", UriKind.Relative));
        private string dataBaseInitStatusTip;

        public string DataBaseIp
        {
            get { return dataBaseIp; }
            set
            {
                dataBaseIp = value;
                this.RaisePropertyChanged("DataBaseIp");
            }
        }
        public string DataBaseAccount
        {
            get { return dataBaseAccount; }
            set
            {
                dataBaseAccount = value;
                this.RaisePropertyChanged("DataBaseAccount");
            }
        }
        public string DataBasePassword
        {
            get { return dataBasePassword; }
            set
            {
                dataBasePassword = value;
                this.RaisePropertyChanged("DataBasePassword");
            }
        }

        public bool DataBaseConnectSuccess
        {
            get { return dataBaseConnectSuccess; }
            set
            {
                dataBaseConnectSuccess = value;
                DoWhenDataBaseConnectSuccessChanged();
                this.RaisePropertyChanged("DataBaseConnectSuccess");
            }
        }
        public BitmapImage DataBaseConnectStatusImage
        {
            get { return dataBaseConnectStatusImage; }
            set
            {
                dataBaseConnectStatusImage = value;
                this.RaisePropertyChanged("DataBaseConnectStatusImage");
            }
        }
        public string DataBaseConnectStatusTip
        {
            get { return dataBaseConnectStatusTip; }
            set
            {
                dataBaseConnectStatusTip = value;
                this.RaisePropertyChanged("DataBaseConnectStatusTip");
            }
        }

        public bool DataBaseInitSuccess
        {
            get { return dataBaseInitSuccess; }
            set
            {
                dataBaseInitSuccess = value;
                DoWhenDataBaseInitSuccess();
                this.RaisePropertyChanged("DataBaseInitSuccess");
            }
        }
        public BitmapImage DataBaseInitStatusImage
        {
            get { return dataBaseInitStatusImage; }
            set
            {
                dataBaseInitStatusImage = value;
                this.RaisePropertyChanged("DataBaseInitStatusImage");
            }
        }
        public string DataBaseInitStatusTip
        {
            get { return dataBaseInitStatusTip; }
            set
            {
                dataBaseInitStatusTip = value;
                this.RaisePropertyChanged("DataBaseInitStatusTip");

            }
        }

        #endregion

        #region 绑定到界面的数据的辅助函数

        private void DoWhenDataBaseConnectSuccessChanged()
        {
            if (this.dataBaseConnectSuccess)
            {
                this.DataBaseConnectStatusImage 
                    = new BitmapImage(new Uri(@"/Resources/Images/connect.png", UriKind.Relative));
                this.DataBaseConnectStatusTip = "连接数据成功";
            }
            else
            {
                this.DataBaseConnectStatusImage =
                    new BitmapImage(new Uri(@"/Resources/Images/unconnect.png", UriKind.Relative));
                this.DataBaseConnectStatusTip = "连接数据失败";
            }
        }

        private void DoWhenDataBaseInitSuccess()
        {
            if (this.DataBaseInitSuccess)
            {
                this.DataBaseInitStatusImage
                    = new BitmapImage(new Uri(@"/Resources/Images/connect.png", UriKind.Relative));
                this.DataBaseInitStatusTip = "初始化数据成功";
            }
            else
            {
                this.DataBaseInitStatusImage =
                    new BitmapImage(new Uri(@"/Resources/Images/unconnect.png", UriKind.Relative));
                this.DataBaseInitStatusTip = "初始化数据失败";
            }
        }

        #endregion 

        public DataBaseConfigerViewModel()
        {
            InitializeCommads();
            InitializeDataForView();
        }

        private void InitializeCommads()
        {
            this.ConnectDataBaseCommand =
                new DelegateCommand(new Action(this.ConnectDataBaseCommandCommandExecute),
                        new Func<bool>(this.CanConnectDataBaseCommandCommandExecute));

            this.InitializeDataBaseCommand =
                new DelegateCommand(new Action(this.InitializeDataBaseCommandExecute),
                        new Func<bool>(this.CanInitializeDataBaseCommandExecute));
        }

        private void InitializeDataForView()
        {
            IDataBaseConfigService dataBaseConfigService =
                IocContainerFactory.GetUnityContainer().Resolve<IDataBaseConfigService>();

            GetCurrentDataBaseLoginInfoResponse response =
                dataBaseConfigService.GetCurrentDataBaseLoginInfoAndConnetStatus();

            if (response.ResponseSucceed)
            {
                this.DataBaseIp = response.Ip;
                this.DataBaseAccount = response.Account;
                this.DataBasePassword = response.Password;

                this.DataBaseConnectSuccess = response.DataBaseConnecting;
            }
        }

        private void ConnectDataBaseCommandCommandExecute()
        {
            IDataBaseConfigService dataBaseConfigService =
                IocContainerFactory.GetUnityContainer().Resolve<IDataBaseConfigService>();

            TestConnetDataBaseRequest request = new TestConnetDataBaseRequest();
            request.SaveConfig = true;
            request.Ip = this.dataBaseIp;
            request.Account = this.dataBaseAccount;
            request.Password = this.dataBasePassword;

            TestConnetDataBaseResponse response = dataBaseConfigService.TestConnetDataBase(request);

            if (response.ResponseSucceed)
            {
                this.DataBaseConnectSuccess = true;
            }
            else
            {
                this.DataBaseConnectSuccess = false;
                MessageBox.Show(response.Message);
            }

        }

        public bool CanConnectDataBaseCommandCommandExecute()
        {
            bool canExecute = true;

            if (string.IsNullOrWhiteSpace(this.DataBaseIp)
                || string.IsNullOrWhiteSpace(this.DataBaseAccount))
            {
                canExecute = false;
            }

            return canExecute;
        }

        public void InitializeDataBaseCommandExecute()
        {
            if (CreateDataBase() && InitDataBase())
            {
                this.DataBaseInitSuccess = true;
            }
            else
            {
                this.DataBaseInitSuccess = false;
            }
            ;
        }

        private bool CreateDataBase()
        {
            bool success = true;

            IDataBaseConfigService dataBaseConfigService =
                IocContainerFactory.GetUnityContainer().Resolve<IDataBaseConfigService>();
            IApplicationSettings applicationSettings =
                IocContainerFactory.GetUnityContainer().Resolve<IApplicationSettings>();

            string creatadataBaseSqcritFileRelativePath
                = applicationSettings.CreateDataBaseSqcritFilePath;
            if (!string.IsNullOrWhiteSpace(creatadataBaseSqcritFileRelativePath))
            {
                string dataBaseSqcritFileAbsolutePath =
                    System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                                           creatadataBaseSqcritFileRelativePath);

                CreateDataBaseRequest request = new CreateDataBaseRequest();
                request.SqlScriptFiles.Add(dataBaseSqcritFileAbsolutePath);
                CreateDataBaseResponse response = dataBaseConfigService.CreateDataBase(request);

                if (response.ResponseSucceed)
                {
                    success = true;
                }
                else
                {
                    success = false;
                    MessageBox.Show(response.Message);
                }
            }
            else
            {
                success = false;
                MessageBox.Show("无法获取初始化脚本文件！");
            }

            return success;
        }

        private bool InitDataBase()
        {
            bool success = true;

            IDataBaseConfigService dataBaseConfigService =
                IocContainerFactory.GetUnityContainer().Resolve<IDataBaseConfigService>();
            IApplicationSettings applicationSettings =
                IocContainerFactory.GetUnityContainer().Resolve<IApplicationSettings>();

            string initDataBaseSqcritFileRelativePath
                 = applicationSettings.NumericalControlSystemDataBaseSqcritFilePath;
            if (!string.IsNullOrWhiteSpace(initDataBaseSqcritFileRelativePath))
            {
                string initDataBaseSqcritFileAbsolutePath =
                    System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                                           initDataBaseSqcritFileRelativePath);

                ExecuteSqlScriptRequest request = new ExecuteSqlScriptRequest();
                request.SqlScriptFiles.Add(initDataBaseSqcritFileAbsolutePath);
                ExecuteSqlScriptResponse response = dataBaseConfigService.ExecuteSqlScript(request);

                if (response.ResponseSucceed)
                {
                    success = true;
                }
                else
                {
                    success = false;
                    MessageBox.Show(response.Message);
                }
            }
            else
            {
                success = false;
                MessageBox.Show("无法获取初始化脚本文件！");
            }

            return success;
        }

        public bool CanInitializeDataBaseCommandExecute()
        {
            bool canExecute = true;

            return canExecute;
        }
    }
}
