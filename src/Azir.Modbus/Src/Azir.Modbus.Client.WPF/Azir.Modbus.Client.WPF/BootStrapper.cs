using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Azir.Infrastructure.Configuration;
using Azir.Infrastructure.Ioc;
using Azir.Infrastructure.Logging;
using log4net.Config;

namespace Azir.Modbus.Client.WPF
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
            catch (Exception e)
            {
                string message = DateTime.Now.ToString() + " 引导启动失败！异常信息如下 :" + e.Message;
                string logFilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"Log/BootStrapperLog.txt");
                System.IO.File.WriteAllText(logFilePath, message);
            }
        }

        /// <summary>
        /// （在界面显示前）初始化运行环境
        /// </summary>
        private void InitializeRuntimeEnvironmentBeforeShellRun()
        {
            InitializeApplicationSetting();
            InitializeLogger();

            LoggingFactory.GetLogger().WriteSystemLogger("引导程序初始化完成");
        }

        /// <summary>
        /// 初始化程序配置
        /// </summary>
        private static void InitializeApplicationSetting()
        {
            IApplicationSettings applicationSettings = new AppConfigApplicationSettings(); //IocContainerFactory.GetUnityContainer().Resolve<IApplicationSettings>();
            ApplicationSettingsFactory.InitializeApplicationSettingsFactory(applicationSettings);
        }

        /// <summary>
        /// 初始化日志系统
        /// </summary>
        private void InitializeLogger()
        {
            try
            {
                //log4net.Config.XmlConfigurator.Configure();
                string log4NetConfigPath = ApplicationSettingsFactory.GetApplicationSettings().Log4NetConfigPath;
                FileInfo fileInfo = new FileInfo(log4NetConfigPath);
                XmlConfigurator.ConfigureAndWatch(fileInfo);

                ILogger logger = new Log4NetAdapter();//IocContainerFactory.GetUnityContainer().Resolve<ILogger>();
                LoggingFactory.InitializeLogFactory(logger);
            }
            catch (Exception e)
            {
                string message = DateTime.Now.ToString() + "调用InitializeLogger初始化日志系统失败！异常信息如下 :" + e.Message;
                string logFilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"Log/BootStrapperLog.txt");
                System.IO.File.WriteAllText(logFilePath, message);
            }
        }

        /// <summary>
        /// 初始化主界面
        /// </summary>
        private void InitializeShell()
        {
            Application.Current.MainWindow = new NCSMainWindow();
            Application.Current.MainWindow.Show();

            LoggingFactory.GetLogger().WriteSystemLogger("加载程序主窗口完成");
        }
    }
}
