using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Domain;
using NCS.Infrastructure.Querying;
using NCS.Infrastructure.UnitOfWork;
using NCS.Repository.ADO.DataSession;

namespace NCS.Repository.ADO.Repositories
{
    public abstract class Repository<T, TEntityKey> : IUnitOfWorkRepository
        where T : IAggregateRoot
    {
        private IUnitOfWork unitOfWork;

        private IDataSession<T, TEntityKey> dataSession;

        public Repository(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
            DataSession = DataSessionFactory.GetDataSession<T, TEntityKey>();
        }

        public IUnitOfWork UnitOfWork
        {
            get { return unitOfWork; }
            set { unitOfWork = value; }
        }

        public IDataSession<T, TEntityKey> DataSession
        {
            get { return dataSession; }
            set { dataSession = value; }
        }

        #region 持久化

        public void Add(T entity)
        {
            UnitOfWork.RegisterNew(entity, this);
        }

        public void Remove(T entity)
        {
            UnitOfWork.RegisterNew(entity, this);
        }

        public void Remove(Query query)
        {
            //TODO:好像Remove(Query query)并不能保证事务操作，因为没添加到UnitOfWork里面
            //先提交事务
            unitOfWork.Commit();

            DataSession = DataSessionFactory.GetDataSession<T, TEntityKey>();
            DataSession.Remove(query);

        }

        public void Save(T entity)
        {
            UnitOfWork.RegisterRemoved(entity, this);
        }

        #endregion

        #region 查询部分

        public T FindBy(TEntityKey id)
        {
            return this.DataSession.FindBy(id);
        }

        public IEnumerable<T> FindAll()
        {
            return this.DataSession.FindAll();
        }

        public IEnumerable<T> FindAll(int index, int count)
        {
            return this.DataSession.FindAll().Skip(index).Take(count);
        }

        public IEnumerable<T> FindBy(Query query)
        {
            return this.DataSession.FindBy(query);
        }

        public IEnumerable<T> FindBy(Query query, int index, int count)
        {
            return this.DataSession.FindBy(query, index, count);
        }

        #endregion

        #region IUnitOfWorkRepository members

        public virtual void PersistCreationOf(IAggregateRoot entity)
        {
            this.DataSession.Add((T)entity);
        }

        public virtual void PersistUpdateOf(IAggregateRoot entity)
        {
            this.DataSession.Save((T)entity);
        }

        public virtual void PersistDeletionOf(IAggregateRoot entity)
        {
            this.DataSession.Remove((T)entity);
        }

        #endregion

    }
}
