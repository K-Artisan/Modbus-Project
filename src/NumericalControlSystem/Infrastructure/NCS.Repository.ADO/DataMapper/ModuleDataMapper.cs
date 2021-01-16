using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Logging;
using NCS.Model.Entity;

namespace NCS.Repository.ADO.DataMapper
{
    public class ModuleDataMapper : IDataMapper<Module>
    {
        private const string DebugBaseMessage = "数据库对象转换为实体时出现异常。\n" +
                                                "文件位置：NCS.Repository.ADO.DataMapper-MudleDataMappers-";


        private Dictionary<string, string> propertyMapToTableColumn = new Dictionary<string, string>()
            {
                {"Id", "ModuleId"},
                {"Number", "Number"},
                {"Name", "Name"},
                {"Description", "Description"},
            };

        public Dictionary<string, string> PropertyMapToTableColumn
        {
            get { return propertyMapToTableColumn; }
            set { propertyMapToTableColumn = value; }
        }


        public Module ConverFrom(DataRowCollection rows, int i)
        {
            return ConverToMudle(rows, i);
        }

        private static Module ConverToMudle(DataRowCollection rows, int i)
        {
            Module module = new Module();

            try
            {
                module.Id = rows[i]["ModuleId"] != null ? Convert.ToInt32(rows[i]["ModuleId"]) : -1;
                module.Number = rows[i]["Number"] != null ? Convert.ToInt32(rows[i]["Number"]) : -1;
                module.Name = rows[i]["Name"] != null ? Convert.ToString(rows[i]["Name"]) : "";
                module.Description = rows[i]["Description"] != null ? Convert.ToString(rows[i]["Description"]) : "";
            }
            catch (Exception ex)
            {
                string message = DebugBaseMessage + "public static Module ConverToMudle(DataRowCollection rows, int i)\n " + ex.Message;
                LoggingFactory.GetLogger().WriteDebugLogger(message);

                return module;
            }

            return module;
        }
    }
}
