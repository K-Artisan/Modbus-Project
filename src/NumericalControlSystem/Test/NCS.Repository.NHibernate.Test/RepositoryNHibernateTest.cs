using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Querying;
using NCS.Model.Entity;
using NCS.Repository.NHibernate.Repositories;
using NUnit.Framework;

namespace NCS.Repository.NHibernate.Test
{
    [TestFixture]
    public class RepositoryNHibernateTest
    {
        [Test()]
        public void TestDatapiontRepositoryAddMethod()
        {
            NHUnitOfWork nhUnitOfWork = new NHUnitOfWork();
            DataPointRepository dataPointRepository = new DataPointRepository(nhUnitOfWork);

            Random seed = new Random();
            Random randomNum = new Random(seed.Next());

            DataPoint dataPoint = new DataPoint()
                {

                    Number = randomNum.Next(1000),
                    Name="35#",
                    StartRegisterAddress = 12,
                    DataType = DataType.Bit,
                    Description = "20130725测试！"
                };

            dataPointRepository.Add(dataPoint);
        }

        [Test()]
        public void TestDatapiontRepositoryFindByTEntityKeyMethod()
        {
            NHUnitOfWork nhUnitOfWork = new NHUnitOfWork();
            DataPointRepository dataPointRepository = new DataPointRepository(nhUnitOfWork);

            DataPoint dataPoint =  dataPointRepository.FindBy(14);
        }

        [Test()]
        public void TestDatapiontRepositorySavaMethod()
        {
            NHUnitOfWork nhUnitOfWork = new NHUnitOfWork();
            DataPointRepository dataPointRepository = new DataPointRepository(nhUnitOfWork);

            DataPoint dataPointPre = dataPointRepository.FindBy(1);
            dataPointPre.Description = "MyID is" + Convert.ToString((dataPointPre.Id));

            dataPointRepository.Save(dataPointPre);

            DataPoint dataPointAfter = dataPointRepository.FindBy(1);
            nhUnitOfWork.Commit();

            Assert.AreEqual(dataPointPre.Description, dataPointAfter.Description);
        }

        [Test()]
        public void TestDatapiontRepositoryUpdatAndCommitModthInUnitOfWork()
        {
            NHUnitOfWork nhUnitOfWork = new NHUnitOfWork();
            DataPointRepository dataPointRepository = new DataPointRepository(nhUnitOfWork);

            DataPoint dataPointPre = dataPointRepository.FindBy(1);
            dataPointPre.Description = "MyID is" + Convert.ToString((dataPointPre.Id));

            dataPointRepository.Save(dataPointPre);

            DataPoint dataPointAfter = dataPointRepository.FindBy(1);

            //如果不调用nhUnitOfWork.Commit（），
            //修改的数据还保存在ISession中，但是不会被保存到数据库
            //测试方法：
            //在Commit前后分别查看数据库的变化
            nhUnitOfWork.Commit();

            Assert.AreEqual(dataPointPre.Description, dataPointAfter.Description);
        }

        [Test()]
        public void TestDatapiontRepositoryRemoveMethod_RemovingEntityFromISessionButNotFormDataBase()
        {
            NHUnitOfWork nhUnitOfWork = new NHUnitOfWork();
            DataPointRepository dataPointRepository = new DataPointRepository(nhUnitOfWork);

            DataPoint dataPointPre = dataPointRepository.FindBy(15);
            if (dataPointPre != null)
            {
                dataPointRepository.Remove(dataPointPre);
            }

            DataPoint dataPointAfter = dataPointRepository.FindBy(15);

            Assert.IsNull(dataPointAfter);
        }

        public void TestDatapiontRepositoryRemoveMethod_BothRemovingEntityForISessionAndemoveFormDataBase()
        {
            NHUnitOfWork nhUnitOfWork = new NHUnitOfWork();
            DataPointRepository dataPointRepository = new DataPointRepository(nhUnitOfWork);

            DataPoint dataPointPre = dataPointRepository.FindBy(15);
            if (dataPointPre != null)
            {
                dataPointRepository.Remove(dataPointPre);
            }

            //如果不调用nhUnitOfWork.Commit（），就会：
            //只是在ISession中删除，但是没有在数据库中删除
            //测试方法：
            //在Commit前后分别查看数据库的变化
            nhUnitOfWork.Commit();

            DataPoint dataPointAfter = dataPointRepository.FindBy(15);

            Assert.IsNull(dataPointAfter);
        }

        [Test()]
        public void TestDatapiontRepositoryFindByQueryMethod()
        {
            NHUnitOfWork nhUnitOfWork = new NHUnitOfWork();
            DataPointRepository dataPointRepository = new DataPointRepository(nhUnitOfWork);

            Query query = new Query();
            query.AddCriterion(Criterion.Create<DataPoint>(p=>p.DataType, DataType.S32, CriteriaOperator.Equal));
            query.QueryOperator = QueryOperator.And;

            //query.OrderByProperty = new OrderByClause() { PropertyName = "Number" ,Desc = true};
            query.OrderByProperty = OrderByClause.Create<DataPoint>(p => p.Number, false);

            List<DataPoint> dataPoints = (List<DataPoint>)dataPointRepository.FindBy(query);

            Assert.NotNull(dataPoints);
        }


    }
}
