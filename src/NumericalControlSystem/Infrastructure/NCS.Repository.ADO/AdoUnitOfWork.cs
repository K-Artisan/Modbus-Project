/*一点牢骚：
 * UnitOfWork机制的蛋疼之处：
 *    UnitOfWork机制，决定了插入新的实体前，要预先设置数据库中的主键Id，尽管数据库自己生产主键。
 * 但是，如果自己能生成主键还要数据库自动生成主键干什么，即使自己生成主键不能保证主键的唯一性，
 * 除非主键是GUID。
 *    
 *               if (!addedEntities.ContainsKey(entity))
                {
                    addedEntities.Add(entity, unitofWorkRepository);
                }; 
 * 判断实体的唯一性标准是调用实体的GetHashCode（）；
 *      public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
 *而 this.Id是实体在数据库的主键（一般用数据库自动生成），但我插入前怎么能由我生成呢！！！
 *因为：
 *1.不知主键的数据库类型；
 *2.即使知道主键的数据库类型，也不能因为硬编码造成的依赖，耦合。
 *3.人工生成不能保证主键的唯一性，
 *综上，所以就不能添加到addedEntities中，也就不能保存到数据库。
 *折中的解决方案是每添加一个新的实体，就Commit一次（马上提交到数据库，并清空addedEntities）。
 *  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Domain;
using NCS.Infrastructure.UnitOfWork;
using System.Transactions;

namespace NCS.Repository.ADO
{
    public class AdoUnitOfWork : IUnitOfWork
    {
        private Dictionary<IAggregateRoot, IUnitOfWorkRepository> addedEntities;
        private Dictionary<IAggregateRoot, IUnitOfWorkRepository> changedEntities;
        private Dictionary<IAggregateRoot, IUnitOfWorkRepository> deletedEntities;

        public AdoUnitOfWork()
        {
            addedEntities = new Dictionary<IAggregateRoot, IUnitOfWorkRepository>();
            changedEntities = new Dictionary<IAggregateRoot, IUnitOfWorkRepository>();
            deletedEntities = new Dictionary<IAggregateRoot, IUnitOfWorkRepository>();
        }

        #region IUnitOfWork member

        #region 注册登记实体
        public void RegisterNew(IAggregateRoot entity,
                                IUnitOfWorkRepository unitofWorkRepository)
        {
            if (!addedEntities.ContainsKey(entity))
            {
                addedEntities.Add(entity, unitofWorkRepository);
            }; 
        }

        public void RegisterRemoved(IAggregateRoot entity,
                                    IUnitOfWorkRepository unitofWorkRepository)
        {
            if (!deletedEntities.ContainsKey(entity))
            {
                deletedEntities.Add(entity, unitofWorkRepository);
            }
        }

        public void RegisterAmended(IAggregateRoot entity,
                            IUnitOfWorkRepository unitofWorkRepository)
        {
            if (!changedEntities.ContainsKey(entity))
            {
                changedEntities.Add(entity, unitofWorkRepository);
            }
        }

        #endregion
        public void Commit()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (IAggregateRoot entity in this.addedEntities.Keys)
                {
                    this.addedEntities[entity].PersistCreationOf(entity);
                }

                foreach (IAggregateRoot entity in this.changedEntities.Keys)
                {
                    this.changedEntities[entity].PersistUpdateOf(entity);
                }

                foreach (IAggregateRoot entity in this.deletedEntities.Keys)
                {
                    this.deletedEntities[entity].PersistDeletionOf(entity);
                }

                scope.Complete();

                this.addedEntities.Clear();
                this.changedEntities.Clear();
                this.deletedEntities.Clear();
            }
        }

        #endregion

    }
}
