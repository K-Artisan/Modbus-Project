using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Repository.NHibernate.SessionStorage
{
    public static class SessionStorage
    {
        private static Dictionary<string, object> sessions= new Dictionary<string, object>();
        public static Dictionary<string, object> Sessions
        {
            get { return sessions; }
            set { sessions = value; }
        }
    }
}
