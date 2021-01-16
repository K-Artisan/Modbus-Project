using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Client.WPF.ViewModel.DataMonitor;
using Azir.Modbus.DataObject.DataPoint;

namespace Azir.Modbus.Client.WPF.Mapping
{
    public static class DataPointDtoMapper
    {
        public static DataPointViewModel ConvertToDataPointViewModel(this DataPointDto dataPointDto)
        {
            DataPointViewModel dpVm = new DataPointViewModel();

            dpVm.Id = dataPointDto.Id;
            dpVm.Number = dataPointDto.Number;
            dpVm.Name = dataPointDto.Name;
            dpVm.DeviceAddress = dataPointDto.DeviceAddress;
            dpVm.StartRegisterAddress = dataPointDto.StartRegisterAddress;
            dpVm.DataPointDataType = dataPointDto.DataPointDataType;
            dpVm.DataPointType = dataPointDto.DataPointType;
            dpVm.RealTimeValue = dataPointDto.RealTimeValue;
            dpVm.ValueToSet = dataPointDto.ValueToSet;

            dpVm.ModuleNumber = dataPointDto.ModuleNumber;
            dpVm.ModuleName = dataPointDto.ModuleName;
            dpVm.ModbusUnitNumber = dataPointDto.ModbusUnitNumber;
            dpVm.ModbusUnitName = dataPointDto.ModbusUnitName;

            return dpVm;
        }

        public static DataPointDto ConvertToDataPointInfoView(this DataPointViewModel dataPointViewModel)
        {
            DataPointDto dpDto = new DataPointDto();

            dpDto.Id = dataPointViewModel.Id;
            dpDto.Number = dataPointViewModel.Number;
            dpDto.Name = dataPointViewModel.Name;
            dpDto.DeviceAddress = dataPointViewModel.DeviceAddress;
            dpDto.StartRegisterAddress = dataPointViewModel.StartRegisterAddress;
            dpDto.DataPointDataType = dataPointViewModel.DataPointDataType;
            dpDto.DataPointType = dataPointViewModel.DataPointType;
            dpDto.RealTimeValue = dataPointViewModel.RealTimeValue;
            dpDto.ValueToSet = dataPointViewModel.ValueToSet;

            dpDto.ModuleNumber = dataPointViewModel.ModuleNumber;
            dpDto.ModuleName = dataPointViewModel.ModuleName;

            dpDto.ModbusUnitNumber = dataPointViewModel.ModbusUnitNumber;

            return dpDto;
        }
    }
}
