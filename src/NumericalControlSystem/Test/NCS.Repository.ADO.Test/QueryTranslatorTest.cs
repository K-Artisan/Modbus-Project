using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Querying;

using NCS.Repository.ADO.Repositories;
using NUnit.Framework;
using NCS.Model.Entity;

namespace NCS.Repository.ADO.Test
{
    [TestFixture]
    public class QueryTranslatorTest
    {
        [Test()]
        public void TestCreatQuery()
        {
            Query query = new Query();
            query.AddCriterion(Criterion.Create<DataPoint>(p=>p.Number, 10, CriteriaOperator.LesserThanOrEqual));
            query.AddCriterion(Criterion.Create<DataPoint>(p => p.ModuleBelongTo.Id, 2, CriteriaOperator.Equal));
            query.OrderByProperty = OrderByClause.Create<DataPoint>(p =>p.Name, true);

        }
    }
}
