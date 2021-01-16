using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Infrastructure.Domain
{
    /// <summary>
    /// 所有的领域实体都继承这个类
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class EntityBase<TId>
    {
        private  List<BusinessRule> brokenRules = new List<BusinessRule>();
        public virtual List<BusinessRule> BrokenRules
        {
            get { return brokenRules; }
            set { brokenRules = value; }
        }

        public virtual TId Id { get; set; }

        /// <summary>
        /// 检测实体的有效性，
        /// 如果实体无效，用辅助方法AddBrokenRule（）将BusinessRule
        /// 添加到BrokenRules集合中。
        /// </summary>
        protected abstract void Validate();

        /// <summary>
        /// 在持久化实体前，要调用该方法是否用被破坏的规则。
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<BusinessRule> GetBrokenRules()
        {
            brokenRules.Clear();
            Validate();

            return brokenRules;
        }

        protected void AddBrokenRule(BusinessRule businessRule)
        {
            brokenRules.Add(businessRule);
        }
        public override bool Equals(object entity)
        {
            return entity !=null
                && entity is EntityBase<TId>
                && this == (EntityBase<TId>)entity;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(EntityBase<TId> entity1,
           EntityBase<TId> entity2)
        {
            if ((object)entity1 == null && (object)entity2 == null)
            {
                return true;
            }

            if ((object)entity1 == null || (object)entity2 == null)
            {
                return false;
            }

            if (entity1.Id.ToString() == entity2.Id.ToString())
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(EntityBase<TId> entity1,
            EntityBase<TId> entity2)
        {
            return (!(entity1 == entity2));
        }

    }
}
