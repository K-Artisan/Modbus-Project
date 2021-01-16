using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Service.ViewModel.DataPoints;

namespace NCS.Service.Messaging.DataPointHistoryDataService
{
    public class GetDataPiontHistoryDataResponse : AbstracttResponseBase
    {
        public DataPointHistoryDataView DataPointHistoryDataView { get; set; }
    }
}
