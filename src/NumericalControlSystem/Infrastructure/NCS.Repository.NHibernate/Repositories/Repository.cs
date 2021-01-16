using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Domain;
using NCS.Infrastructure.Querying;
using NCS.Infrastructure.UnitOfWork;
using NCS.Repository.NHibernate.SessionStorage;
using NHibernate;

namespace NCS.Repository.NHibernate.Repositories
{
    public abstract class Repository<T, TEntityKey> : IUnitOfWorkRepository where T : IAggregateRoot
    {
        private IUnitOfWork _unitOfWork;

        public Repository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
            set { _unitOfWork = value; }
        }

        public void Add(T entity)
        {
            //using (ITransaction transaction = SessionFactory.GetCurrentSession().BeginTransaction())
            //{
            //    SessionFactory.GetCurrentSession().Save(entity);
            //    transaction.Commit();
            //}

            UnitOfWork.RegisterNew(entity, this);
        }

        public void Remove(T entity)
        {
            //using (ITransaction transaction = SessionFactory.GetCurrentSession().BeginTransaction())
            //{
            //    SessionFactory.GetCurrentSession().Delete(entity);
            //    transaction.Commit();
            //}

            UnitOfWork.RegisterRemoved(entity, this);
        }

        public void Remove(Query query)
        {
            
        }

        public void Save(T entity)
        {
            //using (ITransaction transaction = SessionFactory.GetCurrentSession().BeginTransaction())
            //{
            //    //SaveOrUpdate并没有立马保存到数据库
            //    SessionFactory.GetCurrentSession().SaveOrUpdate(entity);

            //    //Commit才提交到数据库
            //    transaction.Commit();
            //}

            UnitOfWork.RegisterAmended(entity, this);
        }

        public T FindBy(TEntityKey id)
        {
            return SessionFactory.GetCurrentSession().Get<T>(id);
        }

        public IEnumerable<T> FindAll()
        {
            ICriteria criteriaQuery =
                    SessionFactory.GetCurrentSession().CreateCriteria(typeof(T));

            return (List<T>)criteriaQuery.List<T>();
        }

        public IEnumerable<T> FindAll(int index, int count)
        {
            ICriteria criteriaQuery =
                      SessionFactory.GetCurrentSession().CreateCriteria(typeof(T));

            return (List<T>)criteriaQuery.SetFetchSize(count)
                                    .SetFirstResult(index).List<T>();
        }

        public IEnumerable<T> FindBy(Query query)
        {
            ICriteria criteriaQuery =
                     SessionFactory.GetCurrentSession().CreateCriteria(typeof(T));

            AppendCriteria(criteriaQuery);

            query.TranslateIntoNHQuery<T>(criteriaQuery);

            return criteriaQuery.List<T>();
        }

        public IEnumerable<T> FindBy(Query query, int index, int count)
        {
            ICriteria criteriaQuery =
                     SessionFactory.GetCurrentSession().CreateCriteria(typeof(T));

            AppendCriteria(criteriaQuery);

            query.TranslateIntoNHQuery<T>(criteriaQuery);

            return criteriaQuery.SetFetchSize(count).SetFirstResult(index).List<T>();
        }

        public virtual void AppendCriteria(ICriteria criteria)
        {

        }

        #region IUnitOfWorkRepository

        public void PersistCreationOf(IAggregateRoot entity)
        {
            SessionFactory.GetCurrentSession().Save(entity);
        }

        public void PersistUpdateOf(IAggregateRoot entity)
        {
            SessionFactory.GetCurrentSession().SaveOrUpdate(entity);
        }

        public void PersistDeletionOf(IAggregateRoot entity)
        {
            SessionFactory.GetCurrentSession().Delete(entity);
        }

        #endregion
    }
}
