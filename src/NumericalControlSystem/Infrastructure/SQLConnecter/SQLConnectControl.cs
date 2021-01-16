using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SQLConnecter
{
    public class SQLConnectControl
    {
        private string dataBaseConfigFilePath = @"Config\DataBaseConfig.xml";

        /// <summary>
        /// 初始化得到连接到配置信息数据库中的连接信息
        /// </summary>
        /// <returns>连接配置数据库的连接信息</returns>
        public SQLConnModel InitConnectConfigDB()
        {
            SQLConnModel sqlConnModel = new SQLConnModel();

            string fileName = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, dataBaseConfigFilePath);
            if (System.IO.File.Exists(fileName))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fileName);

                //sqlConnection.DBIP = "localhost";
                sqlConnModel.DBIP = xmlDoc.SelectSingleNode("DBSetting/ConfigDBConnect/DBIP").InnerXml.ToLower();
                sqlConnModel.DBName = xmlDoc.SelectSingleNode("DBSetting/ConfigDBConnect/DBName").InnerXml.ToLower();
                sqlConnModel.DBUser = xmlDoc.SelectSingleNode("DBSetting/ConfigDBConnect/DBUser").InnerXml.ToLower();
                sqlConnModel.DBPasswd = xmlDoc.SelectSingleNode("DBSetting/ConfigDBConnect/DBPassWord").InnerXml;
            }
            
            return sqlConnModel;
        }

        /// <summary>
        /// 初始化得到连接到指定数据库的配置信息库的，连接信息
        /// </summary>
        /// <param name="ip">指定数据库的IP</param>
        /// <returns>连接配置数据库的连接信息</returns>
        public SQLConnModel InitConnectConfigDB(string ip)
        {
            SQLConnModel sqlConnModel = new SQLConnModel();

            string fileName = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, dataBaseConfigFilePath);
            if (System.IO.File.Exists(fileName))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fileName);

                sqlConnModel.DBIP = ip;
                //sqlConnection.DBIP = xmlDoc.SelectSingleNode("DBSetting/ConfigDBConnect/DBIP").InnerXml.ToLower();
                sqlConnModel.DBName = xmlDoc.SelectSingleNode("DBSetting/ConfigDBConnect/DBName").InnerXml.ToLower();
                sqlConnModel.DBUser = xmlDoc.SelectSingleNode("DBSetting/ConfigDBConnect/DBUser").InnerXml.ToLower();
                sqlConnModel.DBPasswd = xmlDoc.SelectSingleNode("DBSetting/ConfigDBConnect/DBPassWord").InnerXml;
            }

            return sqlConnModel;
        }

        public bool SaveConnectConfigDB(SQLConnModel sqlConnModel)
        {
            bool success = true;

            string fileName = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, dataBaseConfigFilePath);
            if (System.IO.File.Exists(fileName))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fileName);

                xmlDoc.SelectSingleNode("DBSetting/ConfigDBConnect/DBIP").InnerXml = sqlConnModel.DBIP;
                xmlDoc.SelectSingleNode("DBSetting/ConfigDBConnect/DBName").InnerXml = sqlConnModel.DBName;
                xmlDoc.SelectSingleNode("DBSetting/ConfigDBConnect/DBUser").InnerXml = sqlConnModel.DBUser;
                xmlDoc.SelectSingleNode("DBSetting/ConfigDBConnect/DBPassWord").InnerXml = sqlConnModel.DBPasswd;

                xmlDoc.Save(fileName);
            }
            else
            {
                success = false;
            }
            return success;
        }
    }
}
