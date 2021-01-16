using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Domain;

namespace NCS.Infrastructure.UnitOfWork
{
    /// <summary>
    /// Repository实现了UnitOfWork机制的Repository
    /// </summary>
    public interface IUnitOfWorkRepository
    {
        /// <summary>
        /// 实体的持久化操作（包括三个操作：增Add、删remove、改save，注意不包括查询）前，
        /// 先用UnitOfWork进行登记，
        /// 以便日后由UnitOfWork统一（原子性）通过Commit操作，提交修改到数据库（持久化操作）
        /// </summary>
        IUnitOfWork UnitOfWork { get; set; }

        /**
         * Persist持久化系列函数
         */
        void PersistCreationOf(IAggregateRoot entity);
        void PersistUpdateOf(IAggregateRoot entity);
        void PersistDeletionOf(IAggregateRoot entity);
    }
}
