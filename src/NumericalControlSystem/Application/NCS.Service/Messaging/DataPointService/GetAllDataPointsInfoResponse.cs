using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Service.ViewModel.DataPoints;

namespace NCS.Service.Messaging.DataPointService
{
    public class GetAllDataPointsInfoResponse : AbstracttResponseBase
    {
        public IEnumerable<DataPointInfoView> DataPointInfoViews { get; set; }
    }
}
