using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Infrastructure.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// 系统日志，用于记录系统的一举一动
        /// </summary>
        /// <param name="message"></param>
        void WriteSystemLogger(string message);
       
        /// <summary>
        /// Debug日志，用于记录系统Bug，主要是给方便
        /// 现场无编译器时程序员调试
        /// </summary>
        /// <param name="message"></param>
        void WriteDebugLogger(string message);
    }
}
