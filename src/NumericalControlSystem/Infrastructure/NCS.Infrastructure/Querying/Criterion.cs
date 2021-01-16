using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NCS.Infrastructure.Querying
{
    /// <summary>
    /// 查询条件,例如：
    /// 查询条件Criterion对应：Name == "easy5"：
    /// Name对应Criterion.PropertyName
    /// "easy5"对应Criterion.Value
    /// 
    /// == 对应QueryOperator.CriteriaOperator
    /// </summary>
    public class Criterion
    {
        private string propertyName;
        public string PropertyName
        {
            get { return propertyName; }
        }

        private object value;
        public object Value
        {
            get { return value; }
        }

        private CriteriaOperator criteriaOperator;
        public CriteriaOperator CriteriaOperator
        {
            get { return criteriaOperator; }
        }

        public Criterion(string propertyName, object value, CriteriaOperator criteriaOperator)
        {
            this.propertyName = propertyName;
            this.value = value;
            this.criteriaOperator = criteriaOperator;
        }

        /// <summary>
        /// 创建Criterion对象,示例1:
        /// Criterion.Create<Product>(p=>p.Color.Id, id, CriteriaOperator.Equal);
        /// 
        /// 示例2：类对象中的另一个类对象的属性创建Criterion，
        /// 例如DataPoint的一个属性ModuleBelongTo的类型是Module，而Module有int类型的Id属性，
        /// 即类的结构如下：
        /// DataPoint datapoint = new DataPoint();
        /// datapoint.ModuleBelongTo = new Module();
        /// 
        /// 用datapoint.ModuleBelongTo.Id创建Criterion
        /// Criterion.Create<DataPoint>(p => p.ModuleBelongTo.Id, 2, CriteriaOperator.Equal)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <param name="criteriaOperator"></param>
        /// <returns></returns>
        public static Criterion Create<T>(Expression<Func<T, object>> expression, object value, CriteriaOperator criteriaOperator)
        {
            string propertyName = PropertyNameHelper.ResolvePropertyName<T>(expression);
            Criterion criterion = new Criterion(propertyName, value, criteriaOperator);

            return criterion;
        }
    }
}
