using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using SQLConnecter;

namespace UniversalDAL
{
    public class DbUtility
    {
        public string ConnectionString { get; private set; }
        private DbProviderFactory providerFactory;

        public SQLConnModel SqlconModel { get; private set; }
        public DbProviderType DbProviderType { get; private set; }

        //public DbUtility(string connectionString, DbProviderType dbProviderType)
        //{
        //    ConnectionString = connectionString;
        //    providerFactory = ProviderFactory.GetDbProviderFactory(dbProviderType);
        //    if (providerFactory == null)
        //    {
        //        throw new ArgumentException("Can't load DbProviderFactory for given value of providerType");
        //    }
        //}

        public DbUtility(SQLConnModel sqlconModel, DbProviderType dbProviderType)
        {
            this.SqlconModel = sqlconModel;
            this.DbProviderType = dbProviderType;

            string conStr = null;
            switch (dbProviderType)
            {
                case DbProviderType.MySql:
                    conStr = "server = "+sqlconModel.DBIP+";uid = "+sqlconModel.DBUser+";pwd = "+sqlconModel.DBPasswd+";database = "+sqlconModel.DBName;
                    ConnectionString = conStr;
                    providerFactory = ProviderFactory.GetDbProviderFactory(dbProviderType);

                    if (providerFactory == null)
                    {
                        throw new ArgumentException("Can't load DbProviderFactory for given value of providerType");
                    }
                    break;
                case DbProviderType.SqlServer:
                    conStr = "Data Source= " + sqlconModel.DBIP + "; Initial Catalog = " + sqlconModel.DBName +
                                            "; User ID = " + sqlconModel.DBUser + "; Password = " + sqlconModel.DBPasswd;
                    ConnectionString = conStr;
                    providerFactory = ProviderFactory.GetDbProviderFactory(dbProviderType);
                    if (providerFactory == null)
                    {
                        throw new ArgumentException("Can't load DbProviderFactory for given value of providerType");
                    }
                    break;
            }
        }

        /// <summary>
        /// 测试打开数据库是否正常
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public bool TestConnection()
        {
            bool res = false;
            DbConnection connection = providerFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            try
            {
                connection.Open();
                res = true;
            }
            catch
            {
                res = false;
            }

            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return res;
        }

