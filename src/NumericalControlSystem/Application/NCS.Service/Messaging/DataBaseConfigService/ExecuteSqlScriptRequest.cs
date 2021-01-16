using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Service.Messaging.DataBaseConfigService
{
    public class ExecuteSqlScriptRequest
    {
        private List<string> sqlScriptFiles = new List<string>();
        public List<string> SqlScriptFiles
        {
            get { return sqlScriptFiles; }
            set { sqlScriptFiles = value; }
        }
    }
}
