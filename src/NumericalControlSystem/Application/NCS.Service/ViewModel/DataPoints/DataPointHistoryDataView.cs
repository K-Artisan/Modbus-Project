using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NCS.Service.Messaging.DataPointHistoryDataService;
using NCS.Service.Messaging.DataPointService;

namespace NCS.Service.ViewModel.DataPoints
{
    public class DataPointHistoryDataView
    {
        private List<DataPointHistoryValue> historyDataValues = new List<DataPointHistoryValue>();
        public List<DataPointHistoryValue> HistoryDataValues
        {
            get { return historyDataValues; }
            set { historyDataValues = value; }
        }


    }
}
