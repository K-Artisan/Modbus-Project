using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Logging;
using NCS.Model.Entity;
using NCS.Repository.ADO.DataMapper;

namespace NCS.Repository.ADO.DataMapper
{
    public class DataPointDataMapper : IDataMapper<DataPoint>
    {
        private const string DebugBaseMessage = "数据库对象转换为实体时出现异常。\n" +
                                                "文件位置：NCS.Repository.ADO.DataMapper-DataPointDataMapper-";
        /// <summary>
        /// key:属性名
        /// value:数据库表中的字段名
        /// </summary>
        private static Dictionary<string, string> propertyMapToTableColumn = new Dictionary<string, string>()
            {
                {"Id", "DataPointId"},
                {"Number", "Number"},
                {"Name", "Name"},
                {"DeviceAddress", "DeviceAddress"},
                {"StartRegisterAddress", "StartRegisterAddress"},
                {"DataType", "DataType"},
                {"DataPointType", "DataPointType"},
                {"Description", "Description"},
                {"ModuleBelongTo.Id", "ModuleId"},
            };

        /// <summary>
        /// key:属性名
        /// value:数据库表中的字段名
        /// </summary>
        public Dictionary<string, string> PropertyMapToTableColumn
        {
            get { return propertyMapToTableColumn; }
            set { propertyMapToTableColumn = value; }
        }

        public DataPoint ConverFrom(DataRowCollection rows, int i)
        {
            return ConverToDataPiont(rows, i);
        }

        private static DataPoint ConverToDataPiont(DataRowCollection rows, int i)
        {
            DataPoint dataPoint = new DataPoint();

            try
            {
                dataPoint.Id = rows[i][propertyMapToTableColumn["Id"]] != null ? Convert.ToInt32(rows[i][propertyMapToTableColumn["Id"]]) : -1;
                dataPoint.Number = rows[i][propertyMapToTableColumn["Number"]] != null ? Convert.ToInt32(rows[i][propertyMapToTableColumn["Number"]]) : -1;
                dataPoint.Name = rows[i][propertyMapToTableColumn["Name"]] != null ? Convert.ToString(rows[i]["Name"]) : "";
                dataPoint.DeviceAddress = rows[i][propertyMapToTableColumn["DeviceAddress"]] != null ? Convert.ToInt32(rows[i][propertyMapToTableColumn["DeviceAddress"]]) : 0;
                dataPoint.StartRegisterAddress = rows[i][propertyMapToTableColumn["StartRegisterAddress"]] != null ? Convert.ToInt32(rows[i][propertyMapToTableColumn["StartRegisterAddress"]]) :0;
                dataPoint.DataType = rows[i][propertyMapToTableColumn["DataType"]] != null
                    ? (DataType)Enum.Parse(typeof(DataType), Convert.ToString(rows[i][propertyMapToTableColumn["DataType"]]))
                    : DataType.S16;
                dataPoint.DataPointType = rows[i][propertyMapToTableColumn["DataPointType"]] != null
                    ? (DataPointType)Enum.Parse(typeof(DataPointType), Convert.ToString(rows[i][propertyMapToTableColumn["DataPointType"]]))
                    : DataPointType.ReadByFunNum03;
                dataPoint.Description = rows[i][propertyMapToTableColumn["Description"]] != null ? Convert.ToString(rows[i][propertyMapToTableColumn["Description"]]) : "";

                Module module = new Module();
                module.Id = rows[i][propertyMapToTableColumn["ModuleBelongTo.Id"]] != null ? Convert.ToInt32(rows[i][propertyMapToTableColumn["ModuleBelongTo.Id"]]) : -1;
                dataPoint.ModuleBelongTo = module;


            }
            catch (Exception ex)
            {
                string message = DebugBaseMessage + "public static DataPoint ConverTo(DataRowCollection rows)\n " + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return dataPoint;
            }

            return dataPoint;
        }
    }
}
