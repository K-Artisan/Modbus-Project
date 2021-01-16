using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLConnecter
{
    public class SQLConnModel
    {
        /// <summary>
        /// 数据库IP
        /// </summary>
        public string DBIP { set; get; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DBName { set; get; }

        /// <summary>
        /// 连接数据库用户
        /// </summary>
        public string DBUser { set; get; }

        /// <summary>
        /// 连接数据库密码
        /// </summary>
        public string DBPasswd { set; get; }
    }
}
