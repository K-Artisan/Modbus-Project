using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Querying;

namespace NCS.Infrastructure.Domain
{
    /// <summary>
    /// 既可以读取也可以持久化的实体
    /// 
    /// 只可以为聚合根定义Repository
    /// </summary>
    /// <typeparam name="T">实体的类型</typeparam>
    /// <typeparam name="TId">实体标示Id的类型</typeparam>
    public interface IRepository<T, TId> :IReadOnlyRepository<T, TId> 
        where T : IAggregateRoot
    {
        void Save(T entity);
        void Add(T entity);

        void Remove(T entity);
        void Remove(Query query);
    }
}
