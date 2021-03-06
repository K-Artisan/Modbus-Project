﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;

namespace NCS.Repository.NHibernate.SessionStorage
{
    public class SessionFactory
    {
        private static ISessionFactory _sessionFactory;

        private static void Init()
        {
            Configuration config = new Configuration();
            config.AddAssembly("NCS.Repository.NHibernate");

            //log4net.Config.XmlConfigurator.Configure();

            config.Configure();

            _sessionFactory = config.BuildSessionFactory();
        }

        private static ISessionFactory GetSessionFactory()
        {
            if (_sessionFactory == null)
                Init();

            return _sessionFactory;
        }

        public static ISession GetCurrentSession()
        {
            ISessionStorageContainer sessionStorageContainer =
                                       SessionStorageFactory.GetStorageContainer();

            ISession currentSession = sessionStorageContainer.GetCurrentSession();

            if (currentSession == null)
            {
                currentSession = GetNewSession();
                sessionStorageContainer.Store(currentSession);
            }

            return currentSession;
        }

        private static ISession GetNewSession()
        {
            return GetSessionFactory().OpenSession();
        }
    }
}
