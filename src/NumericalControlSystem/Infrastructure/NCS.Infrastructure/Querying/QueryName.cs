using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Infrastructure.Querying
{
    /// <summary>
    /// 有时复制的查询对象难以构建，
    /// 我们会经由过程存储过程或视图来处理此种景象，
    /// 须要构建一个列举QueryName用来指导是存储过程（视图）还是构建动态的sql语句
    /// </summary>
    public enum QueryName
    {
        Dynamic = 0,                             //动态创建
        RetrieveOrdersUsingAComplexQuery = 1     //应用已经创建好了的存储过程、视图、这是查询比较错杂时应用存储过程
    }
}
