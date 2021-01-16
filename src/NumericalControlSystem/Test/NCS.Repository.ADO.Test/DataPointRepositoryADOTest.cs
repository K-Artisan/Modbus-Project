using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Configuration;
using NCS.Infrastructure.Ioc;
using NCS.Infrastructure.Logging;
using NCS.Infrastructure.Querying;

using NCS.Repository.ADO.Repositories;
using NUnit.Framework;
using NCS.Model.Entity;

namespace NCS.Repository.ADO.Test
{
     [TestFixture]
    public class DataPointRepositoryAdoTest
    {
         [Test()]
         public void CanAddDataPoint()
         {
             AdoUnitOfWork unitOfWork = new AdoUnitOfWork();
             DataPointRepository dataPointRepository = new DataPointRepository(unitOfWork);

             Random seed = new Random();
             Random randomNum = new Random(seed.Next());

             DataPoint dataPoint = new DataPoint()
             {

                 Number = randomNum.Next(1000),
                 Name = "35#",
                 StartRegisterAddress = 12,
                 DataType = DataType.Bit,
                 Description = "20130801测试！"
             };

             dataPointRepository.Add(dataPoint);
             unitOfWork.Commit();
         }

         [Test()]
         public void TestFindById()
         {
             IApplicationSettings applicationSettings= new AppConfigApplicationSettings();
       
             ApplicationSettingsFactory.InitializeApplicationSettingsFactory(applicationSettings);

             log4net.Config.XmlConfigurator.Configure();

             ILogger logger = new Log4NetAdapter();
             LoggingFactory.InitializeLogFactory(logger);

             AdoUnitOfWork unitOfWork = new AdoUnitOfWork();
             DataPointRepository dataPointRepository = new DataPointRepository(unitOfWork);

             dataPointRepository.FindBy(1);
         }

         [Test()]
         public void TestFindByQuery()
         {
             AdoUnitOfWork unitOfWork = new AdoUnitOfWork();
             DataPointRepository dataPointRepository = new DataPointRepository(unitOfWork);

             Query query = new Query();
             query.AddCriterion(Criterion.Create<DataPoint>(p => p.Number, 10, CriteriaOperator.LesserThanOrEqual));
             query.AddCriterion(Criterion.Create<DataPoint>(p => p.ModuleBelongTo.Id, 2, CriteriaOperator.Equal));
             //query.OrderByProperty = OrderByClause.Create<DataPoint>(p => p.Name, true);

             IEnumerable<DataPoint> result = dataPointRepository.FindBy(query);
         }

                  [Test()]
         public void TestFindByQueryAndCount()
         {
             AdoUnitOfWork unitOfWork = new AdoUnitOfWork();
             DataPointRepository dataPointRepository = new DataPointRepository(unitOfWork);

             Query query = new Query();
             query.AddCriterion(Criterion.Create<DataPoint>(p => p.Number, 10, CriteriaOperator.LesserThanOrEqual));
             query.AddCriterion(Criterion.Create<DataPoint>(p => p.ModuleBelongTo.Id, 2, CriteriaOperator.Equal));
             query.OrderByProperty = OrderByClause.Create<DataPoint>(p => p.Name, true);

             IEnumerable<DataPoint> result = dataPointRepository.FindBy(query,1,1);
         }
        
    }
}
