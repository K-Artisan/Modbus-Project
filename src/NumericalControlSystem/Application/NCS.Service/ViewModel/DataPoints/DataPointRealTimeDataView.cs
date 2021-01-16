using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NCS.Service.Messaging.DataPointService;

namespace NCS.Service.ViewModel.DataPoints
{
    public class DataPointRealTimeDataView
    {
        /// <summary>
        /// 数据库主键
        /// </summary>
        public int DataPointId { get; set; } 
        public double DataPointRealTimeValue { get; set; }
    }
}
