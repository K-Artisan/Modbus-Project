using NCS.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NCS.Service.Messaging.DataPointHistoryDataService
{
    public class AddDataPointHistoryDataRequst
    {
        public List<DataPointHistoryData> DataPointHistoryDatasToAdd { get; set; }
    }
}
