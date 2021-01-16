using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Infrastructure.Querying
{
    /// <summary>
    /// 条件操作符
    /// </summary>
    public enum CriteriaOperator
    {
        Equal,                 //==
        NotApplicable,         //<>
        LessThan,              // <
        LesserThanOrEqual,     //<=
        GreaterThan,           // >
        GreaterThanOrEqual,    // >=
        Like,                  // ％
        //Between,
        //In,
        Not,
        IsNotNull,
        IsNull

        // TODO: Add the remainder of the criteria operators as required.
    }
}
