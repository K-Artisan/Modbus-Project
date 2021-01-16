using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NCS.Infrastructure.Domain;
using NCS.Infrastructure.UnitOfWork;
using NCS.Repository.NHibernate.SessionStorage;


namespace NCS.Repository.NHibernate
{

    /// <summary>
    /// </summary>
    public class NHUnitOfWork : IUnitOfWork
    {
        #region IUnitOfWork member

        /**
         * 1.NHibernate 的接口ISession（ISession x = SessionFactory.GetCurrentSession()；）
         * 实现了UnitOfWork模式，所以在实物提交前不可能与任何的修改。
         * 
         * 2.NHibernate内置另一个模式IdentityMap，负责在ISession中维护业务实体的唯一实例。
         */

        public void RegisterAmended(IAggregateRoot entity, 
            IUnitOfWorkRepository unitofWorkRepository)
        {
            unitofWorkRepository.PersistCreationOf(entity);
        }

        public void RegisterNew(IAggregateRoot entity, 
            IUnitOfWorkRepository unitofWorkRepository)
        {
           unitofWorkRepository.PersistUpdateOf(entity);             
        }

        public void RegisterRemoved(IAggregateRoot entity, 
            IUnitOfWorkRepository unitofWorkRepository)
        {
            unitofWorkRepository.PersistDeletionOf(entity);                         
        }

        public void Commit()
        {
            using (ITransaction transaction =
                SessionFactory.GetCurrentSession().BeginTransaction())
            {
                try
                {
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        #endregion
    }
}
