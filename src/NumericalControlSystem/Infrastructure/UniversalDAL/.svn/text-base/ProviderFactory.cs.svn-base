using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace UniversalDAL
{
    public class ProviderFactory
    {
        public static Dictionary<DbProviderType, string> providerInvariantName = new Dictionary<DbProviderType, string>();
        public static Dictionary<DbProviderType, DbProviderFactory> providerFactories = new Dictionary<DbProviderType, DbProviderFactory>();
        /// <summary>
        /// 静态构造函数（加载已知的数据库访问类的程序集）
        /// </summary>
        static ProviderFactory()
        {
            providerInvariantName.Add(DbProviderType.SqlServer, "System.Data.SqlClient");
            providerInvariantName.Add(DbProviderType.OleDb, "System.Data.OleDb");
            providerInvariantName.Add(DbProviderType.ODBC, "System.Data.ODBC");
            providerInvariantName.Add(DbProviderType.Oracle, "Oracle.DataAccess.Client");
            providerInvariantName.Add(DbProviderType.MySql, "MySql.Data.MySqlClient");
            providerInvariantName.Add(DbProviderType.SQLite, "System.Data.SQLite");
            providerInvariantName.Add(DbProviderType.Firebird, "FirebirdSql.Data.Firebird");
            providerInvariantName.Add(DbProviderType.PostgreSql, "Npgsql");
            providerInvariantName.Add(DbProviderType.DB2, "IBM.Data.DB2.iSeries");
            providerInvariantName.Add(DbProviderType.Informix, "IBM.Data.Informix");
            providerInvariantName.Add(DbProviderType.SqlServerCe, "System.Data.SqlServerCe");
        }
        /// <summary>
        /// 获取指定数据库类型对应的程序集名称
        /// </summary>
        /// <param name="dbProviderType">数据库类型名称</param>
        /// <returns>程序集名称</returns>
        public static string GetProviderInvariantName(DbProviderType dbProviderType)
        {
            return providerInvariantName[dbProviderType];
        }
        /// <summary>
        /// 加载指定数据库类型的DbProviderFactory
        /// </summary>
        /// <param name="dbProviderType">数据库类型名称</param>
        /// <returns>DbProviderFactory</returns>
        public static DbProviderFactory ImportDbProviderFactory(DbProviderType dbProviderType)
        {
            string providerName = GetProviderInvariantName(dbProviderType);
            DbProviderFactory factory = null;
            try
            {
                factory = DbProviderFactories.GetFactory(providerName);
            }
            catch (ArgumentException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return factory;
        }
        /// <summary>
        /// 获取指定数据库类型对应的DbProviderFactory
        /// </summary>
        /// <param name="dbProviderType"></param>
        /// <returns></returns>
        public static DbProviderFactory GetDbProviderFactory(DbProviderType dbProviderType)
        {
            if (!providerFactories.ContainsKey(dbProviderType))  //如果字典中不存在DbProviderFactory,则需要加载
            {
                providerFactories[dbProviderType] = ImportDbProviderFactory(dbProviderType);
            }
            return providerFactories[dbProviderType];
        }
    }
}
