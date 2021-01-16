using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Infrastructure.Domain
{
    /// <summary>
    /// 业务规则类
    /// 存放规则和相关实体属性
    /// 用于检测领域实体的有效性
    /// </summary>
    public class BusinessRule
    {
        public string Property { get; set; }
        public string Rule { get; set; }

        public BusinessRule(string property, string rule)
        {
            Property = property;
            Rule = rule;
        }
    }
}
