using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Domain;

namespace NCS.Infrastructure.UnitOfWork
{
    /// <summary>
    /// UnitOfWork模式：
    /// 1.跟踪领域实体聚合的变化
    /// 2.原子操作中完成实体聚合的持久化
    /// 3.具体实现：
    /// 实体的进行持久化操作（包括三个操作：增Add、删remove、改save，注意不包括查询）前，
    /// 先用UnitOfWork进行登记，
    /// 以便日后由UnitOfWork统一（原子性）通过Commit操作，提交修改到数据库（持久化操作）
    /// </summary>
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 注册登记被添加的实体
        /// </summary>
        /// <param name="entity">目标实体</param>
        /// <param name="unitofWorkRepository">实体所在的仓储</param>
        void RegisterNew(IAggregateRoot entity, IUnitOfWorkRepository unitofWorkRepository);
        /// <summary>
        /// 注册登记被删除的实体
        /// </summary>
        /// <param name="entity">目标实体</param>
        /// <param name="unitofWorkRepository">实体所在的仓储</param>
        void RegisterRemoved(IAggregateRoot entity, IUnitOfWorkRepository unitofWorkRepository);
        /// <summary>
        /// 注册登记被修改的实体
        /// </summary>
        /// <param name="entity">目标实体</param>
        /// <param name="unitofWorkRepository">实体所在的仓储</param>
        void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository unitofWorkRepository);

        void Commit();
    }
}
