using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Service.Messaging.DataBaseConfigService
{
    public class CreateDataBaseRequest
    {
        private List<string> sqlScriptFiles = new List<string>();
        public List<string> SqlScriptFiles
        {
            get { return sqlScriptFiles; }
            set { sqlScriptFiles = value; }
        }
    }
}
