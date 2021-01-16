using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Service.ViewModel.DataPoints;

namespace NCS.Service.Messaging.DataPointHistoryDataService
{
    public class DataPointHistoryValue
    {
        public string DataPointHistoryDataId { get; set; }
        public int DataPointId { get; set; }
        public DateTime DateTime { get; set; }
        public double HistoryValue { get; set; }
    }
}
