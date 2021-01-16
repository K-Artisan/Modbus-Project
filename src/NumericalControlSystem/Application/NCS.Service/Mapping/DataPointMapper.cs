using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using NCS.Service.AutoMapper;
using NCS.Service.Messaging.DataPointService;
using NCS.Service.ViewModel.DataPoints;
using NCS.Model.Entity;

namespace NCS.Service.Mapping
{
    public static class DataPointMapper
    {
        public static GetDataPointInfoResponse ConvetToGetDataPointInfoResponse(this DataPoint dataPoint)
        {
            GetDataPointInfoResponse getDataPointInfoResponse = new GetDataPointInfoResponse();

            getDataPointInfoResponse.DataPointInfoView = dataPoint.ConvertToDataPoinntInfoView();
            
            return getDataPointInfoResponse;
        }


        public static DataPointInfoView ConvertToDataPoinntInfoView(this DataPoint dataPoint)
        {
            DataPointInfoView dataPointInfoView = new DataPointInfoView();

            dataPointInfoView.Id = dataPoint.Id;
            dataPointInfoView.Number = dataPoint.Number;
            dataPointInfoView.Name = dataPoint.Name;
            dataPointInfoView.DeviceAddress = dataPoint.DeviceAddress;
            dataPointInfoView.StartRegisterAddress = dataPoint.StartRegisterAddress;
            dataPointInfoView.DataType = dataPoint.DataType;
            dataPointInfoView.DataPointType = dataPoint.DataPointType;
            dataPointInfoView.Description = dataPoint.Description;
            dataPointInfoView.RealTimeValue = dataPoint.RealTimeValue;
            dataPointInfoView.ValueToSet = dataPoint.ValueToSet;
            dataPointInfoView.ModuleId = dataPoint.ModuleBelongTo.Id;

            return dataPointInfoView;
        }

        public static DataPoint ConvertToDataPoint(this DataPointInfoView dataPointInfoView)
        {
            DataPoint dataPoint = new DataPoint();

            dataPoint.Id = dataPointInfoView.Id;
            dataPoint.Number = dataPointInfoView.Number;
            dataPoint.Name = dataPointInfoView.Name;
            dataPoint.DeviceAddress = dataPointInfoView.DeviceAddress;
            dataPoint.StartRegisterAddress = dataPointInfoView.StartRegisterAddress;
            dataPoint.DataType = dataPointInfoView.DataType;
            dataPoint.DataPointType =dataPointInfoView.DataPointType;
            dataPoint.Description = dataPointInfoView.Description;
            dataPoint.RealTimeValue =dataPointInfoView.RealTimeValue;
            dataPoint.ValueToSet =dataPointInfoView.ValueToSet;

            Module moduleBelongTo = new Module();
            moduleBelongTo.Id = dataPointInfoView.ModuleId;
            dataPoint.ModuleBelongTo = moduleBelongTo;

            return dataPoint;
        }

        public static IEnumerable<DataPointInfoView> ConverToDataPointInfoViews(this IEnumerable<DataPoint> dataPoints)
        {
            List<DataPointInfoView> dataPointInfoViews = new List<DataPointInfoView>();

            foreach (var dataPoint in dataPoints)
            {
                DataPointInfoView dataPointInfoView = dataPoint.ConvertToDataPoinntInfoView();
                dataPointInfoViews.Add(dataPointInfoView);
            }

            return dataPointInfoViews;
        }

        public static IEnumerable<DataPoint> ConverToDataPoints(this IEnumerable<DataPointInfoView> dataPointInfoViews)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            foreach (var dataPointInfoView in dataPointInfoViews)
            {
                DataPoint dataPoint = dataPointInfoView.ConvertToDataPoint();
                dataPoints.Add(dataPoint);
            }

            return dataPoints;
        }

        public static IEnumerable<DataPoint> ConverToDataPoints(this List<DataPointInfoView> dataPointInfoViews)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            foreach (var dataPointInfoView in dataPointInfoViews)
            {
                DataPoint dataPoint = dataPointInfoView.ConvertToDataPoint();
                dataPoints.Add(dataPoint);
            }

            return dataPoints;
        }
    }
}
