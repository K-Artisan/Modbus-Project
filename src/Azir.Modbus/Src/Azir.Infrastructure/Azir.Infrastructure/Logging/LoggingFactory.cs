using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azir.Infrastructure.Logging
{
    /// <summary>
    /// 使用方法：
    /// 1.在程序启动的地方：
    /// LoggingFactory.InitializeLogFactory(...);
    /// 
    /// 2.调用
    /// LoggingFactory.GetLogger().Log(ex.Message);
    /// </summary>
    public class LoggingFactory
    {
        private static ILogger logger;

        public static void InitializeLogFactory(ILogger logger)
        {
            LoggingFactory.logger = logger;
        }

        public static ILogger GetLogger()
        {
            return logger;
        }
    }
}
