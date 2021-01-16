using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NCS.Service.Messaging.DataPointHistoryDataService;
using NCS.Service.Messaging.DataPointService;
using NCS.Service.ViewModel.DataPoints;
using NCS.Model.Entity;

namespace NCS.Service.Mapping
{
    public static class DataPointHistoryDataMapper
    {
        public static DataPointHistoryDataView ConvetToDataPointHistoryDataView(
            this IEnumerable<DataPointHistoryData> dataPointHistoryDatas)
        {
            DataPointHistoryDataView dataPointHistoryDataView = new DataPointHistoryDataView();

            foreach (DataPointHistoryData dataPointHistoryData in dataPointHistoryDatas)
            {
                DataPointHistoryValue dataPointHistoryValue =
                    dataPointHistoryData.ConvetToDataPointHistoryValue();

                dataPointHistoryDataView.HistoryDataValues.Add(dataPointHistoryValue);
            }

            return dataPointHistoryDataView;
        }

        public static DataPointHistoryValue ConvetToDataPointHistoryValue(
            this DataPointHistoryData dataPointHistoryData)
        {
            DataPointHistoryValue dataPointHistoryValue = new DataPointHistoryValue();

            dataPointHistoryValue.DataPointHistoryDataId = dataPointHistoryData.Id;
            dataPointHistoryValue.DataPointId = dataPointHistoryData.DataPoint.Id;
            dataPointHistoryValue.DateTime = dataPointHistoryData.DateTime;
            dataPointHistoryValue.HistoryValue = dataPointHistoryData.Value;

            return dataPointHistoryValue;
        }

    }


}
