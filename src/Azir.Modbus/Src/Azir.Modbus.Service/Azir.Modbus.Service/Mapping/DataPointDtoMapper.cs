using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.DataObject.DataPoint;
using Azir.Modbus.Protocol.DataPoints;
using Azir.ModbusServer.TCP;

namespace Azir.Modbus.Service.Mapping
{
    public static class DataPointDtoMapper
    {
        public static List<DataPointDto> CreateDataPointDtos(ModbusUnit modbusUnit)
        {
            List<DataPointDto> dataDataPointDtos = new List<DataPointDto>();

            var modbusUnitNumber = modbusUnit.Number;
            var modulesDic = modbusUnit.ModulesDic;
            foreach (var module in modulesDic)
            {
                var moduleNumber = module.Key;
                var dpOfmodules = module.Value.DataPoints;
                foreach (var dpOfm in dpOfmodules)
                {
                    DataPointDto dataPointDto = ConverFrom(dpOfm);
                    dataPointDto.ModuleNumber = moduleNumber;
                    dataPointDto.ModuleName = module.Value.Name;

                    dataPointDto.ModbusUnitNumber = modbusUnitNumber;
                    dataPointDto.ModbusUnitName = modbusUnit.Connector.IpAddress.ToString() + ":"+modbusUnit.Connector.Port.ToString();

                    dataDataPointDtos.Add(dataPointDto);
                }
            }

            return dataDataPointDtos;
        }

        public static DataPointDto ConverFrom(DataPoint dataPoint)
        {
            DataPointDto dpDto = new DataPointDto();

            dpDto.Number = dataPoint.Number;
            dpDto.Name = dataPoint.Name;
            dpDto.DeviceAddress = dataPoint.DeviceAddress;
            dpDto.StartRegisterAddress = dataPoint.StartRegisterAddress;
            dpDto.DataPointDataType = dataPoint.DataPointDataType;
            dpDto.DataPointType = dataPoint.DataPointType;
            dpDto.Description = dataPoint.Description;
            dpDto.RealTimeValue = dataPoint.RealTimeValue;
            dpDto.ValueToSet = dataPoint.ValueToSet;

            return dpDto;
        }

    }
}
