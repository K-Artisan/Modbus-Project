using NCS.Infrastructure.Domain;
using NCS.Infrastructure.Querying;
using NCS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;


namespace NCS.Repository.EF
{
    public abstract class RepositoryBase<T, TEntityKey> 
        : IUnitOfWorkRepository 
        where T : IAggregateRoot
    {
        private IUnitOfWork _unitOfWork;

        public RepositoryBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
            set { _unitOfWork = value; }
        }

        public void Add(T entity)
        {
            _unitOfWork.RegisterNew(entity, this);
        }

        public void Remove(T entity)
        {
            _unitOfWork.RegisterRemoved(entity, this);
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

            _unitOfWork.RegisterAmended(entity, this);
        }

        public T FindBy(TEntityKey id)
        {
            //return SessionFactory.GetCurrentSession().Get<T>(id);
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindAll()
        {
            //ICriteria criteriaQuery =
            //        SessionFactory.GetCurrentSession().CreateCriteria(typeof(T));

            //return (List<T>)criteriaQuery.List<T>();
            throw new NotImplementedException();

        }

        public IEnumerable<T> FindAll(int index, int count)
        {
            //ICriteria criteriaQuery =
            //          SessionFactory.GetCurrentSession().CreateCriteria(typeof(T));

            //return (List<T>)criteriaQuery.SetFetchSize(count)
            //                        .SetFirstResult(index).List<T>();
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindBy(Query query)
        {
            //ICriteria criteriaQuery =
            //         SessionFactory.GetCurrentSession().CreateCriteria(typeof(T));

            //AppendCriteria(criteriaQuery);

            //query.TranslateIntoNHQuery<T>(criteriaQuery);

            //return criteriaQuery.List<T>();
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindBy(Query query, int index, int count)
        {
            //ICriteria criteriaQuery =
            //         SessionFactory.GetCurrentSession().CreateCriteria(typeof(T));

            //AppendCriteria(criteriaQuery);

            //query.TranslateIntoNHQuery<T>(criteriaQuery);

            //return criteriaQuery.SetFetchSize(count).SetFirstResult(index).List<T>();
            throw new NotImplementedException();
        }


        #region IUnitOfWorkRepository

        public void PersistCreationOf(IAggregateRoot entity)
        {
            //DataContextFactory.GetDataContext().AddObject(GetEntitySetName(), entity);

        }

        public void PersistUpdateOf(IAggregateRoot entity)
        {
            // Do nothing as EF tracks changes
        }

        public void PersistDeletionOf(IAggregateRoot entity)
        {
            //DataContextFactory.GetDataContext().AddObject(GetEntitySetName(), entity);

        }

        #endregion
    }
}
