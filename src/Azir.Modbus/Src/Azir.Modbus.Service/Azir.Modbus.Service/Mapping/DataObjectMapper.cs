using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.DataObject.DataPoint;
using Azir.Modbus.Protocol.DataPoints;
using Azir.ModbusServer.TCP.DataObject;

namespace Azir.Modbus.Service.Mapping
{
    public class DataObjectMapper
    {
        #region DataPointRealValueDto

        public static DataPointRealValueDto ConverFrom(DataPoint dataPoint)
        {
            DataPointRealValueDto dto = new DataPointRealValueDto();

            if (dataPoint != null)
            {
                dto.DataPointNumber = dataPoint.Number;
                dto.DataPointRealTimeValue = dataPoint.RealTimeValue;
                dto.ValueToSet = dataPoint.ValueToSet; 
            }

            return dto;
        }

        public static List<DataPointRealValueDto> ConvertToListFrom(List<DataPoint> dataPoints)
        {
            List<DataPointRealValueDto> dtos = new List<DataPointRealValueDto>();
            if (dataPoints != null && dataPoints.Any())
            {
                dtos.AddRange(dataPoints.Select(dataPoint => ConverFrom(dataPoint)));
            }
            return dtos;
        }

        public static List<DataPointRealValueDto> ConvertToListFrom(List<DataPointRealValue> dataPoints)
        {
            List<DataPointRealValueDto> dtos = new List<DataPointRealValueDto>();
            if (dataPoints != null && dataPoints.Any())
            {
                dtos.AddRange(dataPoints.Select(dataPoint => ConverFrom(dataPoint)));
            }
            return dtos;
        }

        public static DataPointRealValueDto ConverFrom(DataPointRealValue dataPoint)
        {
            DataPointRealValueDto dto = new DataPointRealValueDto();

            if (dataPoint != null)
            {
                dto.DataPointNumber = dataPoint.DataPointNumber;
                dto.DataPointRealTimeValue = dataPoint.DataPointRealTimeValue;
                dto.ValueToSet = dataPoint.ValueToSet;
            }

            return dto;
        }

        #endregion


    }
}
