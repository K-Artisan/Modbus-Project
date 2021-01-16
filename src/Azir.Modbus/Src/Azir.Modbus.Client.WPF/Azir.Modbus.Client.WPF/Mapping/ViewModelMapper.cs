using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Client.WPF.ViewModel.DataMonitor;
using Azir.Modbus.DataObject.DataPoint;

namespace Azir.Modbus.Client.WPF.Mapping
{
    public static class ViewModelMapper
    {
        #region DataPointRealValueDto And DataPointViewModel
        public static DataPointViewModel ConvertToDataPointViewModel(this DataPointRealValueDto dataPointDto)
        {
            DataPointViewModel vm = new DataPointViewModel();

            vm.Number = dataPointDto.DataPointNumber;
            vm.RealTimeValue = dataPointDto.DataPointRealTimeValue;
            vm.ValueToSet = dataPointDto.ValueToSet;

            return vm;
        }

        public static List<DataPointViewModel> ConvertToDataPointViewModelList(this List<DataPointRealValueDto> dataPointDtos)
        {
            var vms = new List<DataPointViewModel>();
            foreach (var dataPointDto in dataPointDtos)
            {
                vms.AddRange(dataPointDtos.Select(p => ConvertToDataPointViewModel(p)));
            }

            return vms;
        }


        public static DataPointRealValueDto ConvertToDataPointInfoView(this DataPointViewModel dataPointViewModel)
        {
            DataPointRealValueDto dto = new DataPointRealValueDto();

            dto.DataPointNumber = dataPointViewModel.Number;
            dto.DataPointRealTimeValue = dataPointViewModel.RealTimeValue;
            dto.ValueToSet = dataPointViewModel.ValueToSet;

            return dto;
        } 
        #endregion
    }
}