        /// <summary>
        /// 创建Command对象
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <param name="parameters">执行的SQL语句所需要的参数</param>
        /// <returns>Command对象</returns>
       private DbCommand CreateDbCommand(string sql, CommandType commandType, IList<DbParameter> parameters)
        {
            DbConnection connection = providerFactory.CreateConnection();           
            DbCommand command = providerFactory.CreateCommand();
            connection.ConnectionString = ConnectionString;
            OpenConnection(connection);
            command.Connection = connection;
            command.CommandText = sql;
            command.CommandType = commandType;
            if (!(parameters == null || parameters.Count == 0))
            {
                foreach (DbParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }
        /// <summary>
        /// 打开连接
        /// </summary>
        /// <param name="connection"></param>
        private void OpenConnection(DbConnection connection)
        {
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 对数据库进行增删改操作，返回受影响的行数
        /// </summary>
        /// <param name="sql">执行增删改操作的SQL语句</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, CommandType.Text, null);
        }
        /// <summary>
        /// 对数据库进行增删改操作，返回受影响的行数
        /// </summary>
        /// <param name="sql">执行增删改操作的SQL语句</param>
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string sql, CommandType commandType)
        {
            return ExecuteNonQuery(sql, commandType, null);
        }
        /// <summary>
        /// 对数据库进行增删改操作，返回受影响的行数
        /// </summary>
        /// <param name="sql">执行增删改操作的SQL语句</param>
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <param name="parameters">所需要的参数</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string sql, CommandType commandType, IList<DbParameter> parameters)
        {
            using (DbCommand command = CreateDbCommand(sql, commandType, parameters))
            {
                int affectedRows = command.ExecuteNonQuery();
                command.Connection.Close();
                command.Connection.Dispose();
                return affectedRows;
            }
        }
        /// <summary>
        /// 执行一条查询语句，返回一个DataReader实例
        /// </summary>
        /// <param name="sql">要执行的查询语句</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string sql)
        {
            return ExecuteReader(sql, CommandType.Text, null);
        }
        /// <summary>
        /// 执行一条查询语句，返回一个DataReader实例
        /// </summary>
        /// <param name="sql">要执行的查询语句</param>
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string sql, CommandType commandType)
        {
            return ExecuteReader(sql, commandType, null);
        }
        /// <summary>
        /// 执行一条查询语句，返回一个DataReader实例
        /// </summary>
        /// <param name="sql">要执行的查询语句</param>
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <param name="parameters">所需要的参数</param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string sql, CommandType commandType, IList<DbParameter> parameters)
        {
            DbCommand command = CreateDbCommand(sql, commandType, parameters);
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        /// <summary>
        /// 执行一条查询语句，返回一个包含查询结果的DataTable
        /// </summary>
        /// <param name="sql">要执行的查询语句</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql)
        {
            return ExecuteDataTable(sql, CommandType.Text, null);
        }
        /// <summary>
        /// 执行一条查询语句，返回一个包含查询结果的DataTable
        /// </summary>
        /// <param name="sql">要执行的查询语句</param>
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql, CommandType commandType)
        {
            return ExecuteDataTable(sql, commandType, null);
        }
        /// <summary>
        /// 执行一条查询语句，返回一个包含查询结果的DataTable
        /// </summary>
        /// <param name="sql">要执行的查询语句</param>
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <param name="parameters">所需要的参数</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql, CommandType commandType, IList<DbParameter> parameters)
        {
            using (DbCommand command = CreateDbCommand(sql, commandType, parameters))
            {
                using (DbDataAdapter adapter = providerFactory.CreateDataAdapter())
                {
                    adapter.SelectCommand = command;
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
        /// <summary>
        /// 执行一条查询语句，返回查询结果的首行首列
        /// </summary>
        /// <param name="sql">要执行的查询语句</param>
        /// <returns></returns>
        public Object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, CommandType.Text, null);
        }
        /// <summary>
        /// 执行一条查询语句，返回查询结果的首行首列
        /// </summary>
        /// <param name="sql">要执行的查询语句</param>
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <returns></returns>
        public Object ExecuteScalar(string sql, CommandType commandType)
        {
            return ExecuteScalar(sql, commandType, null);
        }
        /// <summary>
        /// 执行一条查询语句，返回查询结果的首行首列
        /// </summary>
        /// <param name="sql">要执行的查询语句</param>
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <param name="parameters">所需要的参数</param>
        /// <returns></returns>
        public Object ExecuteScalar(string sql, CommandType commandType, IList<DbParameter> parameters)
        {
            using (DbCommand command = CreateDbCommand(sql, commandType, parameters))
            {
                object result = command.ExecuteScalar();
                command.Connection.Close();
                return result;
            }
        }

        #region 创建一个数据库连接
        public DbConnection CreateDbConnection()
        {
            DbConnection connection = providerFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            OpenConnection(connection);
            return connection;
        }
        #endregion

        #region 创建一个数据库Command
        public DbCommand CreateDbCommand(DbConnection conn)
        {
           DbCommand cmd = conn.CreateCommand();
           //cmd.Connection = conn;
           return cmd;
        }
        #endregion

        #region 执行一条查询语句，返回查询结果的首行首列
        /// <summary>
        /// 执行一条查询语句，返回查询结果的首行首列
        /// </summary>
        /// <param name="sql">要执行的查询语句</param>
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <param name="parameters">所需要的参数</param>
        /// <returns></returns>
        public Object ExecuteScalar(DbCommand cmd, string sql, CommandType commandType, IList<DbParameter> parameters)
        {
            using (cmd)
            {
                cmd.CommandText = sql;
                cmd.CommandType = commandType;
                if (!(parameters == null || parameters.Count == 0))
                {
                    cmd.Parameters.Clear();
                    foreach (DbParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                object result = cmd.ExecuteScalar();
                //cmd.Connection.Close();
                return result;
            }
        }
        #endregion

        #region 对数据库进行增删改操作，返回受影响的行数
        /// <summary>
        /// 对数据库进行增删改操作，返回受影响的行数
        /// </summary>
        /// <param name="sql">执行增删改操作的SQL语句</param>
        /// <param name="commandType">执行的SQL语句的类型</param>
        /// <param name="parameters">所需要的参数</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(DbCommand cmd,string sql, CommandType commandType, IList<DbParameter> parameters)
        {
            using (cmd)
            {
                cmd.CommandText = sql;
                cmd.CommandType = commandType;
                if (!(parameters == null || parameters.Count == 0))
                {
                    cmd.Parameters.Clear();
                    foreach (DbParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                int affectedRows = cmd.ExecuteNonQuery();
                //cmd.Connection.Close();
                return affectedRows;
            }
        }
        #endregion
    }
}
