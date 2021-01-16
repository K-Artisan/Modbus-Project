using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLConnecter;

namespace UniversalDAL
{
    public static class DbUtilityCreator
    {
        private static DbUtility dbUtility;

        public static DbUtility GetDefaultDbUtility()
        {
            if (null == dbUtility)
            {
                SQLConnectControl sqlConnectControl = new SQLConnectControl();
                SQLConnModel sqlConnModel = new SQLConnModel();

                sqlConnModel = sqlConnectControl.InitConnectConfigDB();
                dbUtility = new DbUtility(sqlConnModel, DbProviderType.MySql);
            }

            return dbUtility;
        }

        public static DbUtility GetDbUtility(DbProviderType dbProviderType)
        {
            if (null == dbUtility)
            {
                SQLConnectControl sqlConnectControl = new SQLConnectControl();
                SQLConnModel sqlConn = new SQLConnModel();

                sqlConn = sqlConnectControl.InitConnectConfigDB();
                dbUtility = new DbUtility(sqlConn, dbProviderType);
            }

            return dbUtility;
        }
    }
}
