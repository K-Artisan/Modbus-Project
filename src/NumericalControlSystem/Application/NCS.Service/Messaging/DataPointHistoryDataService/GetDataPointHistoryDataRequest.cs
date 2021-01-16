using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Service.Messaging.DataPointHistoryDataService
{
    public class GetDataPointHistoryDataRequest
    {
        public int DataPointId { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
