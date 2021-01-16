/**********************************************************************************
log4net.Layout.PatternLayout中的转换模式(ConversionPattern) 

%m(message):输出的日志消息，如ILog.Debug(…)输出的一条消息 
%M(method):发生日志请求的方法名（只有方法名而已),警告：会影响性能。

%n(new line):换行 

%d(datetime):输出当前语句运行的时刻 

%r(run time):输出程序从运行到执行到当前语句时消耗的毫秒数 

%t(thread id):当前语句所在的线程ID 

%p(priority): 日志的当前优先级别，即DEBUG、INFO、WARN…等 

%c(class):当前日志对象的名称，例如： 

   模式字符串为：%-10c -%m%n 

   代码为： 

ILog log=LogManager.GetLogger(“Exam.Log”); 

log.Debug(“Hello”); 

则输出为下面的形式： 

Exam.Log       - Hello 

%L：输出语句所在的行号 

%F：输出语句所在的文件名 

%-数字：表示该项的最小长度，如果不够，则用空格填充 

例如，转换模式为%r [%t]%-5p %c - %m%n 的 PatternLayout 将生成类似于以下内容的输出： 

176 [main] INFO  org.foo.Bar - Located nearest gas station. 
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azir.Infrastructure.Configuration;
using Azir.Infrastructure.Logging;
using log4net;
using log4net.Config;

namespace Azir.Infrastructure.Logging
{
    /// <summary>
    /// 使用log4net记录事件
    /// </summary>
    public class Log4NetAdapter : ILogger
    {
        public readonly log4net.ILog SystemLogger;
        public readonly log4net.ILog DebugLogger;
        public readonly log4net.ILog ErrorLogger;

        public Log4NetAdapter()
        {
            XmlConfigurator.Configure();

            //通过ApplicationSettingsFactory确定log4Net使用的日志策略
            SystemLogger = LogManager.GetLogger(ApplicationSettingsFactory.GetApplicationSettings().SystemLoggerName);
            DebugLogger = LogManager.GetLogger(ApplicationSettingsFactory.GetApplicationSettings().DebugLoggerName);
            ErrorLogger = LogManager.GetLogger(ApplicationSettingsFactory.GetApplicationSettings().ErrorLoggerName);
        }

        public void WriteSystemLogger(string message)
        {
            SystemLogger.Info(message);
        }

        public void WriteDebugLogger(string message)
        {
            DebugLogger.Debug(message);
        }

        public void WriteErrorLogger(string message)
        {
            DebugLogger.Debug(message);
        }
    }
}
