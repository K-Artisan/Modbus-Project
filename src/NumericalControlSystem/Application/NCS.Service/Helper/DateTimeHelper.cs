using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Service.Helper
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// 获取当前时间距离下一个整点的时间间隔
        /// </summary>
        /// <returns>返回当前时间距离下一个整点的时间间隔：单位毫秒</returns>
        public static double GetCurrentTimeUntilNextHourInterval()
        {
            double interval = 0.0;

            int nowMinute = DateTime.Now.Minute;
            interval = (60 - nowMinute) * 60 * 1000;

            return interval;
        }
    }
}
