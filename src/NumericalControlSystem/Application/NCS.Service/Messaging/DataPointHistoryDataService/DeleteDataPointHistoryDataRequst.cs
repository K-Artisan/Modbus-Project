using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NCS.Service.Messaging.DataPointHistoryDataService
{
    public class DeleteDataPointHistoryDataRequst
    {
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
