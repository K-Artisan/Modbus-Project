using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Infrastructure.Querying
{
    /// <summary>
    /// 确定如何将Criterion对象放在一起计算，例如：
    /// 在计算Name == "easy5" And Age >= 25中：
    /// 查询条件1（类型：Criterion）对应：Name == "easy5"：
    /// 查询条件2（类型：Criterion）对应：Age >= 25：
    /// 
    /// QueryOperator.And对应 And
    /// </summary>
    public enum QueryOperator
    {
        And,
        Or
    }
}
