using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Querying;

namespace NCS.Repository.ADO.DataSession
{
    public interface IDataSession<T,TEntityKey>
    {
        void Add(T entity);

        void Remove(T entity);
        void Remove(Query query);

        void Save(T entity);

        T FindBy(TEntityKey id);
        IEnumerable<T> FindAll();
        IEnumerable<T> FindBy(Query query);
        IEnumerable<T> FindBy(Query query, int index, int count);
    }
}
