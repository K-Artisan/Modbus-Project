using NCS.Service.ViewModel.DataPoints;

namespace NCS.Service.Messaging.ModbusService
{
    public class GetDataPointRealTimeDataResponse : AbstracttResponseBase
    {
        public DataPointRealTimeDataView DataPiontRealTimeData { get; set; }
    }
}
