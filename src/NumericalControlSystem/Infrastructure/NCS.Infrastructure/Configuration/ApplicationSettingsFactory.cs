using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Infrastructure.Configuration
{
    public class ApplicationSettingsFactory
    {
        private static IApplicationSettings applicationSettings;

        /// <summary>
        /// 采用静态方法和方法注入（而不采用构造器注入）是为让服务和其他类使用
        /// ApplicationSettingsFactory类
        /// 时不必把它添加到该类的构造器中，这可以让几
        ///    ApplicationSettingsFactory.InitializeApplicationSettingsFactory(
        /// ObjectFactory.GetInstance<IApplicationSettings>());
        /// </summary>
        /// <param name="applicationSettings"></param>
        public static void InitializeApplicationSettingsFactory(IApplicationSettings applicationSettings)
        {
            ApplicationSettingsFactory.applicationSettings = applicationSettings;
        }

        public static IApplicationSettings GetApplicationSettings()
        {
            return applicationSettings;
        }
    }
}
