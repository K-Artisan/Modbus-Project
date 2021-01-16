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
using SQLConnecter;
using UniversalDAL;

namespace NCS.Repository.ADO.DataSession
{
    public class DataPointHistoryDataSession : IDataSession<DataPointHistoryData, string> 
    {
        private DbUtility dbUtility;

        private string baseSelectQuery = "select DataPointHistoryDataId,DataPointId,DateTime,Value " +
                                     "from numericalcontrolsystem.datapointhistorydata ";

        private string baseDeleteSql = "delete from numericalcontrolsystem.datapointhistorydata ";

        public DataPointHistoryDataSession()
        {
            dbUtility = DbUtilityCreator.GetDefaultDbUtility();
        }

        #region IDataSession<DataPoint, int> members

        public void Add(DataPointHistoryData entity)
        {
            MySqlParameter[] mySqlPrarameters =
            {
                 //new MySqlParameter("@DataPointHistoryDataId", entity.Id),
                 new MySqlParameter("@DataPointHistoryDataId", Guid.NewGuid().ToString("D")),
                 new MySqlParameter("@DataPointId", entity.DataPoint.Id),
                 new MySqlParameter("@DateTime", entity.DateTime),
                 new MySqlParameter("@Value", entity.Value)
            };

            string sqlString = "insert into numericalcontrolsystem.datapointhistorydata(DataPointHistoryDataId,DataPointId,DateTime,Value) " +
                           "values(@DataPointHistoryDataId,@DataPointId,@DateTime,@Value);";

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

        public void Remove(DataPointHistoryData entity)
        {
            MySqlParameter[] mySqlPrarameters =
            {
                 new MySqlParameter("@DataPointHistoryDataId", entity.Id)
            };

            string sqlString = "delete from numericalcontrolsystem.datapointhistorydata " +
                               "where DataPointHistoryDataId=@DataPointHistoryDataId;";

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
            string sqlString = query.TranslateIntoSqlString<DataPointHistoryData>(this.baseDeleteSql, parametersrarameters);

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

        public void Save(DataPointHistoryData entity)
        {
            MySqlParameter[] mySqlPrarameters =
            {
                 new MySqlParameter("@DataPointHistoryDataId", entity.Id),
                 new MySqlParameter("@DataPointId", entity.DataPoint.Id),
                 new MySqlParameter("@DateTime", entity.DateTime),
                 new MySqlParameter("@Value", entity.Value)
            };

            string sqlString = "updata numericalcontrolsystem.datapointhistorydata " +
                               "set DataPointHistoryDataId=@DataPointHistoryDataId, DataPointId=@DataPointId, DateTime=@DateTime, DataType=@DataType, Value=@Value " +
                               "where DataPointHistoryDataId=@DataPointHistoryDataId;";

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

        public DataPointHistoryData FindBy(string id)
        {
            DataPointHistoryData dataPointHistoryData = new DataPointHistoryData();
            DataTable dataTable;

            MySqlParameter[] mySqlPrarameters = { new MySqlParameter("@DataPointHistoryDataId", id) };
            string sqlQuery = baseSelectQuery +
                           "where DataPointHistoryDataId=@DataPointHistoryDataId;";

            try
            {
                dataTable = dbUtility.ExecuteDataTable(sqlQuery, CommandType.Text, mySqlPrarameters);
                if (null != dataTable)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        dataPointHistoryData = DataMapperFactory.GetDataMapper<DataPointHistoryData>().ConverFrom(dataTable.Rows, i);
                    }
                }

            }
            catch (Exception ex)
            {
                string message = "查询失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return dataPointHistoryData;
            }

            return dataPointHistoryData;
        }

        public IEnumerable<DataPointHistoryData> FindAll()
        {
            List<DataPointHistoryData> dataPointHistoryDatas = new List<DataPointHistoryData>();
            DataTable dataTable;

            string sqlQuery = baseSelectQuery +
                                     ";";

            try
            {
                dataTable = dbUtility.ExecuteDataTable(sqlQuery, CommandType.Text);
                if (null != dataTable)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        DataPointHistoryData dataPointHistoryData =
                            DataMapperFactory.GetDataMapper<DataPointHistoryData>().ConverFrom(dataTable.Rows, i);
                        dataPointHistoryDatas.Add(dataPointHistoryData);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "查询失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return dataPointHistoryDatas;
            }

            return dataPointHistoryDatas;
        }

        public IEnumerable<DataPointHistoryData> FindBy(Query query)
        {
            List<DataPointHistoryData> dataPointHistoryDatas = new List<DataPointHistoryData>();
            DataTable dataTable;

            List<MySqlParameter> parametersrarameters = new List<MySqlParameter>();

            string resultQuery = query.TranslateIntoSqlString<DataPointHistoryData>(this.baseSelectQuery, parametersrarameters);

            try
            {
                dataTable = dbUtility.ExecuteDataTable(resultQuery, CommandType.Text, parametersrarameters.ToArray());
                if (null != dataTable)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                         DataPointHistoryData dataPointHistoryData = 
                             DataMapperFactory.GetDataMapper<DataPointHistoryData>().ConverFrom(dataTable.Rows, i);
                        dataPointHistoryDatas.Add(dataPointHistoryData);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "查询失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return dataPointHistoryDatas;
            }

            return dataPointHistoryDatas;

        }

        public IEnumerable<DataPointHistoryData> FindBy(Query query, int index, int count)
        {

            //TODO:该结果返回错误，需要更正
            IEnumerable<DataPointHistoryData> dataPointHistoryDatas = FindBy(query).Skip(index).Take(count);

            return dataPointHistoryDatas;
        }

        #endregion
    }
}
