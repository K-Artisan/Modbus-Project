using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NCS.Service.ViewModel.DataPoints;

namespace NCS.Service.Messaging.ModbusService
{
    public class SetDataPointValueRequest
    {
        private List<DataPointInfoView> dataPointsToSetValue = new List<DataPointInfoView>();
        public List<DataPointInfoView> DataPointsToSetValue
        {
            get { return dataPointsToSetValue; }
            set { dataPointsToSetValue = value; }
        }
    }
}
