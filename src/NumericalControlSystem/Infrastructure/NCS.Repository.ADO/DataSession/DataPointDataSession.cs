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
    public class DataPointDataSession : IDataSession<DataPoint, int> 
    {
        private DbUtility dbUtility;

        private string baseSelectQuery = "select DataPointId,Number,Name,DeviceAddress,StartRegisterAddress,DataType,Description,ModuleId,DataPointType " +
                "from numericalcontrolsystem.datapoint ";

        private string baseDeleteSql = "delete from numericalcontrolsystem.datapoint ";

        public DataPointDataSession()
        {
            dbUtility = DbUtilityCreator.GetDefaultDbUtility();
        }

        #region IDataSession<DataPoint, int> members

        public void Add(DataPoint entity)
        {
            MySqlParameter[] mySqlPrarameters =
            {
                 new MySqlParameter("@Number", entity.Number),
                 new MySqlParameter("@Name", entity.Name),
                 new MySqlParameter("@DeviceAddress", entity.DeviceAddress),
                 new MySqlParameter("@StartRegisterAddress", entity.StartRegisterAddress),
                 new MySqlParameter("@DataType", entity.DataType),
                 new MySqlParameter("@DataPointType", entity.DataPointType),
                 new MySqlParameter("@Description", entity.Description),
                 new MySqlParameter("@ModuleId", entity.ModuleBelongTo.Id)
            };

            string sqlString = "insert into numericalcontrolsystem.datapoint(Number,Name,DeviceAddress,StartRegisterAddress,DataType,DataPointType,Description,ModuleId) " +
                           "values(@Number,@Name,@DeviceAddress,@StartRegisterAddress,@DataType,@DataPointType,@Description,@ModuleId);";

            try
            {
                dbUtility.ExecuteNonQuery(sqlString, CommandType.Text, mySqlPrarameters);
            }
            catch (Exception ex)
            {
                string message = "添加失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);
            }
        }

        public void Remove(DataPoint entity)
        {
            MySqlParameter[] mySqlPrarameters =
            {
                 new MySqlParameter("@DataPointId", entity.Id)
            };

            string sqlString = this.baseDeleteSql + " where DataPointId=@DataPointId;";

            try
            {
                dbUtility.ExecuteNonQuery(sqlString, CommandType.Text, mySqlPrarameters);
            }
            catch (Exception ex)
            {
                string message = "删除失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);
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
            }

        }

        public void Save(DataPoint entity)
        {
            MySqlParameter[] mySqlPrarameters =
            {
                 new MySqlParameter("@DataPointId", entity.Id),
                 new MySqlParameter("@Number", entity.Number),
                 new MySqlParameter("@Name", entity.Name),
                 new MySqlParameter("@DeviceAddress", entity.DeviceAddress),
                 new MySqlParameter("@StartRegisterAddress", entity.StartRegisterAddress),
                 new MySqlParameter("@DataType", entity.DataType),
                 new MySqlParameter("@DataPointType", entity.DataPointType),
                 new MySqlParameter("@Description", entity.Description),
                 new MySqlParameter("@ModuleId", entity.ModuleBelongTo.Id)
            };

            string sqlString = "updata numericalcontrolsystem.datapoint " +
                               "set Number=@Number, Name=@Name, DeviceAddress=@DeviceAddress, StartRegisterAddress=@StartRegisterAddress, DataType=@DataType, DataPointType=@DataPointType, Description=@Description, ModuleId=@ModuleId " +
                               "where DataPointId=@DataPointId;";

            try
            {
                dbUtility.ExecuteNonQuery(sqlString, CommandType.Text, mySqlPrarameters);
            }
            catch (Exception ex)
            {
                string message = "修改失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);
            }
        }

        public DataPoint FindBy(int id)
        {
            DataPoint dataPoint = new DataPoint();
            DataTable dataTable;

            MySqlParameter[] mySqlPrarametersForDataPointQuery = { new MySqlParameter("@Id", id) };
            string dataPointQuery = this.baseSelectQuery  +
                           "where DataPointId=@Id;";

            //string moduleQuery = "select ModuleId,Number,Name,Description " +
            //                     "from numericalcontrolsystem.module " +
            //                     "where ModuleId=@Id;";

            try
            {
                dataTable = dbUtility.ExecuteDataTable(dataPointQuery, CommandType.Text, mySqlPrarametersForDataPointQuery);
                if (null != dataTable)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        dataPoint = DataMapperFactory.GetDataMapper<DataPoint>().ConverFrom(dataTable.Rows, i);
                    }
                }

                //MySqlParameter[] mySqlPrarametersForModuleQuery = { new MySqlParameter("@Id", dataPoint.ModuleBelongTo.Id) };
                //dataTable = dbUtility.ExecuteDataTable(moduleQuery, CommandType.Text, mySqlPrarametersForDataPointQuery);
                //if (null != dataTable)
                //{
                //    for (int i = 0; i < dataTable.Rows.Count; i++)
                //    {
                //        dataPoint.ModuleBelongTo = DataMapperFactory.GetDataMapper<Module>().ConverFrom(dataTable.Rows, i);
                //    }
                //}

            }
            catch (Exception ex)
            {
                string message = "查询失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return dataPoint;
            }

            return dataPoint;
        }

        public IEnumerable<DataPoint> FindAll()
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            DataTable dataTable;

            string dataPointQuery = this.baseSelectQuery;

            try
            {
                dataTable = dbUtility.ExecuteDataTable(dataPointQuery, CommandType.Text);
                if (null != dataTable)
                {
                    DataPoint dataPoint;
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        dataPoint = DataMapperFactory.GetDataMapper<DataPoint>().ConverFrom(dataTable.Rows, i);
                        dataPoints.Add(dataPoint);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "查询失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return dataPoints;
            }

            return dataPoints;
        }

        public IEnumerable<DataPoint> FindBy(Query query)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            DataTable dataTable;

            List<MySqlParameter> parametersrarameters = new List<MySqlParameter>();

            string resultQuery = query.TranslateIntoSqlString<DataPoint>(this.baseSelectQuery, parametersrarameters);

            try
            {
                dataTable = dbUtility.ExecuteDataTable(resultQuery, CommandType.Text, parametersrarameters.ToArray());
                if (null != dataTable)
                {
                    DataPoint dataPoint;
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        dataPoint = DataMapperFactory.GetDataMapper<DataPoint>().ConverFrom(dataTable.Rows, i);
                        dataPoints.Add(dataPoint);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = "查询失败！" + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return dataPoints;
            }

            return dataPoints;

        }

        public IEnumerable<DataPoint> FindBy(Query query, int index, int count)
        {

            //IEnumerable<DataPoint> dataPoints1 = FindBy(query);
            //IEnumerable<DataPoint> dataPoints2 = dataPoints1.Skip(index);
            //IEnumerable<DataPoint> dataPoints = dataPoints2.Take(count);

            //TODO:该结果返回错误，需要更正
            IEnumerable<DataPoint> dataPoints = FindBy(query).Skip(index).Take(count);

            return dataPoints;
        }

        #endregion
    }
}
