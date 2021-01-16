using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azir.Modbus.Protocol.DataPoints;
using Azir.ModbusServer.TCP.DataObject;

namespace Azir.ModbusServer.TCP.Mapper
{
    public class DataObjectMapper
    {
          #region DataPointRealValue

        public static DataPointRealValue ConverFrom(DataPoint dataPoint)
        {
            DataPointRealValue dpvValue = new DataPointRealValue();

            if (dataPoint != null)
            {
                dpvValue.DataPointNumber = dataPoint.Number;
                dpvValue.DataPointRealTimeValue = dataPoint.RealTimeValue;
                dpvValue.ValueToSet = dataPoint.ValueToSet; 
            }

            return dpvValue;
        }

        public static List<DataPointRealValue> ConvertToListFrom(List<DataPoint> dataPoints)
        {
            List<DataPointRealValue> dps = new List<DataPointRealValue>();
            if (dataPoints != null && dataPoints.Any())
            {
                dps.AddRange(dataPoints.Select(dataPoint => ConverFrom(dataPoint)));
            }
            return dps;
        }

        #endregion
    
    }
}
