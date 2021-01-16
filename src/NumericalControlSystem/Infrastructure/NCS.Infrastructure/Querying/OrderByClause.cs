using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NCS.Infrastructure.Querying
{
    /// <summary>
    /// 查询排序子句
    /// </summary>
    public class OrderByClause
    {
        /// <summary>
        /// 根据该属性进行排序
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// ture：降序
        /// false：升序
        /// </summary>
        public bool Descending { get; set; }

        /// <summary>
        /// 示例：
        ///  OrderByClause.Create<DataPoint>(p => p.Number, false);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public static OrderByClause Create<T>(Expression<Func<T, object>> expression, bool ascending)
        {
            string propertyName = PropertyNameHelper.ResolvePropertyName<T>(expression);
            OrderByClause orderByClause = new OrderByClause();
            orderByClause.PropertyName = propertyName;
            orderByClause.Descending = ascending;

            return orderByClause;
        }

    }
}
