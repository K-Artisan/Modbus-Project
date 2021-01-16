using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using NCS.Infrastructure.Configuration;
using NCS.Infrastructure.Ioc;
using NCS.Infrastructure.Logging;
using NCS.Service.ServiceInterface;
using NCS.Service.SeviceImplementation;
using NCS.Service.SeviceImplementation.ModbusService;
using System.IO;

namespace NCS.Client.WPF
{
    /// <summary>
    /// 整个应用程序的引导程序
    /// </summary>
    public class BootStrapper
    {
        public void Run()
        {
            try
            {
                InitializeRuntimeEnvironmentBeforeShellRun();
                InitializeShell();
            }
            //catch (ConfigurationErrorsException configurationErrorsException)
            //{
            //可能是app.config文件中与
            //C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Configmachine.config
            //重复如下配置：
            //  <system.data>
            //  <DbProviderFactories>
            //    <!--<add name="MySQL Data Provider" invariant="MySql.Data.MySqlClient" description=".Net Framework Data Provider for MySQL" type="MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Version=6.4.4.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d" />-->
            //  </DbProviderFactories>
            //</system.data>

            //    //ConfigurationSection configurationSection = ConfigurationManager.AppSettings.Set()
            //}
            //catch (ArgumentException argumentException)
            //{
            //    //可能是app.config文件中没有如下配置：
            //    //  <system.data>
            //    //  <DbProviderFactories>
            //    //    <!--<add name="MySQL Data Provider" invariant="MySql.Data.MySqlClient" description=".Net Framework Data Provider for MySQL" type="MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Version=6.4.4.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d" />-->
            //    //  </DbProviderFactories>
            //    //</system.data>

            //}
            catch(Exception e)
            {
                string message = DateTime.Now.ToString() + " 引导启动失败！异常信息如下 :"+ e.Message;
                string logFilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"Log/BootStrapperLog.txt");
                System.IO.File.WriteAllText(logFilePath, message);
            }
        }

        /// <summary>
        /// （在界面显示前）初始化运行环境
        /// </summary>
        private void InitializeRuntimeEnvironmentBeforeShellRun()
        {
            ConfigureIocContainer();
            RegistryDependencies();
            InitializeApplicationSetting();
            InitializeLogger();

            InitializeServices();

            LoggingFactory.GetLogger().WriteSystemLogger("引导程序初始化完成");
        }


        /// <summary>
        /// 配置Ioc容器
        /// </summary>
        private void ConfigureIocContainer()
        {
            IUnityContainer unityContainer = new UnityContainer();
            UnityConfigurationSection configuration = (UnityConfigurationSection)ConfigurationManager.GetSection(UnityConfigurationSection.SectionName);
            configuration.Configure(unityContainer, "defaultContainer");

            IocContainerFactory.InitializeUnityContainer(unityContainer);
        }

        /// <summary>
        /// 为Ioc容器配置映射关系
        /// </summary>
        private void RegistryDependencies()
        {
            //Note:
            //建议不要在这里用硬编码配置依赖关系依赖注入的映射关系，而是：
            //统一在app.config配置文件配置依赖关系依赖注入的映射关系。
        }

        /// <summary>
        /// 初始化程序配置
        /// </summary>
        private static void InitializeApplicationSetting()
        {
            IApplicationSettings applicationSettings = IocContainerFactory.GetUnityContainer().Resolve<IApplicationSettings>();
            ApplicationSettingsFactory.InitializeApplicationSettingsFactory(applicationSettings);
        }

        /// <summary>
        /// 初始化日志系统
        /// </summary>
        private void InitializeLogger()
        {
            log4net.Config.XmlConfigurator.Configure();

            ILogger logger = IocContainerFactory.GetUnityContainer().Resolve<ILogger>();
            LoggingFactory.InitializeLogFactory(logger);
        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        private void InitializeServices()
        {
            IModbusConfigService modbusConfigService = IocContainerFactory.GetUnityContainer().Resolve<IModbusConfigService>();
            IModbusService modbusService = IocContainerFactory.GetUnityContainer().Resolve<IModbusService>();
            IDataPointHistoryDataService dataPointHistoryDataService = IocContainerFactory.GetUnityContainer().Resolve<IDataPointHistoryDataService>();
        }


        /// <summary>
        /// 初始化主界面
        /// </summary>
        private void InitializeShell()
        {
            App.Current.MainWindow = new NCSMainWindow();
            App.Current.MainWindow.Show();

            LoggingFactory.GetLogger().WriteSystemLogger("加载程序主窗口完成");
        }


    }
}
