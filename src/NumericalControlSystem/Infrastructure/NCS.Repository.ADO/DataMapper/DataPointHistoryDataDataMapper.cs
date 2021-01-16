using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Logging;
using NCS.Model.Entity;
using NCS.Model.Entity;

namespace NCS.Repository.ADO.DataMapper
{
    public class DataPointHistoryDataDataMapper : IDataMapper<DataPointHistoryData>
    {
        private const string DebugBaseMessage = "数据库对象转换为实体时出现异常。\n" +
                                                "文件位置：NCS.Repository.ADO.DataMapper-DataPointHistoryDataDataMapper-";


        private static Dictionary<string, string> propertyMapToTableColumn = new Dictionary<string, string>()
            {
                {"Id", "DataPointHistoryDataId"},
                {"DataPoint.Id", "DataPointId"},
                {"DateTime", "DateTime"},
                {"Value", "Value"},
            };

        public  Dictionary<string, string> PropertyMapToTableColumn
        {
            get { return propertyMapToTableColumn; }
            set { propertyMapToTableColumn = value; }
        }

        public DataPointHistoryData ConverFrom(DataRowCollection rows, int i)
        {
            return ConverToDataPointHistoryData(rows, i);
        }


        private static DataPointHistoryData ConverToDataPointHistoryData(DataRowCollection rows, int i)
        {
            DataPointHistoryData dataPointHistoryData = new DataPointHistoryData();

            try
            {
                dataPointHistoryData.Id = rows[i]["DatapointHistoryDataId"] != null ? Convert.ToString(rows[i]["DatapointHistoryDataId"]) : "";

                DataPoint dataPoint = new DataPoint();
                dataPoint.Id = rows[i]["DataPointId"] != null ? Convert.ToInt32(rows[i]["DataPointId"]) : -1;
                dataPointHistoryData.DataPoint = dataPoint;

                dataPointHistoryData.DateTime = rows[i]["DateTime"] != null ? Convert.ToDateTime(rows[i]["DateTime"]) : new DateTime();
                dataPointHistoryData.Value = rows[i]["Value"] != null ? Convert.ToDouble(rows[i]["Value"]) : -1;
            }
            catch (Exception ex)
            {
                string message = DebugBaseMessage + "public static DataPoint ConverToDataPointHistoryData(DataRowCollection rows)\n " + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return dataPointHistoryData;
            }

            return dataPointHistoryData;
        }
    }
}
