using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCS.Service.Messaging.DataBaseConfigService
{
    public class GetCurrentDataBaseLoginInfoResponse :AbstracttResponseBase
    {
        /// <summary>
        /// 数据库当前的连接状态,true,表示正在连接，否则表示无法连接
        /// </summary>
        public bool DataBaseConnecting { get; set; }

        /// <summary>
        /// 数据库IP
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 数据库账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } 
    }
}
