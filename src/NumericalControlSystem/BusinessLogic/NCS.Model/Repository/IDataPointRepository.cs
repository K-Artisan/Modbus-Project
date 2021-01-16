using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NCS.Infrastructure.Domain;
using NCS.Model.Entity;

namespace NCS.Model.Repository
{
    public interface IDataPointRepository : IRepository<DataPoint, int>
    {
    }
}
