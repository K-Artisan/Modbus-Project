using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace UniversalDAL.MySql
{
    /// <summary>
    /// MySql数据库专用帮助类
    /// </summary>
    public static class MySqlDataBaseHelper
    {
        /// <summary>
        /// 执行MySql数据库的脚本文件
        /// </summary>
        /// <param name="sqlSqrictFilePath"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static bool ExecuteMySqlScriptFile(string sqlSqrictFilePath, string connectionString)
        {
            bool successful = true;

            if (!File.Exists(sqlSqrictFilePath))
            {
                return false;
            }

            MySqlConnection Connection = new MySqlConnection(connectionString);

            try
            {
                Connection.Open();

                try
                {
                    MySqlScript script = new MySqlScript(Connection);

                    FileInfo file = new FileInfo(sqlSqrictFilePath);
                    string sql = file.OpenText().ReadToEnd();
                    script.Query = sql;

                    script.Execute();
                }
                catch (Exception e)
                {
                    //successful = false;
                    throw new Exception("执行Sql脚本\r" + sqlSqrictFilePath + "\r时出现错误，配置数据库失败! 详细描述：\r" + e.Message);
                }
            }
            catch (Exception ex)
            {
                //successful = false;
                throw new Exception( ex.Message);
            }
            finally
            {
                successful = false;
                Connection.Close();
            }

            return successful;
        }
    }
}
