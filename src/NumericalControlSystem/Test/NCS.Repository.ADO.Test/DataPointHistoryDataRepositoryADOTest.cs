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
    public class DataPointHistoryDataRepositoryAdoTest
    {
         [Test()]
         public void CanAddDataPointHistoryData()
         {
             AdoUnitOfWork unitOfWork = new AdoUnitOfWork();
             DataPointHistoryDataRepository dataPointHistoryDataRepository = new DataPointHistoryDataRepository(unitOfWork);

             DataPointHistoryData dataPointHistoryData = new DataPointHistoryData()
             {
                 Id = Guid.NewGuid().ToString("D"),
                 DataPoint = new DataPoint() { Id = 1726 },
                 DateTime = DateTime.Now.AddDays(8),
                 Value = 123456789.12345  
             };

             dataPointHistoryDataRepository.Add(dataPointHistoryData);
             unitOfWork.Commit();
         }

         [Test()]
         public void TestFindById()
         {
         }

         [Test()]
         public void TestFindByQuery()
         {
             AdoUnitOfWork unitOfWork = new AdoUnitOfWork();
             DataPointHistoryDataRepository dataPointHistoryDataRepository = new DataPointHistoryDataRepository(unitOfWork);

             Query query = new Query();
             query.AddCriterion(Criterion.Create<DataPointHistoryData>(p => p.DataPoint.Id, 1, CriteriaOperator.Equal));
             query.AddCriterion(Criterion.Create<DataPointHistoryData>(p => p.DateTime, new DateTime(2013,5,1), CriteriaOperator.GreaterThanOrEqual));
             query.AddCriterion(Criterion.Create<DataPointHistoryData>(p => p.DateTime, DateTime.Now, CriteriaOperator.LesserThanOrEqual));
             query.QueryOperator = QueryOperator.And;
             query.OrderByProperty = OrderByClause.Create<DataPointHistoryData>(p => p.DateTime, false);

             IEnumerable<DataPointHistoryData> result = dataPointHistoryDataRepository.FindBy(query);

             Assert.IsTrue(result.Count()>0);
         }

                  [Test()]
         public void TestFindByQueryAndCount()
         {
             //AdoUnitOfWork unitOfWork = new AdoUnitOfWork();
             //DataPointRepository dataPointRepository = new DataPointRepository(unitOfWork);

             //Query query = new Query();
             //query.AddCriterion(Criterion.Create<DataPoint>(p => p.Number, 10, CriteriaOperator.LesserThanOrEqual));
             //query.AddCriterion(Criterion.Create<DataPoint>(p => p.ModuleBelongTo.Id, 2, CriteriaOperator.Equal));
             //query.OrderByProperty = OrderByClause.Create<DataPoint>(p => p.Name, true);

             //IEnumerable<DataPoint> result = dataPointRepository.FindBy(query,1,1);
         }
        
    }
}
