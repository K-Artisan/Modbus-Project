using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.UnitOfWork;



using NCS.Service.Messaging.DataPointService;
using NCS.Service.Messaging.ModbusService;
using NCS.Service.ServiceInterface;
using NCS.Model.Repository;

namespace NCS.Service.SeviceImplementation
{
    public class DataPointRealTimeDataService : IDataPointRealTimeDataService
    {
        private readonly IDataPointRepository _dataPointRepository;


        public DataPointRealTimeDataService(IDataPointRepository dataPointRepository)
        {
            _dataPointRepository = dataPointRepository;
        } 

        #region IDataPointRealTimeDataService members

        public GetDataPointRealTimeDataResponse GetDataPointRealTimeData(GetDataPointRealTimeDataRequest request)
        {
            GetDataPointRealTimeDataResponse response = null;



            return response;
        }

        public GetAllDataPointsRealTimeDataResponse GetAllDataPointsRealTimeData()
        {
            GetAllDataPointsRealTimeDataResponse response = null;



            return response;
        }

        #endregion
    }
}
