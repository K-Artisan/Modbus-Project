using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Logging;
using NCS.Service.Messaging.DataBaseConfigService;
using NCS.Service.ServiceInterface;
using SQLConnecter;
using UniversalDAL;
using UniversalDAL.MySql;

namespace NCS.Service.SeviceImplementation
{
    public class DataBaseConfigService : IDataBaseConfigService
    {
        private SQLConnModel sqlConnModel = null;

        #region IDataBaseConfigService members

        public CreateDataBaseResponse CreateDataBase(CreateDataBaseRequest request)
        {
            CreateDataBaseResponse response = new CreateDataBaseResponse();

            if (null != request)
            {
                try
                {
                    if (request.SqlScriptFiles.Count > 0)
                    {
                        CreataDataSqlScript(request.SqlScriptFiles);
                    }
                    else
                    {
                        response.ResponseSucceed = false;
                        response.Message = "无脚本文件";
                    }
                }
                catch (Exception ex)
                {
                    string message = "操作失败。" + ex.Message;
                    response.ResponseSucceed = false;
                    response.Message = message;
                    LoggingFactory.GetLogger().WriteDebugLogger(message);
                }
            }

            return response;
        }

        public ExecuteSqlScriptResponse ExecuteSqlScript(ExecuteSqlScriptRequest request)
        {
            ExecuteSqlScriptResponse response = new ExecuteSqlScriptResponse();

            if (null != request)
            {
                try
                {
                    if (request.SqlScriptFiles.Count > 0)
                    {
                        ExecuteSqlScript(request.SqlScriptFiles);
                    }
                    else
                    {
                        response.ResponseSucceed = false;
                        response.Message = "无脚本文件";
                    }
                }
                catch (Exception ex)
                {
                    string message = "操作失败。" + ex.Message;
                    response.ResponseSucceed = false;
                    response.Message = message;
                    LoggingFactory.GetLogger().WriteDebugLogger(message);
                }
            }

            return response;
        }

        public GetCurrentDataBaseLoginInfoResponse GetCurrentDataBaseLoginInfoAndConnetStatus()
        {
            GetCurrentDataBaseLoginInfoResponse response = new GetCurrentDataBaseLoginInfoResponse();

            try
            {
                DbUtility dbUtility = DbUtilityCreator.GetDefaultDbUtility();
                if (null != dbUtility)
                {
                    response.Ip = dbUtility.SqlconModel.DBIP;
                    response.Account = dbUtility.SqlconModel.DBUser;
                    response.Password = dbUtility.SqlconModel.DBPasswd;

                    if (dbUtility.TestConnection())
                    {
                        response.DataBaseConnecting = true;
                    }
                    else
                    {
                        response.DataBaseConnecting = false;
                    }
                }

            }
            catch (Exception ex)
            {
                string message = "操作失败。" + ex.Message;
                response.ResponseSucceed = false;
                response.Message = message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);
            }
           

            return response;
        }

        public TestConnetDataBaseResponse TestConnetDataBase(TestConnetDataBaseRequest request)
        {
            TestConnetDataBaseResponse  response = new TestConnetDataBaseResponse();

            if (null != request)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(request.Ip) &&
                        !string.IsNullOrWhiteSpace(request.Account) &&
                        !string.IsNullOrWhiteSpace(request.Password))
                    {
                        if (request.SaveConfig)
                        {
                            if (TestConnetDataBaseAndSave(request.Ip, request.Account, request.Password))
                            {
                                response.ResponseSucceed = true;
                            }
                            else
                            {
                                response.ResponseSucceed = false;
                                response.Message = "连接数据库失败";
                            }

                        }
                        else
                        {
                            if (TestConnetDataBaseButNotSave(request.Ip, request.Account, request.Password))
                            {
                                response.ResponseSucceed = true;
                            }
                            else
                            {
                                response.ResponseSucceed = false;
                                response.Message = "连接数据库失败";
                            }
                        }
                    }
                    else
                    {
                        response.ResponseSucceed = false;
                        response.Message = "必须填写完整信息：IP地址、账户、密码";
                    }
                }
                catch (Exception ex)
                {
                    string message = "操作失败。" + ex.Message;
                    response.ResponseSucceed = false;
                    response.Message = message;
                    LoggingFactory.GetLogger().WriteDebugLogger(message ); 
                }
            }

            return response;
        }

        #endregion

        private bool TestConnetDataBaseButNotSave(string ip, string account, string password)
        {
            bool connetSuccess = true;

            try
            {
                DbUtility dbUtility = DbUtilityCreator.GetDefaultDbUtility();
                DbUtility dbUtilityToTest = null;

                SQLConnModel sqlConnModel = dbUtility.SqlconModel;
                sqlConnModel.DBIP = ip;
                sqlConnModel.DBUser = account;
                sqlConnModel.DBPasswd = password;

                dbUtilityToTest = new DbUtility(sqlConnModel, dbUtility.DbProviderType);

                connetSuccess = dbUtilityToTest.TestConnection();

                if (connetSuccess)
                {
                    this.sqlConnModel = sqlConnModel;
                }
            }
            catch (Exception)
            {
                connetSuccess = false;
                throw;
            }

            return connetSuccess;
        }

        private bool TestConnetDataBaseAndSave(string ip, string account, string password)
        {
            bool connetSuccess = true;

            try
            {
                if (TestConnetDataBaseButNotSave(ip, account, password))
                {
                    SQLConnectControl sqlConnectControl = new SQLConnectControl();
                    sqlConnectControl.SaveConnectConfigDB(this.sqlConnModel);
                }
                else
                {
                    connetSuccess = false;
                }
            }
            catch (Exception)
            {
                connetSuccess = false;
                throw;
            }

            return connetSuccess;
        }

        private bool CreataDataSqlScript(IEnumerable<string> sqlScriptFilePaths)
        {
            bool connetSuccess = true;

            try
            {
                DbUtility dbUtility = DbUtilityCreator.GetDefaultDbUtility();
                if (dbUtility.DbProviderType == DbProviderType.MySql)
                {
                    //创建数据库脚本时，还没有数据库存在，所以
                    //connectionString不能存在数据库名，否则执行脚本不成功。
                    string connectionString = dbUtility.ConnectionString.Replace(dbUtility.SqlconModel.DBName, "");

                    foreach (var sqlScriptFilePath in sqlScriptFilePaths)
                    {
                        MySqlDataBaseHelper.ExecuteMySqlScriptFile(sqlScriptFilePath, connectionString);

                    }
                }
                else
                {
                    //TODO:处理其它数据库执行脚本代码
                }

            }
            catch (Exception)
            {
                connetSuccess = false;
                throw;
            }

            return connetSuccess;
        }

        private bool ExecuteSqlScript(IEnumerable<string> sqlScriptFilePaths)
        {
            bool connetSuccess = true;

            try
            {
                DbUtility dbUtility = DbUtilityCreator.GetDefaultDbUtility();
                if (dbUtility.DbProviderType == DbProviderType.MySql)
                {
                    foreach (var sqlScriptFilePath in sqlScriptFilePaths)
                    {
                        MySqlDataBaseHelper.ExecuteMySqlScriptFile(sqlScriptFilePath, dbUtility.ConnectionString);
                        
                    }
                }
                else
                {
                    //TODO:处理其它数据库执行脚本代码
                }

            }
            catch (Exception)
            {
                connetSuccess = false;
                throw;
            }

            return connetSuccess;
        }


    }
}
