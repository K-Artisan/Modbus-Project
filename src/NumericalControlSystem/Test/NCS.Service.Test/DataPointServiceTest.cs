using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using NCS.Service.Messaging.DataPointService;
using NUnit.Framework;
using NCS.Service.SeviceImplementation;
using NCS.Infrastructure.UnitOfWork;

namespace NCS.Service.Test
{
    [TestFixture]
    public class DataPointServiceTest
    {
        [Test()]
        public void GetAllDataPointInfoCanGetDataFromDataBase()
        {
            //IUnitOfWork unitOfWork = new NHUnitOfWork();
            //IDataPointRepository dataPointRepository = new DataPointRepository(unitOfWork);

            //DataPointService dataPointService = new DataPointService(dataPointRepository);

            //GetAllDataPointsInfoResponse getAllDataPointsInfoResponse =
            //    dataPointService.GetAllDataPointInfo();

            //Assert.True(getAllDataPointsInfoResponse.ResponseSucceed);
        }

        [Test()]
        public void GetDataPointInfoCanGetDataFromDataBase()
        {
            //IUnitOfWork unitOfWork = new NHUnitOfWork();
            //IDataPointRepository dataPointRepository = new DataPointRepository(unitOfWork);
            //DataPointService dataPointService = new DataPointService(dataPointRepository);

            //GetDataPointInfoRequest request = new GetDataPointInfoRequest(){
            //        DataPointId = 1
            //    };
            //GetDataPointInfoResponse getDataPointInfoResponse =
            //    dataPointService.GetDataPointInfo(request);

            //Assert.True(getDataPointInfoResponse.ResponseSucceed);
        }
    } 
}
