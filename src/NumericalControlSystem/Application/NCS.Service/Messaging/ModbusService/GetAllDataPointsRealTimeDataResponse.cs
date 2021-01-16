using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Service.ViewModel.DataPoints;

namespace NCS.Service.Messaging.ModbusService
{
    public class GetAllDataPointsRealTimeDataResponse : AbstracttResponseBase
    {
        private List<DataPointRealTimeDataView> _allDataPointsRealTimeData = new List<DataPointRealTimeDataView>();
        public List<DataPointRealTimeDataView> AllDataPointsRealTimeData
        {
            get { return _allDataPointsRealTimeData; }
            set { _allDataPointsRealTimeData = value; }
        }
    }
}
