/*使用范例：
 *   Query query = new Query();
 *   query.AddCriterion(Criterion.Create<DataPointHistoryData>(p => p.DataPoint.Id, 1, CriteriaOperator.Equal));
 *   query.AddCriterion(Criterion.Create<DataPointHistoryData>(p => p.DateTime, new DateTime(2013,5,1), CriteriaOperator.GreaterThanOrEqual));
 *   query.AddCriterion(Criterion.Create<DataPointHistoryData>(p => p.DateTime, DateTime.Now, CriteriaOperator.LesserThanOrEqual));
 *   query.QueryOperator = QueryOperator.And;
 *   query.OrderByProperty = OrderByClause.Create<DataPointHistoryData>(p => p.DateTime, false);
 *   
 *   IEnumerable<DataPointHistoryData> result = dataPointHistoryDataRepository.FindBy(query);
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Infrastructure.Querying
{
    /// <summary>
    /// 将Query Object模式组合在一起。
    /// 目前这个Query的局限是：
    /// 所有的查询条件只能只有一种计算方式，即：
    ///  支持：Name == "easy5" And Age >= 25  And Name == "123"
    ///  不支持：Name == "easy5" And Age >= 25  Or Name == "123"
    /// 
    /// </summary>
    public class Query
    {
        private QueryName name;
        public QueryName Name
        {
            get { return name; }
            set { name = value; }
        }
        /// <summary>
        /// 子查询集合
        /// </summary>
        private IList<Query> subQueries = new List<Query>();
        public IEnumerable<Query> SubQueries
        {
            get { return subQueries; }
        }

        /// <summary>
        /// 查询条件Criterion集合
        /// </summary>
        private IList<Criterion> _criterias = new List<Criterion>();
        public IEnumerable<Criterion> Criterias
        {
            get { return _criterias; }
        }

        /// <summary>
        /// Criterion的计算方式,例如：
        /// Name == "easy5" And Age >= 25
        /// </summary>
        public QueryOperator QueryOperator { get; set; }
        public OrderByClause OrderByProperty { get; set; }

        public void AddSubQuery(Query subQuery)
        {
            subQueries.Add(subQuery);
        }

        public void AddCriterion(Criterion criterion)
        {
            if (!IsNamedQuery())
                _criterias.Add(criterion);
            else
                throw new ApplicationException("You cannot add additional criteria to named queries");
        }

        public bool IsNamedQuery()
        {
            return Name != QueryName.Dynamic;
        }
    }
}
