using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NHibernate;

namespace NCS.Repository.NHibernate.SessionStorage
{
    /// <summary>
    /// 非Web场景的智能客户端版本的StorageContainer
    /// </summary>
    public class ThreadSessionStorageContainer : ISessionStorageContainer
    {
        private static readonly Hashtable _nhSessions = new Hashtable();

        public ISession GetCurrentSession()
        {
            ISession nhSession = null;

            //当前线程作为主键
            if (_nhSessions.Contains(GetThreadId()))
                nhSession = (ISession)_nhSessions[GetThreadId()];

            return nhSession;
        }

        public void Store(ISession session)
        {
            if (_nhSessions.Contains(GetThreadId()))
                _nhSessions[GetThreadId()] = session;
            else
                _nhSessions.Add(GetThreadId(), session);
        }

        private static string GetThreadName()
        {
            return Thread.CurrentThread.Name;
        }

        private static int GetThreadId()
        {
            return Thread.CurrentThread.ManagedThreadId;
        }
    }
}
