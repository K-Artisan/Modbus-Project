using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Client.WPF.ViewModel;
using NCS.Service.ViewModel.DataPoints;

namespace NCS.Client.WPF.Mapping
{
    public  static  class DataPointViewModelMapper
    {
        public static DataPointViewModel ConvertToDataPointViewModel(this DataPointInfoView dataPointInfoView)
        {
            DataPointViewModel dataPointViewModel = new DataPointViewModel();

            dataPointViewModel.Id = dataPointInfoView.Id;
            dataPointViewModel.Number = dataPointInfoView.Number;
            dataPointViewModel.Name = dataPointInfoView.Name;
            dataPointViewModel.DeviceAddress = dataPointInfoView.DeviceAddress;
            dataPointViewModel.StartRegisterAddress = dataPointInfoView.StartRegisterAddress;
            dataPointViewModel.DataType = dataPointInfoView.DataType;
            dataPointViewModel.DataPointType = dataPointInfoView.DataPointType;
            dataPointViewModel.RealTimeValue = dataPointInfoView.RealTimeValue;
            dataPointViewModel.ValueToSet = dataPointInfoView.ValueToSet;

            dataPointViewModel.ModuleId = dataPointInfoView.ModuleId;

            return dataPointViewModel;
        }

        public static DataPointInfoView ConvertToDataPointInfoView(this DataPointViewModel dataPointViewModel)
        {
            DataPointInfoView dataPointInfoView = new DataPointInfoView();

            dataPointInfoView.Id = dataPointViewModel.Id;
            dataPointInfoView.Number = dataPointViewModel.Number;
            dataPointInfoView.Name = dataPointViewModel.Name;
            dataPointInfoView.DeviceAddress = dataPointViewModel.DeviceAddress;
            dataPointInfoView.StartRegisterAddress = dataPointViewModel.StartRegisterAddress;
            dataPointInfoView.DataType = dataPointViewModel.DataType;
            dataPointInfoView.DataPointType = dataPointViewModel.DataPointType;
            dataPointInfoView.RealTimeValue = dataPointViewModel.RealTimeValue;
            dataPointInfoView.ValueToSet = dataPointViewModel.ValueToSet;

            dataPointInfoView.ModuleId = dataPointViewModel.ModuleId;

            return dataPointInfoView;
        }
    }
}
