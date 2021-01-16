using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Domain;

namespace NCS.Model.Entity
{
    public class DataPointBusinessRule
    {
        public static readonly BusinessRule BasketRequired =
           new BusinessRule("ModuleId", "数据点的所属模块必须是存在的模块");
    }
}
