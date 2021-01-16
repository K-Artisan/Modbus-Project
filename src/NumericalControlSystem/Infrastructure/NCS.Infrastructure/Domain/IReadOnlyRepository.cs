using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Querying;

namespace NCS.Infrastructure.Domain
{
    /// <summary>
    /// 只能读取的实体
    /// 
    /// 只可以为聚合根定义Repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IReadOnlyRepository<T, TId> where T:IAggregateRoot
    {
        T FindBy(TId id);
        IEnumerable<T> FindAll();
        IEnumerable<T> FindBy(Query query);
        IEnumerable<T> FindBy(Query query, int index, int count);
    }
}
