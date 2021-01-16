using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Querying;



using NCS.Repository.ADO;
using NCS.Repository.ADO.Repositories;
using NCS.Service.Messaging.DataPointHistoryDataService;
using NCS.Service.SeviceImplementation;
using NUnit.Framework;
using NCS.Infrastructure.UnitOfWork;
using NCS.Model.Repository;
using NCS.Model.Entity;

namespace NCS.Service.Test
{
    [TestFixture]
    public class DataPointHistoryDataServiceTest
    {
        [Test()]
        public void TestGetDataPiontHistoryData()
        {
            IUnitOfWork unitOfWork = new AdoUnitOfWork();
            IDataPointRepository dataPointRepository = new DataPointRepository(unitOfWork);
            IDataPointHistoryDataRepository dataPointHistoryDataRepository = new DataPointHistoryDataRepository(unitOfWork);
            IModuleRepository moduleRepository = new ModuleRepository(unitOfWork);

            DataPointHistoryDataService dataPointHistoryDataService = 
                new DataPointHistoryDataService(dataPointHistoryDataRepository,
                    unitOfWork);

            GetDataPointHistoryDataRequest request = new GetDataPointHistoryDataRequest();
            GetDataPiontHistoryDataResponse response = new GetDataPiontHistoryDataResponse();

            request.DataPointId = 1;
            request.BeginTime = new DateTime();
            request.EndTime = DateTime.Now;

            response = dataPointHistoryDataService.GetDataPiontHistoryData(request);

            Assert.True(response.ResponseSucceed);
        }

        [Test()]
        public void TestGetAllDataPointsHistoryData()
        {
            IUnitOfWork unitOfWork = new AdoUnitOfWork();
            IDataPointRepository dataPointRepository = new DataPointRepository(unitOfWork);
            IDataPointHistoryDataRepository dataPointHistoryDataRepository = new DataPointHistoryDataRepository(unitOfWork);
            IModuleRepository moduleRepository = new ModuleRepository(unitOfWork);

            DataPointHistoryDataService dataPointHistoryDataService
                = new DataPointHistoryDataService(dataPointHistoryDataRepository,
                    unitOfWork);

            GetAllDataPointsHistoryDataResponse response = new GetAllDataPointsHistoryDataResponse();

            response = dataPointHistoryDataService.GetAllDataPointsHistoryData();

            Assert.True(response.ResponseSucceed);
        }

        [Test()]
        public void TestAddDataPointHistoryData()
        {
            IUnitOfWork unitOfWork = new AdoUnitOfWork();
            IDataPointHistoryDataRepository dataPointHistoryDataRepository = new DataPointHistoryDataRepository(unitOfWork);
      
            DataPointHistoryDataService dataPointHistoryDataService
                = new DataPointHistoryDataService(dataPointHistoryDataRepository,
                    unitOfWork);

            List<DataPointHistoryData> dataPointHistoryDatas = new List<DataPointHistoryData>();
            for (int i = 0; i < 5000; i++)
            {
                DataPointHistoryData dataPointHistoryData = new DataPointHistoryData()
                {
                    Id = Guid.NewGuid().ToString("D"),
                    DataPoint = new DataPoint() { Id = 1726 },
                    DateTime = DateTime.Now,
                    Value = 123456789.12345
                };

                dataPointHistoryDatas.Add(dataPointHistoryData);
            }

            AddDataPointHistoryDataRequst requst = new AddDataPointHistoryDataRequst();
            requst.DataPointHistoryDatasToAdd = dataPointHistoryDatas;

            AddDataPointHistoryDataResponse response =
            dataPointHistoryDataService.AddDataPointHistoryData(requst);

        }

        [Test()]
        public void TestRemoveDataPointHistoryDataByQuery()
        {
            IUnitOfWork unitOfWork = new AdoUnitOfWork();
            IDataPointHistoryDataRepository dataPointHistoryDataRepository = new DataPointHistoryDataRepository(unitOfWork);

            DataPointHistoryDataService dataPointHistoryDataService
                = new DataPointHistoryDataService(dataPointHistoryDataRepository,
                    unitOfWork);

            DeleteDataPointHistoryDataRequst requst = new DeleteDataPointHistoryDataRequst();
            requst.BeginTime = new DateTime(2013, 8, 3);
            requst.EndTime = DateTime.Now;

            DeleteDataPointHistoryDataResponse response = 
                dataPointHistoryDataService.DeleteDataPointHistoryData(requst);

            Assert.IsTrue(response.ResponseSucceed);
        }

        [Test()]
        public void TestDelecteDataPointHistoryValue()
        {
            IUnitOfWork unitOfWork = new AdoUnitOfWork();
            IDataPointHistoryDataRepository dataPointHistoryDataRepository = new DataPointHistoryDataRepository(unitOfWork);

            DataPointHistoryDataService dataPointHistoryDataService
                = new DataPointHistoryDataService(dataPointHistoryDataRepository,
                    unitOfWork);

            dataPointHistoryDataService.DelecteDataPointHistoryValueBefore(0);
        }

        [Test()]
        public void TestInitializeTimer()
        {
            IUnitOfWork unitOfWork = new AdoUnitOfWork();
            IDataPointHistoryDataRepository dataPointHistoryDataRepository = new DataPointHistoryDataRepository(unitOfWork);

            DataPointHistoryDataService dataPointHistoryDataService
                = new DataPointHistoryDataService(dataPointHistoryDataRepository,
                    unitOfWork);

            //dataPointHistoryDataService.DelecteDataPointHistoryValue();
        }
    } 
}
