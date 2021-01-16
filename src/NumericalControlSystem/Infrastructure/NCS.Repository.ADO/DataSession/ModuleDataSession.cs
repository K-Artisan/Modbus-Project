using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using NCS.Infrastructure.Logging;
using NCS.Infrastructure.Querying;
using NCS.Model.Entity;
using NCS.Repository.ADO.DataMapper;
using UniversalDAL;

namespace NCS.Repository.ADO.DataSession
{
    public class ModuleDataSession : IDataSession<Module, int> 
    {
        private DbUtility dbUtility;
        private string baseDeleteSql = "delete from numericalcontrolsystem.module ";

        public ModuleDataSession()
        {
            dbUtility = DbUtilityCreator.GetDefaultDbUtility();
        }

        #region IDataSession<DataPoint, int> members

        public void Add(Module entity)
        {
            MySqlParameter[] mySqlPrarameters =
            {
                 new MySqlParameter("@Number", entity.Number),
                 new MySqlParameter("@Name", entity.Name),
                 new MySqlParameter("@Description", entity.Description)
            };

            string sqlString = "insert into numericalcontrolsystem.module(Number,Name,Description) " +
                           "values(@Number,@Name,@Description);";

            try
            {
                dbUtility.ExecuteNonQuery(sqlString, CommandType.Text, mySqlPrarameters);
            }
            catch (Exception ex)
            {
                string message = "添加失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                throw new Exception(message);
            }
        }

        public void Remove(Module entity)
        {
            MySqlParameter[] mySqlPrarameters =
            {
                 new MySqlParameter("@ModuleId", entity.Id)
            };

            string sqlString = "delete from numericalcontrolsystem.module where ModuleId=@ModuleId;";

            try
            {
                dbUtility.ExecuteNonQuery(sqlString, CommandType.Text, mySqlPrarameters);
            }
            catch (Exception ex)
            {
                string message = "删除失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                throw new Exception(message);
            }

        }

        public void Remove(Query query)
        {
            List<MySqlParameter> parametersrarameters = new List<MySqlParameter>();
            string sqlString = query.TranslateIntoSqlString<Module>(this.baseDeleteSql, parametersrarameters);

            try
            {
                dbUtility.ExecuteNonQuery(sqlString, CommandType.Text, parametersrarameters.ToArray());
            }
            catch (Exception ex)
            {
                string message = "删除失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                throw new Exception(message);
            }

        }


        public void Save(Module entity)
        {
            MySqlParameter[] mySqlPrarameters =
            {
                 new MySqlParameter("@ModuleId", entity.Id),
                 new MySqlParameter("@Number", entity.Number),
                 new MySqlParameter("@Name", entity.Name),
                 new MySqlParameter("@Description", entity.Description),
            };

            string sqlString = "updata numericalcontrolsystem.module " +
                               "set Number=@Number, Name=@Name,Description=@Description " +
                               "where ModuleId=@ModuleId;";

            try
            {
                dbUtility.ExecuteNonQuery(sqlString, CommandType.Text, mySqlPrarameters);
            }
            catch (Exception ex)
            {
                string message = "修改失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                throw new Exception(message);
            }
        }

        public Module FindBy(int id)
        {
            Module module = new Module();
            DataTable dataTable;

            MySqlParameter[] mySqlPrarameters= { new MySqlParameter("@Id", id) };

            string moduleQuery = "select ModuleId,Number,Name,Description " +
                                 "from numericalcontrolsystem.module " +
                                 "where ModuleId=@Id;";

            try
            {
                dataTable = dbUtility.ExecuteDataTable(moduleQuery, CommandType.Text, mySqlPrarameters);
                if (null != dataTable)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        module = DataMapperFactory.GetDataMapper<Module>().ConverFrom(dataTable.Rows, i);
                    }
                }

            }
            catch (Exception ex)
            {
                string message = "查询失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return module;
            }

            return module;
        }

        public IEnumerable<Module> FindAll()
        {
            List<Module> modules = new List<Module>();
            DataTable dataTable;

            string query = "select ModuleId,Number,Name,Description " +
                           "from numericalcontrolsystem.module;";

            try
            {
                dataTable = dbUtility.ExecuteDataTable(query, CommandType.Text);
                if (null != dataTable)
                {
                    Module module;
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        module = DataMapperFactory.GetDataMapper<Module>().ConverFrom(dataTable.Rows, i);
                        modules.Add(module);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "查询失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return modules;
            }

            return modules;
        }

        public IEnumerable<Module> FindBy(Query query)
        {
            List<Module> modules = new List<Module>();
            DataTable dataTable;

            string baseSelectQuery = "select ModuleId,Number,Name,Description " +
                                     "from numericalcontrolsystem.module;";

            List<MySqlParameter> parametersrarameters = new List<MySqlParameter>();

            string resultQuery = query.TranslateIntoSqlString<DataPoint>(baseSelectQuery, parametersrarameters);

            try
            {
                dataTable = dbUtility.ExecuteDataTable(resultQuery, CommandType.Text, parametersrarameters.ToArray());
                if (null != dataTable)
                {
                    Module module;
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        module = DataMapperFactory.GetDataMapper<Module>().ConverFrom(dataTable.Rows, i);
                        modules.Add(module);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "查询失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return modules;
            }

            return modules;

        }

        public IEnumerable<Module> FindBy(Query query, int index, int count)
        {

            //TODO:该结果返回错误，需要更正
            IEnumerable<Module> modules = FindBy(query).Skip(index).Take(count);

            return modules;
        }

        #endregion
    }
}
